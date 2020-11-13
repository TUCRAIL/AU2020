using Autodesk.Civil.DynamoNodes;
using Dynamo.Graph.Nodes;
using Corridor = Autodesk.Civil.DatabaseServices.Corridor;
using Profile = Autodesk.Civil.DatabaseServices.Profile;


public class CivilBaseline
{
    private CivilBaseline()
    {
    }

    /// <summary>
    /// Create a new baseline in a corridor based on a given alignment and profile
    /// </summary>
    public static string CreateFromAlignmentAndProfile(string baselineName, Autodesk.Civil.DynamoNodes.Corridor corridor, Alignment alignment, Autodesk.Civil.DynamoNodes.Profile profile)
    {
        if (string.IsNullOrEmpty(baselineName)) return null;
        if (corridor == null) return null;
        if (alignment == null) return null;
        if (profile == null) return null;

        var baseCorridor = (Corridor)corridor.InternalDBObject;
        var baseAlignment = (Autodesk.Civil.DatabaseServices.Alignment)alignment.InternalDBObject;
        var baseProfile = (Profile)profile.InternalDBObject;

        var db = baseCorridor.Database;

        using (var trans = db.TransactionManager.StartTransaction())
        {
            var exists = false;

            for (var i = 0; i <= baseCorridor.Baselines.Count - 1; i++)
            {
                var tempBaseline = baseCorridor.Baselines[i];
                if (tempBaseline.Name == baselineName)
                    exists = true;
            }

            Autodesk.Civil.DatabaseServices.Baseline switchBaseline;

            if (!exists)
            {
                switchBaseline = baseCorridor.Baselines.Add(baselineName, baseAlignment.Name, baseProfile.Name);
            }
            else
            {
                switchBaseline = baseCorridor.Baselines[baselineName];
                switchBaseline.SetAlignmentAndProfile(baseAlignment.Id, baseProfile.Id);
            }

            trans.Commit();

            return switchBaseline.Name;
        }
    }
}
