// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class ThreatKnowledgeStatisticsHelper
// {
//     public class ThreatStatistic
//     {
//         public string Name { get; set; }
//         public Func<PerceptionEntry, (bool hasValue, float value)> ValueExtractor { get; set; }
//         public float? MaxValue { get; private set; }
//         public float? MinValue { get; private set; }

//         public ThreatStatistic(
//             string name,
//             Func<PerceptionEntry, (bool hasValue, float value)> valueExtractor)
//         {
//             Name = name;
//             ValueExtractor = valueExtractor;
//         }

//         public void UpdateWith(float newValue)
//         {
//             UpdateMinMax(newValue);
//         }

//         private void UpdateMinMax(float newValue)
//         {
//             MaxValue = MaxValue.HasValue ? Mathf.Max(MaxValue.Value, newValue) : newValue;
//             MinValue = MinValue.HasValue ? Mathf.Min(MinValue.Value, newValue) : newValue;
//         }
//     }

//     private ThreatStatistic[] statistics;
//     private Dictionary<string, ThreatStatistic> statisticsByName;

//     public void InitializeStatistics(
//         List<PerceptionEntry> initialPerceptions,
//         params ThreatStatistic[] statistics)
//     {
//         ValidateStatistics(statistics);
//         this.statistics = statistics;
//         statisticsByName = statistics.ToDictionary(x => x.Name, x => x);
//         ProcessPerceptionsBatch(initialPerceptions);
//     }

//     public ThreatStatistic Get(string statisticName) => statisticsByName[statisticName];

//     public void UpdateStatistics(List<PerceptionEntry> newPerceptions)
//     {
//         ProcessPerceptionsBatch(newPerceptions);
//     }

//     private void ValidateStatistics(ThreatStatistic[] statistics)
//     {
//         if (statistics == null || statistics.Length == 0)
//         {
//             throw new ArgumentException("Statistics array cannot be null or empty");
//         }
//     }

//     private void ProcessPerceptionsBatch(List<PerceptionEntry> perceptions)
//     {
//         foreach (var perception in perceptions)
//         {
//             ProcessSinglePerception(perception);
//         }
//     }

//     private void ProcessSinglePerception(PerceptionEntry perception)
//     {
//         foreach (var statistic in statistics)
//         {
//             TryUpdateStatistic(statistic, perception);
//         }
//     }

//     private void TryUpdateStatistic(ThreatStatistic statistic, PerceptionEntry perception)
//     {
//         var (hasValue, value) = statistic.ValueExtractor(perception);
//         if (hasValue)
//         {
//             statistic.UpdateWith(value);
//         }
//     }
// }