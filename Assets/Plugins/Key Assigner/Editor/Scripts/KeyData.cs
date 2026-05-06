using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Key Data", menuName = "Key Config", order = 52)]
public class KeyData : ScriptableObject
{
    public Key Key;
}

[Serializable]
public struct Key
{
    [Header("Key Settings")]
    [Space(5)]

    public string Path;
    public string Alias;
    public string Password;
}
