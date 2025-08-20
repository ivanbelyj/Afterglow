using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

[CustomEditor(typeof(PerceptionTrackingStorage))]
public class PerceptionTrackingStorageEditor : Editor
{
    private Vector2 scrollPosition;
    private bool showPerceptions = true;
    private bool showMarkers = true;
    private bool showPerceptionData = true;

    private GUIStyle foldoutStyle;
    private GUIStyle perceptionStyle;
    private GUIStyle markerStyle;
    private GUIStyle wrapTextStyle;
    private GUIStyle entityIdStyle;

    private void InitializeStyles()
    {
        foldoutStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold
        };

        perceptionStyle = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(10, 10, 5, 5),
            margin = new RectOffset(5, 5, 5, 5)
        };

        markerStyle = new GUIStyle(EditorStyles.miniButton)
        {
            fixedHeight = 20,
            margin = new RectOffset(2, 2, 2, 2),
            wordWrap = true
        };

        wrapTextStyle = new GUIStyle(EditorStyles.label)
        {
            wordWrap = true,
            richText = true
        };

        entityIdStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = new Color(0.2f, 0.8f, 0.8f) },
            fontStyle = FontStyle.Italic
        };
    }

    public override void OnInspectorGUI()
    {
        InitializeStylesIfNeeded();
        DrawDefaultInspector();
        EditorGUILayout.Space(10);

        if (!ShouldDrawEditorContent())
        {
            return;
        }

        var storage = GetTargetStorage();
        if (storage == null)
        {
            return;
        }

        var perceptions = GetSortedPerceptions(storage);
        if (perceptions.Count == 0)
        {
            DrawEmptyPerceptionsMessage();
            return;
        }

        DrawPerceptionsView(perceptions);
    }

    private void InitializeStylesIfNeeded()
    {
        if (foldoutStyle == null)
        {
            InitializeStyles();
        }
    }

    private bool ShouldDrawEditorContent()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Perception data is only available in Play Mode", MessageType.Info);
            return false;
        }
        return true;
    }

    private PerceptionTrackingStorage GetTargetStorage()
    {
        var storage = (PerceptionTrackingStorage)target;
        if (storage == null || storage.AllPerceptions == null)
        {
            return null;
        }
        return storage;
    }

    private List<PerceptionEntry> GetSortedPerceptions(PerceptionTrackingStorage storage)
    {
        return storage.AllPerceptions.Collection
            .OrderByDescending(p => p.TimestampTo)
            .ToList();
    }

    private void DrawEmptyPerceptionsMessage()
    {
        EditorGUILayout.HelpBox("No perceptions tracked yet", MessageType.Info);
    }

    private void DrawPerceptionsView(List<PerceptionEntry> perceptions)
    {
        DrawMainFoldouts();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (showPerceptions)
        {
            DrawPerceptionsList(perceptions);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawMainFoldouts()
    {
        showPerceptions = EditorGUILayout.Foldout(showPerceptions,
            $"Perceptions ({GetPerceptionCount()})", true, foldoutStyle);

        if (showPerceptions)
        {
            EditorGUI.indentLevel++;
            DrawVisibilityToggles();
            EditorGUI.indentLevel--;
        }
    }

    private int GetPerceptionCount()
    {
        var storage = (PerceptionTrackingStorage)target;
        return storage?.AllPerceptions?.Collection.Count ?? 0;
    }

    private void DrawVisibilityToggles()
    {
        showMarkers = EditorGUILayout.ToggleLeft("Show Markers", showMarkers);
        showPerceptionData = EditorGUILayout.ToggleLeft("Show Perception Data", showPerceptionData);
    }

    private void DrawPerceptionsList(List<PerceptionEntry> perceptions)
    {
        foreach (var perception in perceptions)
        {
            DrawPerceptionCard(perception);
        }
    }

    private void DrawPerceptionCard(PerceptionEntry perception)
    {
        EditorGUILayout.BeginVertical(perceptionStyle);
        DrawPerceptionHeader(perception);
        DrawEntityIdIfExists(perception);
        DrawOptionalSections(perception);
        EditorGUILayout.EndVertical();
    }

    private void DrawEntityIdIfExists(PerceptionEntry perception)
    {
        if (perception.TryGetEntityId(out var entityId))
        {
            EditorGUILayout.LabelField($"Entity ID: {FormatEntityId(entityId)}", entityIdStyle);
        }
    }

    private string FormatEntityId(Guid entityId)
    {
        var idString = entityId.ToString();
        return idString.Length > 12 ?
            $"{idString.Substring(0, 8)}...{idString.Substring(idString.Length - 4)}" :
            idString;
    }

    private void DrawOptionalSections(PerceptionEntry perception)
    {
        if (showMarkers && perception.Markers.Count > 0)
        {
            DrawMarkersSection(perception.Markers);
        }

        if (showPerceptionData && perception.PerceptionData.Count > 0)
        {
            DrawPerceptionDataSection(perception.PerceptionData);
        }
    }

    string GetTimestampDisplayString(PerceptionEntry perception)
    {
        bool hasFrom = perception.TimestampFrom.HasValue;
        bool hasTo = perception.TimestampTo.HasValue;

        if (!hasFrom && !hasTo)
            return "No timestamp";

        if (hasFrom && hasTo)
            return $"{perception.TimestampFrom.Value:F2} - {perception.TimestampTo.Value:F2}";

        if (hasFrom)
            return $"From: {perception.TimestampFrom.Value:F2}";

        return $"To: {perception.TimestampTo.Value:F2}";
    }

    private void DrawPerceptionHeader(PerceptionEntry perception)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(perception.VerbalRepresentation, EditorStyles.boldLabel);
        EditorGUILayout.LabelField(GetTimestampDisplayString(perception), GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Accessibility: {perception.Accessibility:F2}", GUILayout.Width(150));
        EditorGUILayout.LabelField($"Retention: {perception.RetentionIntensity?.ToString("F2") ?? "N/A"}");
        EditorGUILayout.EndHorizontal();
    }

    private void DrawMarkersSection(HashSet<string> markers)
    {
        EditorGUILayout.LabelField("Markers:", EditorStyles.miniBoldLabel);
        EditorGUILayout.BeginVertical();

        foreach (var marker in markers)
        {
            EditorGUILayout.LabelField(marker, markerStyle, GUILayout.ExpandWidth(true));
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawPerceptionDataSection(Dictionary<string, object> data)
    {
        EditorGUILayout.LabelField("Perception Data:", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;

        foreach (var kvp in data)
        {
            DrawPerceptionDataRow(kvp.Key, kvp.Value);
        }

        EditorGUI.indentLevel--;
    }

    private void DrawPerceptionDataRow(string key, object value)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(key, GUILayout.Width(150));

        if (value is IEnumerable enumerable && !(value is string))
        {
            EditorGUILayout.BeginVertical();
            int index = 0;
            foreach (var item in enumerable)
            {
                EditorGUILayout.LabelField($"  [{index++}]: {item?.ToString() ?? "null"}", wrapTextStyle);
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.LabelField(value?.ToString() ?? "null", wrapTextStyle);
        }

        EditorGUILayout.EndHorizontal();
    }
}