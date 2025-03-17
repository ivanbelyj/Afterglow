using UnityEngine;

/// <summary>
/// Object, that can be percepted visually
/// </summary>
public class Sight : MonoBehaviour
{
    [TextArea]
    [SerializeField]
    private string verbalRepresentation;

    public string VerbalRepresentation {
        get => verbalRepresentation;
        set => verbalRepresentation = value;
    }
}
