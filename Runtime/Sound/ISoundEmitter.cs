using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundEmitter
{
    float SoundIntensity { get; set; }
    void Emit(SoundData soundData);
}
