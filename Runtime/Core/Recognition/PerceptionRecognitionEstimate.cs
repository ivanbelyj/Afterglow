using UnityEngine;

/// <summary>
/// Estimate of perception based on one of the aspects of recognition
/// </summary>
public record PerceptionRecognitionEstimate
{
    /// <summary>
    /// Not limited. Typically values of the same order of magnitude
    /// </summary>
    public float Intensity { get; set; }

    /// <summary>
    /// Not limited. Typically values of different orders of magnitude.
    /// See https://en.wikipedia.org/wiki/Salience_(neuroscience).
    /// Salience scale (typical):
    /// <list type="bullet">
    /// <item>
    /// 0-1 - deep background (not paying attention)
    /// 1-10 - background (minimal attention)
    /// 10-100 - foreground (primary tasks, focus)
    /// 100-1000 - salient events (unexpected, relatively usual events)
    /// 1000-10 000 - shocking events (unexpected and extremely uncommon / shocking)
    /// </item>
    /// </list>
    /// </summary>
    public float Salience { get; set; }

    // /// <summary>
    // /// [0; Infinity]
    // /// </summary>
    // public float Repetition { get; set; }
}
