using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DynamoApp.Services;
using Autodesk.AutoCAD.DynamoNodes;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Dynamo.Graph.Nodes;
using DynamoServices;


[RegisterForTrace]
public class CivilProfile : Object
{
    internal CivilProfile(Profile profile, bool isDynamoOwned) : base(profile, isDynamoOwned)
    {
    }

    /// <summary>
    /// Create a profile based on an asset
    /// </summary>
    public static CivilProfile CreateFromAsset(string assetName, string assetDescription, double assetStartStation, double assetEndStation,
                                Autodesk.Civil.DynamoNodes.Profile dynProfile, ObjectId layerId, ObjectId styleId, ObjectId labelSetId, Document document)
    {
        if (string.IsNullOrEmpty(assetName)) return null;
        if (string.IsNullOrEmpty(assetDescription)) return null;
        if (dynProfile == null) return null;
        if (document == null) return null;

        var profile = (Profile)dynProfile.InternalDBObject;
        var db = document.AcDocument.Database;

        using (var ctx = new DocumentContext(db))
        {
            var id = ElementBinder.GetObjectIdFromTrace(ctx.Database);
            if (id.IsValid && !id.IsErased)
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    var existingProfile = (Profile)trans.GetObject(id, OpenMode.ForWrite, false, true);
                    existingProfile.Entities.Clear();

                    var startLevel = profile.ElevationAt(assetStartStation);
                    var endLevel = profile.ElevationAt(assetEndStation);
                    var startPoint = new Point2d(assetStartStation, startLevel);
                    var endPoint = new Point2d(assetEndStation, endLevel);

                    existingProfile.Entities.AddFixedTangent(startPoint, endPoint);
                    existingProfile.Description = assetDescription;

                    trans.Commit();
                }
            }
            else
            {
                if (layerId.IsNull) return null;
                if (styleId.IsNull) return null;
                if (labelSetId.IsNull) return null;

                using (var trans = db.TransactionManager.StartTransaction())
                {
                    id = Profile.CreateByLayout(assetName, profile.AlignmentId, layerId, styleId, labelSetId);
                    var assetProfile = (Profile)trans.GetObject(id, OpenMode.ForWrite, false, true);

                    var startLevel = profile.ElevationAt(assetStartStation);
                    var endLevel = profile.ElevationAt(assetEndStation);
                    var startPoint = new Point2d(assetStartStation, startLevel);
                    var endPoint = new Point2d(assetEndStation, endLevel);

                    assetProfile.Entities.AddFixedTangent(startPoint, endPoint);
                    assetProfile.Description = assetDescription;

                    trans.Commit();
                }
            }

            var profileObject = ctx.GetTransaction().GetObject(id, OpenMode.ForRead, false, true) as Profile;
            return profileObject != null ? new CivilProfile(profileObject, true) : null;
        }
    }

    /// <summary>
    /// Get the grade at a given station
    /// </summary>
    public static double GetGradeAt(Autodesk.Civil.DynamoNodes.Profile profile, double station)
    {
        if (profile == null)
            return -1.0;

        return ((Profile) profile.InternalDBObject).GradeAt(station);
    }
}
