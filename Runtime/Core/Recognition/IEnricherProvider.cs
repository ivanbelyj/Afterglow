using System.Collections.Generic;
using UnityEngine;

public interface IEnricherProvider<TRepresentation>
{
    IEnumerable<IPerceptionEnricher<TRepresentation>> GetEnrichers();
}
