using UnityEngine;

[RequireComponent(typeof(PerceptionRecognizerCore))]
public abstract class RecognitionHandlerBase : MonoBehaviour, IRecognitionHandler
{
    public abstract PerceptionRecognitionEstimate HandleRecognition(PerceptionEntry perceptionEntry);

    protected virtual void Awake()
    {
        GetComponent<PerceptionRecognizerCore>().RegisterHandler(this);
    }
}
