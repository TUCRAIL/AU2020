using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DynamoNodes;
using Autodesk.AutoCAD.Geometry;
using Autodesk.DesignScript.Geometry;
using Dynamo.Graph.Nodes;
using BlockReference = Autodesk.AutoCAD.DatabaseServices.BlockReference;
using Object = Autodesk.AutoCAD.DynamoNodes.Object;

public class CadDynamicBlock : Object
{
    internal CadDynamicBlock(BlockReference block, bool isDynamoOwned) : base(block, isDynamoOwned)
    {
    }

    internal BlockReference Block => AcObject as BlockReference;

    /// <summary>
    /// Convert from a BlockReference
    /// </summary>
    public static CadDynamicBlock ByConvertFromBlock(Autodesk.AutoCAD.DynamoNodes.BlockReference dbr)
        => dbr == null ? null : new CadDynamicBlock((BlockReference)dbr.InternalDBObject, false);

    /// <summary>
    /// Rotate a CadDynamicBlock based on a given normal and angle
    /// </summary>
    public static CadDynamicBlock ByRotation(CadDynamicBlock dynamicBlock, Vector normal, double degree)
    {
        if (dynamicBlock == null) return null;

        var block = dynamicBlock.Block;
        var normalVector = new Vector3d(normal.X, normal.Y, normal.Z);
        block.TransformBy(Matrix3d.Rotation(degree * Math.PI / 180, normalVector, block.Position));

        return new CadDynamicBlock(block, false);
    }

    /// <summary>
    /// Retrieves all block references, included the dynamic
    /// </summary>
    public static CadDynamicBlock[] GetDynamicBlockReferences(Document document, string blockName)
    {
        if (document == null) return null;
        if (string.IsNullOrEmpty(blockName)) return null;

        var db = document.AcDocument.Database;
        var references = new List<CadDynamicBlock>();

        using (var trans = db.TransactionManager.StartTransaction())
        {
            var bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead, false, true);

            if (bt.Has(blockName))
            {
                var definition = (BlockTableRecord)trans.GetObject(bt[blockName], OpenMode.ForRead, false, true);

                var referenceIds = definition.GetBlockReferenceIds(true, false);

                if (definition.IsDynamicBlock)
                {
                    var anonymousIds = definition.GetAnonymousBlockIds();

                    foreach (ObjectId Id in anonymousIds)
                    {
                        var anonymous = (BlockTableRecord)trans.GetObject(Id, OpenMode.ForRead, false, true);
                        foreach (ObjectId refId in anonymous.GetBlockReferenceIds(true, false))
                            referenceIds.Add(refId);
                    }
                }

                foreach (ObjectId id in referenceIds)
                    references.Add(new CadDynamicBlock((BlockReference)trans.GetObject(id, OpenMode.ForRead, false, true), false));
            }

            trans.Commit();
        }

        return references.ToArray();
    }

    /// <summary>
    /// Get the attribute value
    /// </summary>
    public static string GetAttributeValue(CadDynamicBlock dynamicBlock, string attributeName)
    {
        if (dynamicBlock == null) return null;
        if (string.IsNullOrEmpty(attributeName)) return null;

        var block = dynamicBlock.Block;

        using (var trans = block.Database.TransactionManager.StartTransaction())
        {
            foreach (ObjectId attId in block.AttributeCollection)
            {
                var att = (AttributeReference)trans.GetObject(attId, OpenMode.ForRead, false, true);

                if (att.Tag == attributeName)
                    return att.IsMTextAttribute ? att.MTextAttribute.Text : att.TextString;
            }

            trans.Commit();
        }

        using (var trans = block.Database.TransactionManager.StartTransaction())
        {
            var btr = (BlockTableRecord)trans.GetObject(block.BlockTableRecord, OpenMode.ForRead, false, true);

            if (btr.HasAttributeDefinitions)
            {
                foreach (var oid in btr)
                {
                    var dbObject = trans.GetObject(oid, OpenMode.ForRead, false, true);

                    if (dbObject is AttributeDefinition att)
                    {
                        if (att.Tag == attributeName)
                            return att.IsMTextAttributeDefinition ? att.MTextAttributeDefinition.Text : att.TextString;
                    }
                }
            }

            trans.Commit();
        }

        return null;
    }

    /// <summary>
    /// Set the attribute value
    /// </summary>
    public static bool SetAttributeValue(CadDynamicBlock dynamicBlock, string attributeName, string attributeValue)
    {
        if (dynamicBlock == null) return false;
        if (string.IsNullOrEmpty(attributeName)) return false;
        if (string.IsNullOrEmpty(attributeValue)) return false;

        var block = dynamicBlock.Block;

        using (var trans = block.Database.TransactionManager.StartTransaction())
        {
            foreach (ObjectId attId in block.AttributeCollection)
            {
                var att = (AttributeReference)trans.GetObject(attId, OpenMode.ForWrite, false, true);

                if (att.Tag == attributeName)
                {
                    if (att.IsMTextAttribute)
                        att.MTextAttribute.Contents = attributeValue;

                    att.TextString = attributeValue;
                    return true;
                }
            }

            trans.Commit();
        }

        using (var trans = block.Database.TransactionManager.StartTransaction())
        {
            var btr = (BlockTableRecord)trans.GetObject(block.BlockTableRecord, OpenMode.ForRead, false, true);

            if (btr.HasAttributeDefinitions)
            {
                foreach (var oid in btr)
                {
                    var dbObject = trans.GetObject(oid, OpenMode.ForWrite, false, true);

                    if (dbObject is AttributeDefinition)
                    {
                        var att = (AttributeDefinition)dbObject;

                        if (att.Tag == attributeName)
                        {
                            if (att.IsMTextAttributeDefinition)
                                att.MTextAttributeDefinition.Contents = attributeValue;

                            att.TextString = attributeValue;
                            return true;
                        }
                    }
                }
            }

            trans.Commit();
        }

        return false;
    }

    /// <summary>
    /// Get the dynamic property value
    /// </summary>
    public static object GetPropertyValue(CadDynamicBlock dynamicBlock, string propertyName)
    {
        if (dynamicBlock == null) return null;
        if (string.IsNullOrEmpty(propertyName)) return null;

        var block = dynamicBlock.Block;

        using (var trans = block.Database.TransactionManager.StartTransaction())
        {
            var btr = (BlockTableRecord)trans.GetObject(block.DynamicBlockTableRecord, OpenMode.ForRead, false, true);

            if (btr.IsDynamicBlock)
            {
                var dynBlock = (BlockReference)trans.GetObject(block.Id, OpenMode.ForRead, false, true);
                foreach (DynamicBlockReferenceProperty dbrProperty in dynBlock.DynamicBlockReferencePropertyCollection)
                {
                    //var values = dbrProperty.GetAllowedValues();
                    if (dbrProperty.PropertyName == propertyName)
                        return dbrProperty.Value;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Set the dynamic property value
    /// </summary>
    public static bool SetPropertyValue(CadDynamicBlock dynamicBlock, string propertyName, object propertyValue)
    {
        if (dynamicBlock == null) return false;
        if (string.IsNullOrEmpty(propertyName)) return false;
        if (propertyValue == null) return false;

        var block = dynamicBlock.Block;

        using (var trans = block.Database.TransactionManager.StartTransaction())
        {
            var btr = (BlockTableRecord)trans.GetObject(block.DynamicBlockTableRecord, OpenMode.ForRead, false, true);

            if (btr.IsDynamicBlock)
            {
                var dynBlock = (BlockReference)trans.GetObject(block.Id, OpenMode.ForWrite, false, true);
                var properties = dynBlock.DynamicBlockReferencePropertyCollection;

                foreach (DynamicBlockReferenceProperty property in properties)
                    if (property.PropertyName == propertyName)
                        switch ((DwgDataType)property.PropertyTypeCode)
                        {
                            case DwgDataType.Real:
                                {
                                    if (propertyValue is double)
                                    {
                                        property.Value = propertyValue;
                                        return true;
                                    }
                                    break;
                                }
                            case DwgDataType.Int16:
                            case DwgDataType.Int32:
                                {
                                    if (Convert.GetTypeCode(propertyValue) == TypeCode.Int16
                                        || Convert.GetTypeCode(propertyValue) == TypeCode.Int32)
                                    {
                                        var value = Convert.ToInt32(propertyValue);

                                        // TODO: why is this?
                                        if (value == 0 || value == 1)
                                        {
                                            property.Value = Convert.ToInt16(propertyValue);
                                            return true;
                                        }
                                    }
                                    break;
                                }
                            case DwgDataType.Text:
                                {
                                    if (propertyValue is string)
                                    {
                                        property.Value = propertyValue;
                                        return true;
                                    }

                                    break;
                                }
                            case DwgDataType.Dwg3Real:
                                {
                                    if (propertyValue is Point dynamoPoint)
                                    {
                                        property.Value = new Point3d(dynamoPoint.X, dynamoPoint.Y, dynamoPoint.Z);
                                        return true;
                                    }
                                    break;
                                }
                            default:
                                {
                                    return false;
                                }
                        }
            }

            trans.Commit();
        }

        return default;
    }
}
