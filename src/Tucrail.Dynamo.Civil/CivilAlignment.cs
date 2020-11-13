using System.Collections.Generic;
using System.Linq;
using Autodesk.Civil;
using Autodesk.Civil.DynamoNodes;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;


public class CivilAlignment
{
    private CivilAlignment()
    {
    }

    /// <summary>
    /// Get the speed at a given station
    /// </summary>
    public static double GetSpeedAt(Alignment alignment, double station)
    {
        if (alignment == null)
            return -1.0;

        return ((Autodesk.Civil.DatabaseServices.Alignment) alignment.InternalDBObject).DesignSpeeds.FirstOrDefault(o => station >= o.Station)?.Value ?? -1.0;
    }

    /// <summary>
    /// Get the station and offset for a given point of an alignment
    /// </summary>
    [MultiReturn("Station", "Offset")]
    public static Dictionary<string, double> StationOffset(Alignment alignment, Point point)
    {
        if (alignment == null) return null;
        if (point == null) return null;

        var station = 0.0;
        var offset = 0.0;

        ((Autodesk.Civil.DatabaseServices.Alignment)alignment.InternalDBObject).StationOffset(point.X, point.Y, ref station, ref offset);

        return new Dictionary<string, double> { { "Station", station }, { "Offset", offset } };
    }

    /// <summary>
    /// Get the cant value at a given station
    /// </summary>
    [MultiReturn("Cant", "PivotType")]
    public static Dictionary<string, object> GetCantInfoAt(Alignment alignment, double station)
    {
        if (alignment == null) return null;

        var cant = ((Autodesk.Civil.DatabaseServices.Alignment)alignment.InternalDBObject).RailAlignmentInfo.GetCantInfoAtStation(station);

        string PivotType;

        if (cant.Pivot == RailAlignmentPivotType.None)
            PivotType = "None";
        else if (cant.Pivot == RailAlignmentPivotType.LeftRail)
            PivotType = "LeftRail";
        else if (cant.Pivot == RailAlignmentPivotType.RightRail)
            PivotType = "RightRail";
        else if (cant.Pivot == RailAlignmentPivotType.Centerline)
            PivotType = "CenterLine";
        else
            PivotType = "error";

        return new Dictionary<string, object> { { "Cant", cant.AppliedCANT }, { "PivotType", PivotType } };
    }
}
