using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DynamoApp.Services;
using Autodesk.AutoCAD.DynamoNodes;
using Autodesk.AutoCAD.Geometry;
using Autodesk.DesignScript.Geometry;
using Dynamo.Graph.Nodes;
using DynamoServices;
using Object = Autodesk.AutoCAD.DynamoNodes.Object;
using Solid3d = Autodesk.AutoCAD.DatabaseServices.Solid3d;

[RegisterForTrace]
public class CadSolid : Object
{
    internal CadSolid(Solid3d solid, bool isDynamoOwned) : base(solid, isDynamoOwned)
    {
    }

    /// <summary>
    /// Create a <see cref="CadSolid"/> as lofted between cross sections
    /// </summary>
    /// <param name="document"></param>
    /// <param name="crossSections"></param>
    /// <param name="layer">set the layer for the solid (can be null)</param>
    /// <returns></returns>
    public static CadSolid ByLoft(Document document, Polyline3D[] crossSections, string layer)
    {
        if (document == null) return null;
        if (crossSections?.Any() != true) return null;

        var db = document.AcDocument.Database;

        using (var ctx = new DocumentContext(db))
        {
            var id = ElementBinder.GetObjectIdFromTrace(ctx.Database);

            var apply = new Action<Solid3d>(solid =>
            {
                var polylines = crossSections.Select(o => (Polyline3d)o.InternalDBObject).ToArray();
                solid.CreateLoftedSolid(polylines, new Entity[]{}, null, new LoftOptions());

                if(string.IsNullOrWhiteSpace(layer))
                    solid.Layer = layer;
            });

            using (var trans = db.TransactionManager.StartTransaction())
            {
                if (id.IsValid & !id.IsErased)
                {
                    var solid = (Solid3d)trans.GetObject(id, OpenMode.ForWrite, false, true);
                    apply(solid);
                }
                else
                {
                    var solid = new Solid3d();
                    apply(solid);

                    var bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead, false, true);
                    var modelspace = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite, false, true);

                    id = modelspace.AppendEntity(solid);
                    trans.AddNewlyCreatedDBObject(solid, true);
                }

                trans.Commit();
            }

            var solidObj = ctx.GetTransaction().GetObject(id, OpenMode.ForRead, false, true) as Solid3d;
            return solidObj != null ? new CadSolid(solidObj, true) : null;
        }
    }
}

