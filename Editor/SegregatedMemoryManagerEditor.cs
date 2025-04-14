using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(SegregatedMemoryManager))]
public class SegregatedMemoryManagerEditor : Editor
{
    private Vector2 scrollPosition;
    private bool showSources = true;
    private bool showDetails = false;

    private GUIStyle headerStyle;
    private GUIStyle boxStyle;
    private GUIStyle statsStyle;
    private GUIStyle miniBoldLabelStyle;

    private void InitializeStyles()
    {
        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            normal = { textColor = new Color(0.8f, 0.8f, 1f) }
        };

        boxStyle = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(10, 10, 5, 5),
            margin = new RectOffset(5, 5, 5, 5)
        };

        statsStyle = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            wordWrap = true
        };

        miniBoldLabelStyle = new GUIStyle(EditorStyles.miniBoldLabel);
    }

    public override void OnInspectorGUI()
    {
        InitializeStylesIfNeeded();
        DrawDefaultInspector();
        EditorGUILayout.Space(10);

        var manager = (SegregatedMemoryManager)target;
        if (manager == null)
        {
            return;
        }

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Perception statistics are only available in Play Mode", MessageType.Info);
            return;
        }

        DrawSourcesFoldout(manager);
    }

    private void InitializeStylesIfNeeded()
    {
        if (headerStyle == null)
        {
            InitializeStyles();
        }
    }

    private void DrawSourcesFoldout(SegregatedMemoryManager manager)
    {
        showSources = EditorGUILayout.Foldout(showSources, $"Perception Sources ({GetSourceCount(manager)})", true, headerStyle);

        if (!showSources)
        {
            return;
        }

        EditorGUI.indentLevel++;
        showDetails = EditorGUILayout.ToggleLeft("Show Detailed Stats", showDetails);
        EditorGUI.indentLevel--;

        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollView.scrollPosition;
            DrawAllSourcesStats(manager);
        }
    }

    private int GetSourceCount(SegregatedMemoryManager manager)
    {
        try
        {
            return manager.PerceptionSources?.Count ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    private void DrawAllSourcesStats(SegregatedMemoryManager manager)
    {
        IReadOnlyDictionary<string, IPerceptionSource> sources;
        try
        {
            sources = manager.PerceptionSources;
            if (sources == null || sources.Count == 0)
            {
                EditorGUILayout.HelpBox("No perception sources found", MessageType.Info);
                return;
            }
        }
        catch
        {
            EditorGUILayout.HelpBox("Failed to access perception sources", MessageType.Error);
            return;
        }

        foreach (var kvp in sources)
        {
            using (new EditorGUILayout.VerticalScope(boxStyle))
            {
                DrawSourceStats(kvp.Key, kvp.Value);
            }
        }
    }

    private void DrawSourceStats(string sourceKey, IPerceptionSource source)
    {
        EditorGUILayout.LabelField(sourceKey, EditorStyles.boldLabel);

        List<PerceptionEntry> perceptions;
        try
        {
            perceptions = source.GetPerceptions()?.ToList();
            if (perceptions == null || perceptions.Count == 0)
            {
                EditorGUILayout.LabelField("No perceptions available", statsStyle);
                return;
            }
        }
        catch
        {
            EditorGUILayout.LabelField("Error loading perceptions", statsStyle);
            return;
        }

        EditorGUILayout.LabelField($"Total Perceptions: {perceptions.Count}", statsStyle);

        var markerStats = CalculateMarkerStatistics(perceptions);
        EditorGUILayout.LabelField($"Unique Markers: {markerStats.Count}", statsStyle);

        if (!showDetails)
        {
            return;
        }

        EditorGUI.indentLevel++;
        try
        {
            DrawMarkerStatistics(markerStats);
            DrawAccessibilityStats(perceptions);
        }
        finally
        {
            EditorGUI.indentLevel--;
        }
    }

    private Dictionary<string, int> CalculateMarkerStatistics(List<PerceptionEntry> perceptions)
    {
        var markerStats = new Dictionary<string, int>();
        
        if (perceptions == null)
        {
            return markerStats;
        }

        foreach (var perception in perceptions)
        {
            if (perception?.Markers == null)
            {
                continue;
            }
            
            foreach (var marker in perception.Markers)
            {
                if (marker != null)
                {
                    markerStats[marker] = markerStats.TryGetValue(marker, out var count) ? count + 1 : 1;
                }
            }
        }
        
        return markerStats;
    }

    private void DrawMarkerStatistics(Dictionary<string, int> markerStats)
    {
        if (markerStats.Count == 0)
        {
            return;
        }

        EditorGUILayout.LabelField("Marker Statistics:", miniBoldLabelStyle);
        
        foreach (var kvp in markerStats.OrderByDescending(x => x.Value))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(kvp.Key, GUILayout.Width(150));
                EditorGUILayout.LabelField(kvp.Value.ToString(), statsStyle);
            }
        }
    }

    private void DrawAccessibilityStats(List<PerceptionEntry> perceptions)
    {
        if (perceptions == null || perceptions.Count == 0)
        {
            return;
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Accessibility Stats:", miniBoldLabelStyle);
        
        try
        {
            var avgAccessibility = perceptions.Average(p => p?.Accessibility ?? 0f);
            var minAccessibility = perceptions.Min(p => p?.Accessibility ?? 0f);
            var maxAccessibility = perceptions.Max(p => p?.Accessibility ?? 0f);
            
            EditorGUILayout.LabelField($"Average: {avgAccessibility:F2}", statsStyle);
            EditorGUILayout.LabelField($"Min: {minAccessibility:F2}", statsStyle);
            EditorGUILayout.LabelField($"Max: {maxAccessibility:F2}", statsStyle);
        }
        catch
        {
            EditorGUILayout.LabelField("Error calculating accessibility stats", statsStyle);
        }
    }
}