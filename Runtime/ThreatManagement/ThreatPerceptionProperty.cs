using System;

[Serializable]
public struct BoundCompoundProperty
{
    public float MinEstimate { get; set; }
    public float MaxEstimate { get; set; }

    /// <summary>
    /// Adjusts estimation between min and max. [0; 1]
    /// </summary>
    public float Optimism { get; set; }

    public BoundCompoundProperty(float minEstimated, float maxEstimated, float optimism = 0.5f)
    {
        MinEstimate = minEstimated;
        MaxEstimate = maxEstimated;
        Optimism = optimism;   
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
            MinEstimate = (left.MinEstimate + right.MinEstimate) / 2,
            MaxEstimate = (left.MaxEstimate + right.MaxEstimate) / 2,
            Optimism = GetWeightedOptimism(left, right)
        };
    }

    private static float GetWeightedOptimism(
        in BoundCompoundProperty c1,
        in BoundCompoundProperty c2)
    {
        float weight1 = c1.MaxEstimate - c1.MinEstimate;
        float weight2 = c2.MaxEstimate - c2.MinEstimate;
        return (c1.Optimism * weight1 + c2.Optimism * weight2) / (weight1 + weight2);
    }
}
