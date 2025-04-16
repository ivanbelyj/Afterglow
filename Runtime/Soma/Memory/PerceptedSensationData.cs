
public record PerceptedSensationData : IPerceptedSensoryData<SensationData>
{
    public PerceptionEntry PerceptionEntry { get; set; }
    public SensationData SensationData { get; set; }

    SensationData IPerceptedSensoryData<SensationData>.Representation => SensationData;

    public PerceptedSensationData(SensationData sensation, PerceptionEntry perception)
    {
        SensationData = sensation;
        PerceptionEntry = perception;
    }
}
