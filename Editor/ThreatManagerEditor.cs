using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(ThreatManager))]
public class ThreatManagerEditor : Editor
{
    private Vector2 scrollPosition;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var threatManager = (ThreatManager)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Threats", EditorStyles.boldLabel);
        
        if (Application.isPlaying)
        {
            var threats = threatManager.GetCurrentThreatEstimates();
            DisplayThreats(threats);
        }
        else
        {
            EditorGUILayout.HelpBox("Threat information will be displayed during play mode", MessageType.Info);
        }
    }

    private void DisplayThreats(List<ThreatEstimate> threats)
    {
        if (threats == null)
        {
            EditorGUILayout.HelpBox("Threats list is null", MessageType.Warning);
            return;
        }

        if (threats.Count == 0)
        {
            EditorGUILayout.HelpBox("No active threats detected", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        foreach (var threat in threats)
        {
            if (threat != null)
            {
                DrawThreatBox(threat);
                EditorGUILayout.Space(10);
            }
            else
            {
                EditorGUILayout.HelpBox("Null threat detected in list", MessageType.Warning);
            }
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawThreatBox(ThreatEstimate threat)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Header
        EditorGUILayout.LabelField("Threat Detected", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        
        // Basic Info
        DrawPropertyWithNullCheck("Type:", threat.ThreatType?.ToString() ?? "Null", 80, EditorStyles.boldLabel);
        DrawPropertyWithNullCheck("Threat ID:", threat.EntityId.ToString(), 80);
        
        if (threat.Probability >= 0 && threat.Probability <= 1)
        {
            DrawPropertyWithNullCheck("Probability:", $"{threat.Probability:P0}", 80, GetProbabilityStyle(threat.Probability));
        }
        else
        {
            DrawPropertyWithNullCheck("Probability:", "Invalid value", 80, GetErrorStyle());
        }
        
        // Potentials Section
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Threat Potentials:");
        
        if (threat.Potentials != null && threat.Potentials.Length > 0)
        {
            EditorGUI.indentLevel++;
            foreach (var potential in threat.Potentials)
            {
                EditorGUILayout.LabelField($"• {potential}");
            }
            EditorGUI.indentLevel--;
        }
        else
        {
            EditorGUILayout.HelpBox("No potentials data", MessageType.None);
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawPropertyWithNullCheck(string label, string value, int labelWidth, GUIStyle valueStyle = null)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
        
        if (value == null)
        {
            EditorGUILayout.LabelField("Null", GetErrorStyle());
        }
        else
        {
            if (valueStyle != null)
            {
                EditorGUILayout.LabelField(value, valueStyle);
            }
            else
            {
                EditorGUILayout.LabelField(value);
            }
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private GUIStyle GetProbabilityStyle(float probability)
    {
        var style = new GUIStyle(EditorStyles.label);
        
        if (probability > 0.7f)
            style.normal.textColor = Color.red;
        else if (probability > 0.4f)
            style.normal.textColor = Color.yellow;
        else
            style.normal.textColor = Color.green;
            
        return style;
    }

    private GUIStyle GetErrorStyle()
    {
        var style = new GUIStyle(EditorStyles.label);
        style.normal.textColor = Color.red;
        return style;
    }
}