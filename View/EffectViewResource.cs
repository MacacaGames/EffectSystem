using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(menuName = "GameResource/EffectResource", fileName = "EffectResource")]
public class EffectViewResource : ScriptableObject
{   
    [SerializeField] public string EffectViewJson;

    [SerializeField] public string[] prefabPath = { "Assets/_Game/Effect/ViewPrefab/" };

    [SerializeField]
    GameObject defaultEffectViewprefabs;
    
    [SerializeField]
    List<EffectViewInfo> effectViewInfos = new List<EffectViewInfo>();
    
    [SerializeField]
    List<GameObject> effectViewprefabs = new List<GameObject>();
    
    public GameObject GetEffectViewPrefab(string effectId)
    {
        string prefabName = effectViewInfos.FirstOrDefault(x => x.id == effectId)?.prefabName;
        if (prefabName == null) return null;
        GameObject? result = effectViewprefabs.FirstOrDefault(x => x.name == prefabName);
        return result;
    }
    
    public EffectViewInfo GetEffectViewInfo(string id)
    {
        return effectViewInfos.SingleOrDefault(m => m.id == id);
    }
    

    #if UNITY_EDITOR
    [Button]
    private void GetAllViewPrefab()
    {
        effectViewInfos.Clear();
        effectViewprefabs.Clear();
        string content = EffectViewJson;
        effectViewInfos = JsonConvert.DeserializeObject<List<EffectViewInfo>>(content);
        
        var prefabGUIDs = UnityEditor.AssetDatabase.FindAssets("t:GameObject", prefabPath);
        foreach (var mPrefabGUID in prefabGUIDs)
        {
            var mPrefabPath = UnityEditor.AssetDatabase.GUIDToAssetPath(mPrefabGUID);
            var fileName = Path.GetFileNameWithoutExtension(mPrefabPath);
            GameObject tmpPrefab = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath(mPrefabPath, typeof(GameObject));
            if (tmpPrefab == null) continue;

            effectViewprefabs.Add(tmpPrefab);
            Debug.Log($"GameObject collected: {mPrefabPath}");
        }
    }
    #endif
}
