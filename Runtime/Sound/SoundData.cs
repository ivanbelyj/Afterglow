using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract record SoundData
{
    public Guid SoundId { get; set; }
    public abstract string ToDisplayText();

    public SoundData(Guid soundId)
    {
        SoundId = soundId;
    }
}
