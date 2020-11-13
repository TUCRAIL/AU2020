using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DynamoApp.Services;
using Autodesk.AutoCAD.DynamoNodes;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Dynamo.Graph.Nodes;
using DynamoServices;


[RegisterForTrace]
public class CivilCorridor : Object
{
    internal CivilCorridor(Corridor corridor, bool isDynamoOwned) : base(corridor, isDynamoOwned)
    {
        Name = corridor.Name;
    }

    /// <summary>
    /// Get the corridor name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Create a corridor based on a switch name
    /// </summary>
    public static CivilCorridor CreateSwitchCorridor(string alignmentName, string prefix, string[] switches, Document document)
    {
        if (string.IsNullOrEmpty(alignmentName)) return null;
        if (string.IsNullOrEmpty(prefix)) return null;
        if (switches == null) return null;
        if (document == null) return null;

        var db = document.AcDocument.Database;
        var doc = CivilDocument.GetCivilDocument(db);

        using (var ctx = new DocumentContext(db))
        {
            var id = ElementBinder.GetObjectIdFromTrace(ctx.Database);

            var switchList = "Connected Switches: ";

            foreach (var @switch in switches)
                switchList += "-" + @switch;

            if (id.IsValid && !id.IsErased)
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    var switchCorridor = (Corridor)trans.GetObject(id, OpenMode.ForWrite, false, true);
                    switchCorridor.Name = prefix + alignmentName;
                    switchCorridor.Description = switchList;
                    trans.Commit();
                }
            }
            else
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    var corridorName = prefix + alignmentName;
                    id = doc.CorridorCollection.Add(corridorName);
                    var switchCorridor = (Corridor)trans.GetObject(id, OpenMode.ForWrite, false, true);
                    switchCorridor.Description = switchList;
                    trans.Commit();
                }
            }

            var corridor = ctx.GetTransaction().GetObject(id, OpenMode.ForRead, false, true) as Corridor;
            return corridor != null ? new CivilCorridor(corridor, true) : null;
        }
    }
}
