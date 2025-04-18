using System.Collections.Generic;
using UnityEngine;

public interface IPerceptionEnricherProvider<TRepresentation>
{
    IEnumerable<IPerceptionEnricher<TRepresentation>> GetEnrichers();
}
