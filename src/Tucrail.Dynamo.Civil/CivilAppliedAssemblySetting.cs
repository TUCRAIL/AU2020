using Autodesk.Civil;

public class CivilAppliedAssemblySetting
{
    private CivilAppliedAssemblySetting() {}

    internal double FrequencyAlongTargetCurves { get; private set; }
    internal double? MODAlongTargetCurves { get; private set; }
    internal CorridorAlongOffsetTargetCurveOption TargetCurveOption { get; private set; }
    internal bool AppliedAdjacentToOffsetTargetStartEnd { get; private set; }
    internal bool AppliedAtOffsetTargetGeometryPoints { get; private set; }
    internal bool AppliedAtProfileHighLowPoints { get; private set; }
    internal bool AppliedAtProfileGeometryPoints { get; private set; }
    internal bool AppliedAtSuperelevationCriticalPoints { get; private set; }
    internal bool AppliedAtHorizontalGeometryPoints { get; private set; }
    internal double FrequencyAlongProfileCurves { get; private set; }
    internal double FrequencyAlongSpirals { get; private set; }
    internal double FrequencyAlongCurves { get; private set; }
    internal double? MODAlongCurves { get; private set; }
    internal CorridorAlongCurveOption CorridorAlongCurvesOption { get; private set; }
    internal double FrequencyAlongTangents { get; private set; }


    public static CivilAppliedAssemblySetting Create(bool targetStartEnd, 
        bool horizontalGeometryPoints, bool targetGeometryPoints, bool profileGeometryPoints, bool profileHighLowPoints, bool superelevationCriticalPoints, CorridorAlongCurveOption curvesOption, 
        double frequencyAlongCurves, double frequencyAlongProfileCurves, double frequencyAlongSpirals, double frequencyAlongTangents, double frequencyAlongTargetCurves, 
        double? modAlongCurves, double? modAlongTargetCurves, CorridorAlongOffsetTargetCurveOption targetCurveOption)
    {
        return new CivilAppliedAssemblySetting
        {
            FrequencyAlongTargetCurves = frequencyAlongTargetCurves,
            MODAlongTargetCurves = modAlongTargetCurves,
            TargetCurveOption = targetCurveOption,
            AppliedAdjacentToOffsetTargetStartEnd = targetStartEnd,
            AppliedAtOffsetTargetGeometryPoints = targetGeometryPoints,
            AppliedAtProfileHighLowPoints = profileHighLowPoints,
            AppliedAtProfileGeometryPoints = profileGeometryPoints,
            AppliedAtSuperelevationCriticalPoints = superelevationCriticalPoints,
            AppliedAtHorizontalGeometryPoints = horizontalGeometryPoints,
            FrequencyAlongProfileCurves = frequencyAlongProfileCurves,
            FrequencyAlongSpirals = frequencyAlongSpirals,
            FrequencyAlongCurves = frequencyAlongCurves,
            MODAlongCurves = modAlongCurves,
            CorridorAlongCurvesOption = curvesOption,
            FrequencyAlongTangents = frequencyAlongTangents
        };
    }
}
