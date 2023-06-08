using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames.EffectSystem
{
    [CreateAssetMenu(menuName = "Data/EffectSystem/EffectGroup", fileName = "EffectGroup")]
    public class EffectGroup
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
    : ScriptableObject
#endif
    {
        public string id;

        [SerializeField]
        public string title;

        [SerializeField]
        public string description;

        public List<EffectInfo> effects = new List<EffectInfo>();

        [HideInInspector]
        public string address;

        public string GetID()
        {
            return name;
        }

        public void ExportEffectToJson()
        {
            Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(effects));
        }

        public void FromJson(string test)
        {
           effects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EffectInfo>>(test);
        }
    }
}