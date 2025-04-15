using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(Sight), true)]
[CanEditMultipleObjects]
public class SightEditor : Editor
{
    private SerializedProperty entityTypeProperty;
    private SerializedProperty verbalRepresentationProperty;
    private SerializedProperty spatialRadiusProperty;
    
    private Sight sightTarget;
    private bool showDataFoldout = true;
    private Vector2 scrollPosition;
    private List<string> keysToRemove = new List<string>();
    private bool hasNullValues;
    private GUIStyle wrapTextStyle;
    private bool requireConstantRepaint = true;

    private void OnEnable()
    {
        entityTypeProperty = serializedObject.FindProperty("entityType");
        verbalRepresentationProperty = serializedObject.FindProperty("verbalRepresentation");
        spatialRadiusProperty = serializedObject.FindProperty("spatialRadius");
        sightTarget = (Sight)target;
        
        wrapTextStyle = new GUIStyle(EditorStyles.label) { wordWrap = true };
    }

    public override bool RequiresConstantRepaint()
    {
        return requireConstantRepaint;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawSettings();
        DrawDefaultProperties();
        DrawSightData();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSettings()
    {
        EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
        requireConstantRepaint = EditorGUILayout.Toggle("Require Constant Repaint", requireConstantRepaint);
        EditorGUILayout.Space();
    }

    private void DrawDefaultProperties()
    {
        EditorGUILayout.PropertyField(entityTypeProperty);
        EditorGUILayout.PropertyField(verbalRepresentationProperty);
        EditorGUILayout.PropertyField(spatialRadiusProperty);
    }

    private void DrawSightData()
    {
        showDataFoldout = EditorGUILayout.Foldout(showDataFoldout, "Sight Data", true);
        if (!showDataFoldout) return;
        
        EditorGUI.indentLevel++;
        if (IsDataEmpty()) return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(200));
        DrawDataEntries();
        EditorGUILayout.EndScrollView();

        DrawWarnings();
        DrawManagementButtons();
        EditorGUI.indentLevel--;
    }

    private bool IsDataEmpty()
    {
        if (sightTarget.SightData == null || sightTarget.SightData.Count == 0)
        {
            EditorGUILayout.HelpBox("No Sight Data available", MessageType.Info);
            return true;
        }
        return false;
    }

    private void DrawDataEntries()
    {
        keysToRemove.Clear();
        hasNullValues = false;
        var keys = sightTarget.SightData.Keys.ToList();

        foreach (var key in keys)
        {
            if (!sightTarget.SightData.TryGetValue(key, out object value))
                continue;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawDataRow(key, value);
            EditorGUILayout.EndVertical();
        }

        RemoveMarkedKeys();
    }

    private void DrawDataRow(string key, object value)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(key, GUILayout.Width(150));
        DrawValueField(key, value);
        DrawRemoveButton(key);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawValueField(string key, object value)
    {
        if (value == null)
        {
            EditorGUILayout.LabelField("NULL");
            hasNullValues = true;
            return;
        }

        if (value is IEnumerable enumerable && !(value is string))
        {
            DrawEnumerableValue(enumerable);
        }
        else
        {
            DrawSingleValue(key, value);
        }
    }

    private void DrawEnumerableValue(IEnumerable enumerable)
    {
        EditorGUILayout.BeginVertical();
        int index = 0;
        foreach (var item in enumerable)
        {
            EditorGUILayout.LabelField($"  [{index++}]: {item?.ToString() ?? "null"}", wrapTextStyle);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawSingleValue(string key, object value)
    {
        try
        {
            switch (value)
            {
                case string s:
                    sightTarget.SightData[key] = EditorGUILayout.TextField(s);
                    break;
                case int i:
                    sightTarget.SightData[key] = EditorGUILayout.IntField(i);
                    break;
                case float f:
                    sightTarget.SightData[key] = EditorGUILayout.FloatField(f);
                    break;
                case bool b:
                    sightTarget.SightData[key] = EditorGUILayout.Toggle(b);
                    break;
                case Vector2 v2:
                    sightTarget.SightData[key] = EditorGUILayout.Vector2Field("", v2);
                    break;
                case Vector3 v3:
                    sightTarget.SightData[key] = EditorGUILayout.Vector3Field("", v3);
                    break;
                case System.Enum e:
                    sightTarget.SightData[key] = EditorGUILayout.EnumPopup(e);
                    break;
                default:
                    EditorGUILayout.LabelField(value.ToString(), wrapTextStyle);
                    break;
            }
        }
        catch (System.Exception e)
        {
            EditorGUILayout.LabelField($"Error: {e.Message}");
        }
    }

    private void DrawRemoveButton(string key)
    {
        if (GUILayout.Button("×", GUILayout.Width(20)))
            keysToRemove.Add(key);
    }

    private void RemoveMarkedKeys()
    {
        foreach (var key in keysToRemove)
            sightTarget.SightData.Remove(key);
    }

    private void DrawWarnings()
    {
        if (hasNullValues)
            EditorGUILayout.HelpBox("Some values are NULL. Consider removing or fixing them.", MessageType.Warning);
    }

    private void DrawManagementButtons()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Data"))
            sightTarget.SightData["NewKey"] = "NewValue";
        
        if (GUILayout.Button("Clear All") && EditorUtility.DisplayDialog(
            "Clear Sight Data", 
            "Are you sure you want to clear all Sight Data?", 
            "Yes", "No"))
        {
            sightTarget.SightData.Clear();
        }
        EditorGUILayout.EndHorizontal();
    }
}