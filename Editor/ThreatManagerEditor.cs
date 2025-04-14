using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[CustomEditor(typeof(ThreatManager))]
public class ThreatManagerEditor : Editor
{
    private Vector2 scrollPosition;
    private bool requireConstantRepaint = true;
    private bool isInPlayMode;

    private void OnEnable()
    {
        requireConstantRepaint = EditorPrefs.GetBool("ThreatManagerEditor_RequireConstantRepaint", true);
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        isInPlayMode = EditorApplication.isPlaying;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        isInPlayMode = state == PlayModeStateChange.EnteredPlayMode;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawConstantRepaintToggle();
        DrawThreatsSection();
    }

    private void DrawConstantRepaintToggle()
    {
        EditorGUI.BeginChangeCheck();
        requireConstantRepaint = EditorGUILayout.Toggle("Require Constant Repaint", requireConstantRepaint);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetBool("ThreatManagerEditor_RequireConstantRepaint", requireConstantRepaint);
        }
    }

    private void DrawThreatsSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Threats", EditorStyles.boldLabel);

        if (isInPlayMode)
        {
            HandlePlayModeRepaint();
            var threatManager = (ThreatManager)target;
            DisplayThreats(threatManager.GetThreatsInfo(), threatManager.GetCurrentTargeting());
        }
        else
        {
            EditorGUILayout.HelpBox("Threat information will be displayed during play mode", MessageType.Info);
        }
    }

    private void HandlePlayModeRepaint()
    {
        if (!requireConstantRepaint) return;
        Repaint();
    }

    private void DisplayThreats(List<ThreatInfo> threatsInfo, ThreatTargetingResult targetingResult)
    {
        if (threatsInfo == null)
        {
            EditorGUILayout.HelpBox("Threats list is null", MessageType.Warning);
            return;
        }

        if (threatsInfo.Count == 0)
        {
            EditorGUILayout.HelpBox("No active threats detected", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        DrawAllThreats(threatsInfo, targetingResult);
        EditorGUILayout.EndScrollView();
    }

    private void DrawAllThreats(List<ThreatInfo> threatsInfo, ThreatTargetingResult targetingResult)
    {
        var currentTarget = targetingResult?.TargetedThreat;
        Guid? currentTargetId = currentTarget?.ThreatEstimate?.EntityId;

        foreach (var threatInfo in threatsInfo)
        {
            if (threatInfo?.threatEstimate == null)
            {
                EditorGUILayout.HelpBox("Null threat detected in list", MessageType.Warning);
                continue;
            }

            DrawSingleThreat(threatInfo, currentTargetId, targetingResult);
            EditorGUILayout.Space(10);
        }
    }

    private void DrawSingleThreat(ThreatInfo threatInfo, Guid? currentTargetId, ThreatTargetingResult targetingResult)
    {
        bool isCurrentTarget = IsCurrentTarget(threatInfo, currentTargetId);
        bool isTargeted = IsTargeted(threatInfo, targetingResult);
        bool isIgnored = !isTargeted && !isCurrentTarget;

        DrawThreatBox(threatInfo, isCurrentTarget, isTargeted, isIgnored);
    }

    private bool IsCurrentTarget(ThreatInfo threatInfo, Guid? currentTargetId)
    {
        return currentTargetId.HasValue && 
               threatInfo.threatEstimate.EntityId == currentTargetId.Value;
    }

    private bool IsTargeted(ThreatInfo threatInfo, ThreatTargetingResult targetingResult)
    {
        return targetingResult?.OrderedTargetingEstimates != null && 
               targetingResult.OrderedTargetingEstimates.Any(t => 
                   t.ThreatEstimate.EntityId == threatInfo.threatEstimate.EntityId);
    }

    private void DrawThreatBox(ThreatInfo threatInfo, bool isCurrentTarget, bool isTargeted, bool isIgnored)
    {
        var boxStyle = CreateBoxStyle(isCurrentTarget, isTargeted, isIgnored);
        
        EditorGUILayout.BeginVertical(boxStyle);
        DrawThreatHeader(threatInfo, isCurrentTarget, isTargeted, isIgnored);
        DrawThreatDetails(threatInfo, isTargeted, isCurrentTarget);
        DrawThreatPotentials(threatInfo.threatEstimate.Potentials);
        EditorGUILayout.EndVertical();
    }

    private GUIStyle CreateBoxStyle(bool isCurrentTarget, bool isTargeted, bool isIgnored)
    {
        var style = new GUIStyle(EditorStyles.helpBox);

        if (isCurrentTarget)
            style.normal.background = MakeTex(2, 2, new Color(0.8f, 0.2f, 0.2f, 0.5f));
        else if (isIgnored)
            style.normal.background = MakeTex(2, 2, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        else if (isTargeted)
            style.normal.background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.8f, 0.3f));

        return style;
    }

    private void DrawThreatHeader(ThreatInfo threatInfo, bool isCurrentTarget, bool isTargeted, bool isIgnored)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Threat Detected", EditorStyles.boldLabel);

        if (isCurrentTarget)
            EditorGUILayout.LabelField("★ CURRENT TARGET ★", GetHighlightStyle(Color.red));
        else if (isIgnored)
            EditorGUILayout.LabelField("IGNORED", GetHighlightStyle(Color.gray));
        else if (isTargeted)
            EditorGUILayout.LabelField($"Utility: {threatInfo.targeting.Utility:F2}", 
                GetUtilityStyle(threatInfo.targeting.Utility));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);
    }

    private void DrawThreatDetails(ThreatInfo threatInfo, bool isTargeted, bool isCurrentTarget)
    {
        var threat = threatInfo.threatEstimate;
        
        DrawPropertyWithNullCheck("Type:", threat.ThreatType?.ToString() ?? "Null", 80, EditorStyles.boldLabel);
        DrawPropertyWithNullCheck("Distance:", threat.Distance.ToString(), 80, EditorStyles.boldLabel);
        DrawPropertyWithNullCheck("Threat ID:", threat.EntityId.ToString(), 80);
        DrawUtilityField(threatInfo, isTargeted, isCurrentTarget);
        DrawProbabilityField(threat);
    }

    private void DrawUtilityField(ThreatInfo threatInfo, bool isTargeted, bool isCurrentTarget)
    {
        if (isTargeted || isCurrentTarget)
        {
            var utility = isCurrentTarget ? threatInfo.targeting.Utility : threatInfo.targeting.Utility;
            DrawPropertyWithNullCheck("Utility:", $"{utility:F2}", 80, GetUtilityStyle(utility));
        }
        else
        {
            DrawPropertyWithNullCheck("Utility:", "N/A", 80, GetHighlightStyle(Color.gray));
        }
    }

    private void DrawProbabilityField(ThreatEstimate threat)
    {
        if (threat.Probability >= 0 && threat.Probability <= 1)
        {
            DrawPropertyWithNullCheck("Probability:", $"{threat.Probability:P0}", 80, GetProbabilityStyle(threat.Probability));
        }
        else
        {
            DrawPropertyWithNullCheck("Probability:", $"{threat.Probability:P0} (invalid value)", 80, GetErrorStyle());
        }
    }

    private void DrawThreatPotentials(ThreatPotentialEstimate[] potentials)
    {
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Threat Potentials:");

        if (potentials == null || potentials.Length == 0)
        {
            EditorGUILayout.HelpBox("No potentials data", MessageType.None);
            return;
        }

        EditorGUI.indentLevel++;
        foreach (var potential in potentials)
        {
            EditorGUILayout.LabelField($"• {potential}");
        }
        EditorGUI.indentLevel--;
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private GUIStyle GetHighlightStyle(Color color)
    {
        return new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = color },
            fontSize = 12
        };
    }

    private GUIStyle GetUtilityStyle(float utility)
    {
        var style = new GUIStyle(EditorStyles.label);
        
        if (utility > 1.5f) style.normal.textColor = Color.red;
        else if (utility > 1f) style.normal.textColor = new Color(1f, 0.5f, 0f);
        else if (utility > 0.5f) style.normal.textColor = Color.yellow;
        else style.normal.textColor = Color.green;
            
        return style;
    }

    private GUIStyle GetProbabilityStyle(float probability)
    {
        var style = new GUIStyle(EditorStyles.label);
        
        if (probability > 0.7f) style.normal.textColor = Color.red;
        else if (probability > 0.4f) style.normal.textColor = Color.yellow;
        else style.normal.textColor = Color.green;
            
        return style;
    }

    private GUIStyle GetErrorStyle()
    {
        return new GUIStyle(EditorStyles.label) { normal = { textColor = Color.red } };
    }

    private void DrawPropertyWithNullCheck(string label, string value, int labelWidth, GUIStyle valueStyle = null)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
        EditorGUILayout.LabelField(value ?? "Null", valueStyle ?? GetErrorStyle());
        EditorGUILayout.EndHorizontal();
    }
}