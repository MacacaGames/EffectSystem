using System;
using UnityEngine;


public struct EffectViewInfo
{
    public string id;
    public string viewRootType;
    public string prefabAddress;

    [Obsolete]
    [Newtonsoft.Json.JsonIgnore, MessagePack.IgnoreMember]
    public GameObject prefab;
    [Obsolete]
    [MessagePack.IgnoreMember, Newtonsoft.Json.JsonIgnore]
    public static Func<string, GameObject> GetPrefab;
    [Obsolete]
    public string prefabName;
}