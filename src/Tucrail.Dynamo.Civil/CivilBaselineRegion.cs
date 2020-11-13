using Autodesk.Civil.DynamoNodes;
using System.Linq;
using Dynamo.Graph.Nodes;
using Corridor = Autodesk.Civil.DatabaseServices.Corridor;
using AppliedAssemblySetting = Autodesk.Civil.DatabaseServices.AppliedAssemblySetting;

public class CivilBaselineRegion
{
    private CivilBaselineRegion()
    {
    }

    /// <summary>
    /// Create a baseline region based on a region
    /// </summary>
    public static int Create(Baseline baseline, string regionName, string assemblyName, double startStation, double endStation,
                            double[] additionalStations, string[] additionalStationDescriptions, CivilAppliedAssemblySetting appliedAssemblySetting)
    {
        if (string.IsNullOrEmpty(regionName)) return -1;
        if (string.IsNullOrEmpty(assemblyName)) return -1;
        if (baseline == null) return -1;

        var corridor = (Corridor)baseline.Corridor.InternalDBObject;
        var baseBaseline = corridor.Baselines[baseline.Name];

        baseBaseline.BaselineRegions.Remove(regionName);
        var baselineRegion = baseBaseline.BaselineRegions.Add(regionName, assemblyName, startStation, endStation);

        if (additionalStations?.Any() == true)
        {
            baselineRegion.ClearAdditionalStations();

            if (additionalStations.Length == additionalStationDescriptions?.Length)
            {
                for (var i = 0; i < additionalStations.Length; i++)
                {
                    var station = additionalStations[i];
                    var description = additionalStationDescriptions[i] ?? string.Empty;
                    baselineRegion.AddStation(station, description);
                }
            }
            else
            {
                foreach (var station in additionalStations)
                    baselineRegion.AddStation(station, string.Empty);
            }
        }

        if (appliedAssemblySetting != null && baselineRegion.AppliedAssemblySetting != null)
        {
            baselineRegion.AppliedAssemblySetting.FrequencyAlongTargetCurves = appliedAssemblySetting.FrequencyAlongTargetCurves;
            baselineRegion.AppliedAssemblySetting.MODAlongTargetCurves = appliedAssemblySetting.MODAlongTargetCurves.GetValueOrDefault();
            baselineRegion.AppliedAssemblySetting.TargetCurveOption = appliedAssemblySetting.TargetCurveOption;
            baselineRegion.AppliedAssemblySetting.AppliedAdjacentToOffsetTargetStartEnd = appliedAssemblySetting.AppliedAdjacentToOffsetTargetStartEnd;
            baselineRegion.AppliedAssemblySetting.AppliedAtOffsetTargetGeometryPoints = appliedAssemblySetting.AppliedAtOffsetTargetGeometryPoints;
            baselineRegion.AppliedAssemblySetting.AppliedAtProfileHighLowPoints = appliedAssemblySetting.AppliedAtProfileHighLowPoints;
            baselineRegion.AppliedAssemblySetting.AppliedAtProfileGeometryPoints = appliedAssemblySetting.AppliedAtProfileGeometryPoints;
            baselineRegion.AppliedAssemblySetting.AppliedAtSuperelevationCriticalPoints = appliedAssemblySetting.AppliedAtSuperelevationCriticalPoints;
            baselineRegion.AppliedAssemblySetting.AppliedAtHorizontalGeometryPoints = appliedAssemblySetting.AppliedAtHorizontalGeometryPoints;
            baselineRegion.AppliedAssemblySetting.FrequencyAlongProfileCurves = appliedAssemblySetting.FrequencyAlongProfileCurves;
            baselineRegion.AppliedAssemblySetting.FrequencyAlongSpirals = appliedAssemblySetting.FrequencyAlongSpirals;
            baselineRegion.AppliedAssemblySetting.FrequencyAlongCurves = appliedAssemblySetting.FrequencyAlongCurves;
            baselineRegion.AppliedAssemblySetting.MODAlongCurves = appliedAssemblySetting.MODAlongCurves.GetValueOrDefault();
            baselineRegion.AppliedAssemblySetting.CorridorAlongCurvesOption = appliedAssemblySetting.CorridorAlongCurvesOption;
            baselineRegion.AppliedAssemblySetting.FrequencyAlongTangents = appliedAssemblySetting.FrequencyAlongTangents;
        }

        return baseBaseline.BaselineRegions.IndexOf(baseBaseline.BaselineRegions[regionName]);
    }

    /// <summary>
    /// Create a baseline region based on an asset
    /// </summary>
    public static int CreateFromAsset(Baseline baseline, string assetName, string assemblyName, double assetStartStation, double assetEndStation)
    {
        if (string.IsNullOrEmpty(assetName)) return -1;
        if (string.IsNullOrEmpty(assemblyName)) return -1;
        if (baseline == null) return -1;

        var corridor = (Corridor)baseline.Corridor.InternalDBObject;
        var baseBaseline = corridor.Baselines[baseline.Name];
        baseBaseline.BaselineRegions.Add(assetName, assemblyName, assetStartStation, assetEndStation);
        return baseBaseline.BaselineRegions.IndexOf(baseBaseline.BaselineRegions[assetName]);
    }
}
