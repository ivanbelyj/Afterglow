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
    /// Not limited. Typically values of different orders of magnitude
    /// </summary>
    public float Salience { get; set; }

    // /// <summary>
    // /// [0; Infinity]
    // /// </summary>
    // public float Repetition { get; set; }
}
