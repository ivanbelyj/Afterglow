using UnityEngine;

public interface IRecognizer<TRepresentation>
{
    PerceptionEntry Recognize(TRepresentation representation);
}
