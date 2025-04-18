
public record PerceptedPhysicalImpact : IPerceptedSensoryData<PerceivedPhysicalImpact>
{
    public PerceptionEntry PerceptionEntry { get; set; }
    public PerceivedPhysicalImpact PhysicalImpact { get; set; }

    PerceivedPhysicalImpact IPerceptedSensoryData<PerceivedPhysicalImpact>.Representation => PhysicalImpact;

    public PerceptedPhysicalImpact(PerceivedPhysicalImpact physicalImpact, PerceptionEntry perception)
    {
        PhysicalImpact = physicalImpact;
        PerceptionEntry = perception;
    }
}
