using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThreatManager))]
public class ThreatDebugDrawHandler : MonoBehaviour, IThreatHandler
{
    [SerializeField]
    private bool drawGizmos = true;

    [SerializeField]
    private int gizmosFontSize = 24;

    private EntityProvider entityProvider;
    private ThreatManager threatManager;

    private ThreatProcessingResult currentProcessingResult;

    private void Awake()
    {
        entityProvider = GetComponent<EntityProvider>();
        threatManager = GetComponent<ThreatManager>();
    }

    public void Handle(ThreatProcessingResult processingResult)
    {
        currentProcessingResult = processingResult;
    }

    #region Draw Gizmos
    private void OnDrawGizmos()
    {
        if (!ShouldDrawGizmos()) 
        {
            return;
        }
        
        if (currentProcessingResult == null
            || currentProcessingResult.CurrentThreatEstimates.Count == 0) 
        {
            return;
        }

        DrawAllThreatGizmos();
    }

    private bool ShouldDrawGizmos()
    {
        return drawGizmos && Application.isPlaying;
    }

    private void DrawAllThreatGizmos()
    {
        var currentTargetId = GetCurrentTargetId();
        var position = GetEstimateOriginPosition();

        foreach (var threatInfo in threatManager.GetThreatsInfo())
        {
            if (threatInfo.threatEstimate == null
                // Some of the last estimated threats could be already destructured
                // between ticks (gizmos are drawn more often than simulation
                // updates estimations)
                || threatInfo.threatEstimate.ThreatPerception.IsDestructured) 
            {
                continue;
            }
            DrawSingleThreatGizmo(position, threatInfo, currentTargetId);
        }
    }

    private Vector3 GetEstimateOriginPosition()
        => threatManager
            .GetEstimateOriginPosition()
            .Position;

    private Guid? GetCurrentTargetId()
    {
        return currentProcessingResult.Targeting.TargetedThreat?.ThreatEstimate?.EntityId;
    }

    private void DrawSingleThreatGizmo(
        Vector3 position,
        ThreatInfo threatInfo,
        Guid? currentTargetId)
    {
        var threat = threatInfo.threatEstimate;
        var isCurrentTarget = IsCurrentTarget(threat, currentTargetId);
        var isTargeted = threatInfo.targeting != null;
        var threatPosition = GetThreatPosition(threat);

        if (threatPosition == null) 
        {
            return;
        }
        
        DrawThreatVisuals(position, threat, threatPosition.Value, isCurrentTarget, isTargeted);
    }

    private bool IsCurrentTarget(ThreatEstimate threat, Guid? currentTargetId)
    {
        return currentTargetId.HasValue && threat.EntityId == currentTargetId.Value;
    }

    private void DrawThreatVisuals(
        Vector3 origin,
        ThreatEstimate threat,
        Vector3 threatPosition, 
        bool isCurrentTarget,
        bool isTargeted)
    {
        DrawMainThreatLine(origin, threatPosition, threat.Probability, isCurrentTarget, isTargeted);
        DrawDistanceIndicator(origin, threatPosition, threat.Distance);
        DrawThreatTypeLabel(threatPosition, threat.ThreatType);
        DrawProbabilityLabel(threatPosition, threat.Probability);
        DrawEntityIdLabel(threatPosition, threat.EntityId);
        
        if (isCurrentTarget)
        {
            DrawCurrentTargetMarker(threatPosition);
        }
        
        DrawThreatPotentials(threatPosition, threat.Potentials);
    }

    private void DrawMainThreatLine(
        Vector3 from,
        Vector3 to,
        float probability, 
        bool isCurrentTarget,
        bool isTargeted)
    {
        var baseColor = GetThreatColor(probability);
        var lineAlpha = isCurrentTarget ? 0.8f : (isTargeted ? 0.6f : 0.3f);
        
        Gizmos.color = baseColor.WithAlpha(lineAlpha);
        Gizmos.DrawLine(from, to);
    }

    private void DrawDistanceIndicator(Vector3 from, Vector3 to, float distance)
    {
        var midPoint = Vector3.Lerp(from, to, 0.5f);
        DrawTextGizmo(midPoint, $"{distance:F1}m", Color.white, gizmosFontSize);
    }

    private void DrawThreatTypeLabel(Vector3 position, string threatType)
    {
        DrawTextGizmo(position + Vector3.up * 0.3f, threatType, Color.white, gizmosFontSize);
    }

    private void DrawProbabilityLabel(Vector3 position, float probability)
    {
        var probPos = position + Vector3.up * 0.6f;
        DrawTextGizmo(probPos, $"{probability:P0}", GetProbabilityColor(probability), gizmosFontSize);
    }

    private void DrawCurrentTargetMarker(Vector3 position)
    {
        Gizmos.color = Color.red.WithAlpha(0.7f);
        Gizmos.DrawLine(position + Vector3.left * 0.5f, position + Vector3.right * 0.5f);
        Gizmos.DrawLine(position + Vector3.up * 0.5f, position + Vector3.down * 0.5f);
    }

    private void DrawThreatPotentials(Vector3 position, ThreatPotentialEstimate[] potentials)
    {
        if (potentials == null || potentials.Length == 0) return;

        var primaryPotential = potentials[0];
        if (!primaryPotential.PotentialAvailableInSeconds.HasValue) return;

        var potPos = position + Vector3.up * 1.2f;
        var text = $"P: {primaryPotential.Potential:F1} in {primaryPotential.PotentialAvailableInSeconds:F1}s";
        DrawTextGizmo(potPos, text, GetPotentialColor(primaryPotential.Potential), gizmosFontSize);
    }

    private void DrawEntityIdLabel(Vector3 position, Guid entityId)
    {
        var shortId = GetShortenedEntityId(entityId);
        var idPos = position + Vector3.up * 0.9f;
        DrawTextGizmo(idPos, $"ID: {shortId}", Color.cyan, gizmosFontSize);
    }

    private string GetShortenedEntityId(Guid entityId)
    {
        var fullId = entityId.ToString();
        return fullId.Length > 8 ? fullId.Substring(0, 8) + "..." : fullId;
    }

    private Vector3? GetThreatPosition(ThreatEstimate threatEstimate)
    {
        return threatEstimate.ThreatPerception.Position.Position;
    }

    private void DrawTextGizmo(Vector3 position, string text, Color color, int fontSize)
    {
    #if UNITY_EDITOR
        var style = new GUIStyle
        {
            normal = { textColor = color },
            fontSize = fontSize
        };
        UnityEditor.Handles.Label(position, text, style);
    #endif
    }

    private Color GetThreatColor(float probability)
    {
        return probability > 0.7f ? Color.red :
            probability > 0.4f ? Color.yellow :
            Color.green;
    }

    private Color GetProbabilityColor(float probability)
    {
        return probability > 0.7f ? Color.Lerp(Color.red, Color.white, 0.7f) :
            probability > 0.4f ? Color.Lerp(Color.yellow, Color.white, 0.5f) :
            Color.Lerp(Color.green, Color.white, 0.3f);
    }

    private Color GetUtilityColor(float utility)
    {
        return utility > 1.5f ? Color.red :
            utility > 1f ? new Color(1f, 0.5f, 0f) :
            utility > 0.5f ? Color.yellow :
            Color.green;
    }

    private Color GetPotentialColor(float potential)
    {
        return potential > 0.7f ? Color.red :
            potential > 0.4f ? Color.yellow :
            Color.green;
    }
    #endregion
}
