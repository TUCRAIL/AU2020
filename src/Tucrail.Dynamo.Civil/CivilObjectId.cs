using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DynamoNodes;
using Autodesk.Civil.ApplicationServices;
using Dynamo.Graph.Nodes;


public class CivilObjectId
{
    private CivilObjectId()
    {
    }

    /// <summary>
    /// Get the ObjectId for the given profile style name
    /// </summary>
    public static ObjectId GetProfileStyleId(Document document, string styleName)
    {
        if(document == null)
            return ObjectId.Null;
        if (string.IsNullOrEmpty(styleName))
            return ObjectId.Null;

        var db = document.AcDocument.Database;
        var doc = CivilDocument.GetCivilDocument(db);

        var styleId = ObjectId.Null;

        if (doc.Styles.ProfileStyles.Contains(styleName))
            styleId = doc.Styles.ProfileStyles[styleName];

        return styleId;
    }

    /// <summary>
    /// Get the ObjectId for the given label set style name
    /// </summary>
    public static ObjectId GetLabelSetStyleId(Document document, string type, string styleName)
    {
        if (document == null)
            return ObjectId.Null;
        if (string.IsNullOrEmpty(type))
            return ObjectId.Null;
        if (string.IsNullOrEmpty(styleName))
            return ObjectId.Null;

        var db = document.AcDocument.Database;
        var doc = CivilDocument.GetCivilDocument(db);

        if (type == "AlignmentLabel")
        {
            if (doc.Styles.LabelSetStyles.AlignmentLabelSetStyles.Contains(styleName))
                return doc.Styles.LabelSetStyles.AlignmentLabelSetStyles[styleName];
        }
        else if (type == "ProfileLabel")
        {
            if (doc.Styles.LabelSetStyles.ProfileLabelSetStyles.Contains(styleName))
                return doc.Styles.LabelSetStyles.ProfileLabelSetStyles[styleName];
        }
        else if (type == "SectionLabel")
        {
            if (doc.Styles.LabelSetStyles.SectionLabelSetStyles.Contains(styleName))
                return doc.Styles.LabelSetStyles.SectionLabelSetStyles[styleName];
        }

        return ObjectId.Null;
    }

    /// <summary>
    /// Get the ObjectId for the given layer name
    /// </summary>
    public static ObjectId GetLayerId(Document document, string layerName)
    {
        if (document == null) return ObjectId.Null;
        if (string.IsNullOrEmpty(layerName)) return ObjectId.Null;

        var db = document.AcDocument.Database;

        var layerId = ObjectId.Null;

        using (var trans = db.TransactionManager.StartTransaction())
        {
            var layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead, false, true);

            if (layerTable.Has(layerName))
                layerId = layerTable[layerName];

            trans.Commit();
        }

        return layerId;
    }
}
