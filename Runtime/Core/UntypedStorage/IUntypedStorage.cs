using System.Collections.Generic;
using UnityEngine;

public interface IUntypedStorage
{
    Dictionary<string, object> Data { get; }
}
