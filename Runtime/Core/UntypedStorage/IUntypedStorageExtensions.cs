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

    #region List

    /// <summary>
    /// Adds a value to a list stored under the specified key.
    /// Creates a new list if the key doesn't exist.
    /// </summary>
    /// <typeparam name="T">Type of list elements</typeparam>
    /// <param name="untypedStorage">The storage to modify</param>
    /// <param name="key">Key to access the list</param>
    /// <param name="value">Value to add to the list</param>
    /// <exception cref="ArgumentNullException">Thrown when key is null</exception>
    public static void AddToList<T>(this IUntypedStorage untypedStorage, string key, T value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (!untypedStorage.Data.TryGetValue(key, out var listObj))
        {
            listObj = new List<T>();
            untypedStorage.Data[key] = listObj;
        }

        if (listObj is List<T> list)
        {
            list.Add(value);
        }
        else
        {
            throw new InvalidOperationException($"Element under key '{key}' is not a List<{typeof(T).Name}>");
        }
    }

    /// <summary>
    /// Gets a list of values stored under the specified key.
    /// </summary>
    /// <typeparam name="T">Type of list elements</typeparam>
    /// <param name="untypedStorage">The storage to access</param>
    /// <param name="key">Key to access the list</param>
    /// <returns>Existing list or empty list if key doesn't exist</returns>
    /// <exception cref="ArgumentNullException">Thrown when key is null</exception>
    public static List<T> GetList<T>(this IUntypedStorage untypedStorage, string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (untypedStorage.Data.TryGetValue(key, out var listObj) && listObj is List<T> list)
        {
            return list;
        }
        return new List<T>();
    }

    /// <summary>
    /// Tries to get a list of values stored under the specified key.
    /// </summary>
    /// <typeparam name="T">Type of list elements</typeparam>
    /// <param name="untypedStorage">The storage to access</param>
    /// <param name="key">Key to access the list</param>
    /// <param name="list">Output list parameter</param>
    /// <returns>True if list exists and has correct type, false otherwise</returns>
    public static bool TryGetList<T>(this IUntypedStorage untypedStorage, string key, out List<T> list)
    {
        if (key == null || !untypedStorage.Data.TryGetValue(key, out var listObj) || listObj is not List<T> typedList)
        {
            list = default;
            return false;
        }

        list = typedList;
        return true;
    }

    #endregion
}
