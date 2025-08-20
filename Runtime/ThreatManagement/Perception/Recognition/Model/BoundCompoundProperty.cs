using System;
using UnityEngine;

[Serializable]
public struct BoundCompoundProperty
{
    [SerializeField]
    private float minEstimate;
    [SerializeField]
    private float maxEstimate;

    [Range(0, 1f)]
    [SerializeField]
    private float optimism;

    public float MinEstimate
    {
        get => minEstimate;
        private set => minEstimate = value;
    }
    public float MaxEstimate
    {
        get => maxEstimate; private set => maxEstimate = value;
    }

    /// <summary>
    /// Adjusts estimation between min and max. [0; 1]
    /// </summary>
    public float Optimism
    {
        get => optimism; private set => optimism = value;
    }

    public BoundCompoundProperty(float minEstimate, float maxEstimate, float optimism = 0.5f)
    {
        if (minEstimate > maxEstimate)
        {
            throw new ArgumentException("MinEstimate cannot be greater than MaxEstimate.");
        }

        if (optimism < 0f || optimism > 1f)
        {
            throw new ArgumentOutOfRangeException(nameof(optimism), "Optimism must be in [0; 1].");
        }

        this.minEstimate = minEstimate;
        this.maxEstimate = maxEstimate;
        this.optimism = optimism;
    }

    public float GetValue() => MinEstimate + (MaxEstimate - MinEstimate) * Optimism;

    public static explicit operator float(BoundCompoundProperty property)
    {
        return property.GetValue();
    }

    public static BoundCompoundProperty operator +(
        BoundCompoundProperty left,
        BoundCompoundProperty right)
    {
        return new BoundCompoundProperty()
        {
            MinEstimate = left.MinEstimate + right.MinEstimate,
            MaxEstimate = left.MaxEstimate + right.MaxEstimate,
            Optimism = GetWeightedOptimism(left, right),
        };
    }

    public static BoundCompoundProperty operator *(
        BoundCompoundProperty left,
        float right)
    {
        return new BoundCompoundProperty
        {
            MinEstimate = left.MinEstimate * right,
            MaxEstimate = left.MaxEstimate * right,
            Optimism = left.Optimism
        };
    }

    public static BoundCompoundProperty operator *(float left, BoundCompoundProperty right)
    {
        return right * left;
    }

    public static BoundCompoundProperty operator *(
        BoundCompoundProperty left,
        BoundCompoundProperty right)
    {
        return new BoundCompoundProperty
        {
            MinEstimate = left.MinEstimate * right.MinEstimate,
            MaxEstimate = left.MaxEstimate * right.MaxEstimate,
            Optimism = GetWeightedOptimism(left, right)
        };
    }

    private static float GetWeightedOptimism(
        in BoundCompoundProperty c1,
        in BoundCompoundProperty c2)
    {
        float weight1 = c1.MaxEstimate - c1.MinEstimate;
        float weight2 = c2.MaxEstimate - c2.MinEstimate;

        if (Mathf.Approximately(weight1 + weight2, 0f))
        {
            return (c1.Optimism + c2.Optimism) * 0.5f;
        }

        return (c1.Optimism * weight1 + c2.Optimism * weight2) / (weight1 + weight2);
    }

    public override string ToString()
    {
        return $"[{MinEstimate:F2}, {MaxEstimate:F2}] : {Optimism:P0} → {GetValue():F2}";
    }
}
