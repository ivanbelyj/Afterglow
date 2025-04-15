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
    private bool showThreatKnowledgeGlobal = false;
    private bool isInPlayMode;
    private Dictionary<Guid, bool> threatKnowledgeFoldoutStates = new Dictionary<Guid, bool>();

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

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(showThreatKnowledgeGlobal ? "Hide All Knowledge" : "Show All Knowledge", 
            GUILayout.Width(150)))
        {
            showThreatKnowledgeGlobal = !showThreatKnowledgeGlobal;
            foreach (var key in threatKnowledgeFoldoutStates.Keys.ToList())
            {
                threatKnowledgeFoldoutStates[key] = showThreatKnowledgeGlobal;
            }
        }
        EditorGUILayout.EndHorizontal();

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
        bool isTargeted = threatInfo.targeting != null;
        bool isIgnored = threatInfo.targeting == null;

        DrawThreatBox(threatInfo, isCurrentTarget, isTargeted, isIgnored);
    }

    private bool IsCurrentTarget(ThreatInfo threatInfo, Guid? currentTargetId)
    {
        return currentTargetId != null &&
               threatInfo.threatEstimate.EntityId == currentTargetId.Value;
    }

    private void DrawThreatBox(ThreatInfo threatInfo, bool isCurrentTarget, bool isTargeted, bool isIgnored)
    {
        var boxStyle = CreateBoxStyle(isCurrentTarget, isTargeted, isIgnored);
        
        EditorGUILayout.BeginVertical(boxStyle);
        
        DrawThreatHeader(threatInfo, isCurrentTarget, isTargeted, isIgnored);
        DrawThreatDetails(threatInfo, isTargeted, isCurrentTarget);
        DrawThreatPotentials(threatInfo.threatEstimate.Potentials);

        DrawThreatKnowledgeSection(threatInfo);

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
        DrawPropertyWithNullCheck("Threat ID:", threat.EntityId.ToString(), 80, GetThreatIdStyle());
        DrawUtilityField(threatInfo, isTargeted, isCurrentTarget);
        DrawProbabilityField(threat);
    }

    private GUIStyle GetThreatIdStyle()
    {
        return new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = Color.Lerp(Color.cyan, Color.white, 0.3f) }
        };
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
        if (potentials == null || potentials.Length == 0)
        {
            EditorGUILayout.HelpBox("No potentials data", MessageType.None);
            return;
        }

        EditorGUILayout.LabelField("Threat Potentials:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        
        foreach (var potential in potentials)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("•", GUILayout.Width(15));
            EditorGUILayout.LabelField(potential.ToString(), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
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
        
        if (utility > 1.5f)
        {
            style.normal.textColor = Color.red;
            style.fontStyle = FontStyle.Bold;
        }
        else if (utility > 1f)
        {
            style.normal.textColor = Color.red;
        }
        else if (utility > 0.5f)
        {
            style.normal.textColor = new Color(1f, 0.5f, 0f);
        }
        else
        {
            style.normal.textColor = Color.yellow;
        }
            
        return style;
    }

    private GUIStyle GetProbabilityStyle(float probability)
    {
        var style = new GUIStyle(EditorStyles.label);
        style.normal.textColor = Color.Lerp(new Color(0.2f, 0.8f, 1f), new Color(1f, 0.6f, 0f), probability);
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

    private void DrawThreatKnowledgeSection(ThreatInfo threatInfo)
    {
        if (threatInfo.threatEstimate?.ThreatPerception?.ThreatKnowledge == null)
            return;

        var threatId = threatInfo.threatEstimate.EntityId;
        if (!threatKnowledgeFoldoutStates.ContainsKey(threatId))
        {
            threatKnowledgeFoldoutStates[threatId] = false;
        }

        var knowledge = threatInfo.threatEstimate.ThreatPerception.ThreatKnowledge;
        var compoundData = knowledge.GetCompoundData();

        bool showSection = showThreatKnowledgeGlobal || threatKnowledgeFoldoutStates[threatId];
        
        EditorGUI.BeginChangeCheck();
        showSection = EditorGUILayout.Foldout(showSection, "Threat Knowledge", true, EditorStyles.foldoutHeader);
        if (EditorGUI.EndChangeCheck())
        {
            threatKnowledgeFoldoutStates[threatId] = showSection;
        }

        if (!showSection)
            return;

        EditorGUI.indentLevel++;
        
        var textStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 11,
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0)
        };

        EditorGUILayout.Space(3);
        EditorGUILayout.LabelField("Compound Assessment:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Movement Speed:", textStyle, GUILayout.Width(120));
        EditorGUILayout.LabelField(compoundData.MovementSpeed.ToString(), GetOptimismStyle(compoundData.MovementSpeed.Optimism), GUILayout.Width(180));
        EditorGUILayout.EndHorizontal();

        DrawProbabilityFactors(compoundData);

        if (compoundData.Potentials != null && compoundData.Potentials.Count > 0)
        {
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Threat Potentials:", EditorStyles.boldLabel);
            
            foreach (var potential in compoundData.Potentials)
            {
                EditorGUILayout.LabelField(
                    $"{potential.PotentialName}: " +
                    $"P={potential.Potential:F2}, " +
                    $"R={potential.Radius:F1}m, " +
                    $"T={potential.ActivationTimeSeconds:F1}s",
                    textStyle);
            }
        }

        if (knowledge.factors != null && knowledge.factors.Count > 0)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Component Factors:", EditorStyles.boldLabel);
            
            for (int i = 0; i < knowledge.factors.Count; i++)
            {
                DrawFactor(knowledge.factors[i], textStyle);
            }
        }
        
        EditorGUI.indentLevel--;
    }

    private void DrawProbabilityFactors(ThreatPerceptionCompoundData data)
    {
        EditorGUILayout.LabelField("Probability Factors:", EditorStyles.boldLabel);
        
        // Create a compact grid layout for probability factors
        int factorsPerRow = 2;
        int factorCount = 4; // Presence, Suspicion, Awareness, Focus
        int rows = Mathf.CeilToInt(factorCount / (float)factorsPerRow);

        for (int i = 0; i < rows; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            for (int j = 0; j < factorsPerRow; j++)
            {
                int index = i * factorsPerRow + j;
                if (index >= factorCount) break;

                string label = "";
                float value = 0f;
                
                switch (index)
                {
                    case 0:
                        label = "Presence";
                        value = data.ThreatPresence.GetValue();
                        break;
                    case 1:
                        label = "Suspicion";
                        value = data.PerceptorSuspicion.GetValue();
                        break;
                    case 2:
                        label = "Awareness";
                        value = data.ThreatAwareness.GetValue();
                        break;
                    case 3:
                        label = "Focus";
                        value = data.ThreatFocus.GetValue();
                        break;
                }

                EditorGUILayout.BeginHorizontal(GUILayout.Width(EditorGUIUtility.currentViewWidth / factorsPerRow - 10));
                EditorGUILayout.LabelField($"{label}:", GUILayout.Width(70));
                EditorGUILayout.LabelField($"{value:F2}", GetProbabilityStyle(value), GUILayout.Width(40));
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawFactor(ThreatPerceptionCompoundData factor, GUIStyle textStyle)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Movement Speed:", textStyle, GUILayout.Width(120));
        EditorGUILayout.LabelField(factor.MovementSpeed.ToString(), GetOptimismStyle(factor.MovementSpeed.Optimism), GUILayout.Width(180));
        EditorGUILayout.EndHorizontal();

        DrawProbabilityFactors(factor);

        if (factor.Potentials != null && factor.Potentials.Count > 0)
        {
            EditorGUILayout.LabelField("Potentials:", EditorStyles.boldLabel);
            foreach (var p in factor.Potentials)
            {
                EditorGUILayout.LabelField($"- {p.PotentialName}: {p.Potential:F2}", textStyle);
            }
        }
        
        EditorGUILayout.EndVertical();
    }

    private GUIStyle GetOptimismStyle(float optimism)
    {
        var style = new GUIStyle(EditorStyles.label)
        {
            fontSize = 11,
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0)
        };

        if (optimism > 0.5f)
            style.normal.textColor = Color.Lerp(Color.yellow, new Color(1f, 0.647f, 0f), (optimism - 0.5f) / 0.5f);
        else
            style.normal.textColor = Color.Lerp(Color.cyan, Color.yellow, optimism / 0.5f);

        return style;
    }
}