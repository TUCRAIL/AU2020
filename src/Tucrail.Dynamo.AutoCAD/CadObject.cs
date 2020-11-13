using System;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DynamoApp.Services;
using Autodesk.AutoCAD.DynamoNodes;
using Dynamo.Graph.Nodes;


public class CadObject
{
    private CadObject()
    {
    }

    /// <summary>
    /// Change an object's color based on it's handle
    /// </summary>
    public static bool SetColor(Document document, string handle, int red, int green, int blue)
    {
        if (document == null) return false;
        if (string.IsNullOrEmpty(handle)) return false;

        var db = document.AcDocument.Database;

        // And attempt to get an ObjectId for the Handle
        var id = DocumentContext.GetObjectId(db, handle);

        using (var ctx = new DocumentContext(document.AcDocument))
        {
            var selected = (Entity)ctx.GetTransaction().GetObject(id, OpenMode.ForWrite, false, true);
            selected.Color = Color.FromRgb((byte)red, (byte)green, (byte)blue);
        }

        return true;
    }
}
