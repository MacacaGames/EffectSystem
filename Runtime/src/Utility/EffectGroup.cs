using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames.EffectSystem
{
#if !Server
    [CreateAssetMenu(menuName = "Data/EffectSystem/EffectGroup", fileName = "EffectGroup")]
#endif
    public class EffectGroup
#if !Server
        : ScriptableObject
#endif
    {
        public string id;

        public string title;

        public string description;

        public List<EffectInfo> effects = new List<EffectInfo>();

        public string address;

        public string GetID()
        {
            return id;
        }

        public void ExportEffectToJson()
        {
            EffectInfoExtensions.Log(Newtonsoft.Json.JsonConvert.SerializeObject(effects));
        }

        public void FromJson(string test)
        {
            effects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EffectInfo>>(test);
        }
    }
}