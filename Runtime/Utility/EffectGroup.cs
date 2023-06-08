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

        public EffectInfo[] effects = new EffectInfo[0];

        [HideInInspector]
        public string address;

        public string GetID()
        {
            return name;
        }

        public void GetJsonNeteffects()
        {
            Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(effects));
        }
      
        public void FromJsonNet(string test)
        {
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EffectInfo>>(test);
        }
    }
}