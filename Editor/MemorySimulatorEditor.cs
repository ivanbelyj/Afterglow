using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MemorySimulator))]
public class MemorySimulatorEditor : Editor
{
    private string verbalRepresentation = "New Memory";
    private float retentionIntensity = 0.1f;
    private float accessibility = 1.0f;

    private GUIStyle monoSpacedTextAreaStyle;

    private Vector2 scrollPosition;

    private void OnEnable()
    {
        monoSpacedTextAreaStyle = new GUIStyle(EditorStyles.textArea)
        {
            font = Font.CreateDynamicFontFromOSFont("Consolas", 12)
        };
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Add custom UI for adding memories
        GUILayout.Space(10);
        GUILayout.Label("Add New Memory", EditorStyles.boldLabel);

        verbalRepresentation = EditorGUILayout.TextField("Verbal Representation", verbalRepresentation);
        retentionIntensity = EditorGUILayout.FloatField("Retention Intensity", retentionIntensity);
        accessibility = EditorGUILayout.FloatField("Accessibility", accessibility);

        var simulator = (MemorySimulator)target;

        if (simulator != null)
        {
            // Todo: implement adding perceptions through the editor
            // if (GUILayout.Button("Add Memory"))
            // {
            //     simulator.AddMemory(verbalRepresentation, retentionIntensity, accessibility, null);
            // }

            GUILayout.Space(10);
            GUILayout.Label("Debug Output", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            simulator.debugOutput = EditorGUILayout.TextArea(
                simulator.debugOutput,
                monoSpacedTextAreaStyle,
                GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();
        }
    }
}
