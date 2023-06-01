using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "GameResource/EffectResource", fileName = "EffectResource")]
public class EffectResource : ScriptableObject
{   
    [SerializeField] public string EffectViewJson;

    [SerializeField] public string[] prefabPath = { "Assets/_Game/Effect/ViewPrefab/" };

    [SerializeField]
    GameObject defaultEffectViewprefabs;
    [SerializeField]
    List<GameObject> effectViewprefabs = new List<GameObject>();

    public GameObject GetEffectViewPrefab(string prefabName)
    {
        if (effectViewprefabs == null || effectViewprefabs.Count < 1)
        {
            return defaultEffectViewprefabs;
        }
        return effectViewprefabs.SingleOrDefault(m => m.name == prefabName);
    }


    #if UNITY_EDITOR
    [Button]
    private void GetAllViewPrefab()
    {
        effectViewprefabs.Clear();
        var prefabGUIDs = AssetDatabase.FindAssets("t:GameObject", prefabPath);
        foreach (var mPrefabGUID in prefabGUIDs)
        {
            var mPrefabPath = AssetDatabase.GUIDToAssetPath(mPrefabGUID);
            var fileName = Path.GetFileNameWithoutExtension(mPrefabPath);
            GameObject tmpPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(mPrefabPath, typeof(GameObject));
            if (tmpPrefab == null) continue;

            effectViewprefabs.Add(tmpPrefab);
            Debug.Log($"GameObject collected: {mPrefabPath}");
        }
    }
    #endif
}
