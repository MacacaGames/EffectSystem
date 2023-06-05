using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Sirenix.OdinInspector;
using MacacaGames;
using System.Linq;
#if !Server
using MacacaGames.GameSystem;
using UnityEngine;
using UnityEditor;

namespace MacacaGames.EffectSystem
{
    [CreateAssetMenu(menuName = "Data/Effect/EffectFilter", fileName = "EffectFilterTable")]
#endif
    public class EffectFilter
#if !Server
    : ScriptableObjectLifeCycle
#endif
    {
        #region SkillViewQuery
        // #if !Server
        // public MeditationItemDataGroup meditationItemDataGroup;


        //     public MeditationItemDataUsage GetMeditationData(string key)
        //     {
        //         MeditationItemDataUsage viewData;
        //         try
        //         {
        //             viewData = meditationItemDataGroup.GetDataUsage(1).SingleOrDefault(m => m.GetID() == key);

        //             if (viewData == null)
        //             {
        //                 throw new Exception();
        //             }
        //         }
        //         catch
        //         {
        //             viewData = meditationItemDataGroup.GetDataUsage(1).SingleOrDefault(m => m.GetID() == "A00");
        //             Debug.Log("can't find effect, return default effect ");
        //         }

        //         return viewData;
        //     }
        //     public List<MeditationItemDataUsage> GetMeditationDatas(List<string> keys)
        //     {
        //         List<MeditationItemDataUsage> itemList = new List<MeditationItemDataUsage>();
        //         foreach (var item in meditationItemDataGroup.GetDataUsage(1))
        //         {
        //             if (keys.Contains(item.GetID()))
        //             {
        //                 itemList.Add(item);
        //             }
        //             else
        //             {
        //                 //return Default meditationItemData
        //                 var defaultItem = meditationItemDataGroup.GetDataUsage(1).SingleOrDefault(m => m.itemData.id == "default");
        //                 if (defaultItem != null)
        //                 {
        //                     itemList.Add(defaultItem);
        //                 }
        //             }
        //         }
        //         return itemList;
        //     }
        // #endif

        #endregion

        #region UNITY_RUNTIME_CODE
#if !Server
        public List<EffectGroup> skillEffectGroups = new List<EffectGroup>();

        EffectSystem effectManager => ApplicationController.Instance.GetMonobehaviourLifeCycle<EffectSystem>();
        public override async Task Init()
        {

        }


#endif
        #endregion

        #region UNITY_EDITOR_CODE
#if UNITY_EDITOR
        [Button]
        void SetSKillEffect(string json, string jsonArray)
        {
            SetLocalSkillEffectGroup(jsonArray);
            ExportAllEffectGroupToServer(skillEffectGroups);
            ExportSkillToServer(json);
        }
        void SetLocalSkillEffectGroup(string jsonArray)
        {
            string[] jsonStrings = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(jsonArray);

            skillEffectGroups.Clear();

            //get instance and parse in json
            for (int i = 1; i <= jsonStrings.Length; i++)
            {
                string id = "SPEC" + i.ToString().PadLeft(2, '0');
                var path = $"Assets/0_Game/Data/Effect/playable/{id}.asset";
                var currentAsset = AssetDatabase.LoadAssetAtPath<EffectGroup>(path);

                //if instance not found, create one
                if (currentAsset == null)
                {
                    var obj = ScriptableObject.CreateInstance<EffectGroup>();
                    AssetDatabase.CreateAsset(obj, path);
                    currentAsset = AssetDatabase.LoadAssetAtPath<EffectGroup>(path);
                    Debug.Log("Spec not found, auto create one");
                }

                JsonUtility.FromJsonOverwrite(jsonStrings[i - 1], currentAsset);
                UnityEditor.EditorUtility.SetDirty(currentAsset);

                skillEffectGroups.Add(currentAsset);
            }
            AssetDatabase.SaveAssets();
            Debug.Log("set skill done");
        }
        void ExportSkillToServer(string json)
        {
            var path = Application.dataPath + "/UnityShared/Json/EffectFilterSpec.json";
            System.IO.File.WriteAllText(path, json);
        }

        void ExportAllEffectGroupToServer(IEnumerable<EffectGroup> effectGruops)
        {
            foreach (var item in effectGruops)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(item.effects);
                var path = Application.dataPath + "/UnityShared/Json/Effects/" + item.id + ".json";
                System.IO.File.WriteAllText(path, json);
            }

            Debug.Log("export effect group to server done");
        }

#endif
        #endregion
    }
}