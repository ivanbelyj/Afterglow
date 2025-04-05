using UnityEngine;

public interface IRecognitionHandler
{
    PerceptionRecognitionEstimate HandleRecognition(PerceptionEntry perceptionEntry);
}
