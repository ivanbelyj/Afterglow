using UnityEngine;

// Персонаж запоминает важные точки на карте
// (либо он может их точно знать, например, если имеет доступ к информационной системе).

// Помнит ли путь персонаж между точками? Скорее всего,
// переход от одной точки к другой представляется отдельным воспоминанием
// (вероятно, оно более сложное, поэтому быстрее забывается)

// Если персонаж помнит переход - переход симулируется с применением
// системы навигации.
// Если не помнит - пробует идти в направлении точки с помощью эвристики.
// Эвристика может привести в тупик. Вероятно, в таком случае нужен алгоритм исследования неизвестной территории.

/// <summary>
/// Provides navigation based on complex perception sources (memory,
/// perception and intuition)
/// </summary>
[RequireComponent(typeof(ISpacialAwarenessProvider))]
public class PerceptionBasedNavigator : MonoBehaviour
{
    private ISpacialAwarenessProvider spacialAwarenessProvider;

    private void Awake()
    {
        spacialAwarenessProvider = GetComponent<ISpacialAwarenessProvider>();
        if (spacialAwarenessProvider == null)
        {
            Debug.LogError($"{nameof(ISpacialAwarenessProvider)} is required for {nameof(PerceptionBasedNavigator)}");
        }
    }

    public PerceptionBasedNavigator(ISpacialAwarenessProvider spacialAwarenessProvider)
    {
        this.spacialAwarenessProvider = spacialAwarenessProvider;
    }

    public (PerceptionBasedNavigationType, Vector3) GetPositionToNavigate()
    {
        // TODO:
        return default;
    }
}
