using System;
using System.Collections.Generic;
using UnityEngine;

public static class IUntypedStorageExtensions
{
    public static bool HasKey(this IUntypedStorage untypedStorage, string key)
    {
        return untypedStorage.Data.ContainsKey(key);
    }

    public static bool TryGet<T>(this IUntypedStorage untypedStorage, string key, out T value)
    {
        if (untypedStorage.Data.TryGetValue(key, out object objValue))
        {
            value = (T)objValue;
            return true;
        }
        value = default;
        return false;
    }

    public static T Get<T>(this IUntypedStorage untypedStorage, string key)
    {
        if (!untypedStorage.TryGet<T>(key, out var value))
        {
            throw new KeyNotFoundException($"Key {key} not found in untyped data");
        }
        return value;
    }

    /// <summary>
    /// Adds or updates a value in the storage
    /// </summary>
    /// <typeparam name="T">Type of the value to store</typeparam>
    /// <param name="untypedStorage">The storage to modify</param>
    /// <param name="key">Key to associate with the value</param>
    /// <param name="value">Value to store</param>
    /// <exception cref="ArgumentNullException">Thrown when key is null</exception>
    public static void Set<T>(this IUntypedStorage untypedStorage, string key, T value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key), "Key cannot be null");
        }

        untypedStorage.Data[key] = value;
    }
}
