﻿using UnityEngine;
using System.Collections.Generic;

/// Usage:
/// 
/// [System.Serializable]
/// class MyDictionary : SerializableDictionary<int, GameObject> {}
/// public MyDictionary dic;
///
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // We save the keys and values in two lists because Unity does understand those.
    [SerializeField]
    protected List<TKey> _keys = new List<TKey>();
    [SerializeField]
    protected List<TValue> _values = new List<TValue>();

    // Before the serialization we fill these lists
    public void OnBeforeSerialize()
    {
        //官方例子有误，去掉
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in this)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    // After the serialization we create the dictionary from the two lists
    public void OnAfterDeserialize()
    {
        
        this.Clear();
        int count = Mathf.Min(_keys.Count, _values.Count);
        for (int i = 0; i < count; ++i)
        {
            this.Add(_keys[i], _values[i]);
        }
    }
}