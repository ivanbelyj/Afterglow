
public record ThreatTargetingEstimate
{
    /// <summary>
    /// Low and moderate values (typically 0-1):
    /// <para>
    /// Beneficiality of threat elimination at the moment
    /// (for example, dangerous threat in its vulnerable state - high value;
    /// not dangerous threat in its perfect state - typically low value).
    /// </para>
    /// 
    /// High values (typically 1-2):
    /// <para>
    /// Degree of critical harm that threat can cause imminently,
    /// no matter how beneficial the elimination is right now.
    /// For example, high value when very dangerous enemy threatens
    /// character's partner
    /// </para>
    /// </summary>
    public float Utility { get; set; }

    /// <summary>
    /// Original threat estimate
    /// </summary>
    public ThreatEstimate ThreatEstimate { get; set; }
}
