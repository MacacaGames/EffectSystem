using System;
using UnityEngine;


public struct EffectViewInfo
{
    public string id;
    public string viewRootType;
    public string prefabAddress;

    [Newtonsoft.Json.JsonIgnore, MessagePack.IgnoreMember]
    public GameObject prefab;
    [MessagePack.IgnoreMember, Newtonsoft.Json.JsonIgnore]
    public static Func<string, GameObject> GetPrefab;
    public string prefabName;
}