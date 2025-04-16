using System.Collections.Generic;
using UnityEngine;

public interface ISensoryMemoryStorage<TRepresentation> : ISensoryMemoryStorageCore
{
    IEnumerable<IPerceptedSensoryData<TRepresentation>> GetPerceptedData();
}
