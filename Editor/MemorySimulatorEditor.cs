using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MemorySimulator))]
public class MemorySimulatorEditor : Editor
{
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

        var simulator = (MemorySimulator)target;
        if (simulator == null) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug View", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(
            scrollPosition, 
            GUILayout.Height(200));

        simulator.debugOutput = EditorGUILayout.TextArea(
            simulator.debugOutput,
            monoSpacedTextAreaStyle,
            GUILayout.ExpandHeight(true));

        EditorGUILayout.EndScrollView();
    }
}
