using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames.EffectSystem
{
    [CreateAssetMenu(menuName = "Data/Effect/EffectGroup", fileName = "EffectGroup")]
    public class EffectGroup
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
    : ScriptableObject
#endif
    {
        public string id;

        [SerializeField]
        public string title;

        [TextArea(1, 5)]
        [SerializeField]
        public string description;

        public EffectInfo[] effects = new EffectInfo[0];

        [HideInInspector]
        public string address;

        public string GetID()
        {
            return name;
        }

        // public override IEnumerable<IQueryableData> GetData()
        // {
        //     return effects.OfType<IQueryableData>();
        // }



        [Sirenix.OdinInspector.Button]
        public void GetLitJsoneffects()
        {
            // Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(effects));

            Debug.Log(LitJson.JsonMapper.ToJson(effects));
        }
        [Sirenix.OdinInspector.Button]
        public void GetJsonNeteffects()
        {
            Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(effects));

        }
        [Sirenix.OdinInspector.Button]
        public void FromJson(string test)
        {
            var result = LitJson.JsonMapper.ToObject<List<EffectInfo>>(test);

            Debug.Log(result);
        }
        [Sirenix.OdinInspector.Button]
        public void FromJsonNet(string test)
        {
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EffectInfo>>(test);

            Debug.Log(result);
        }

        // [NodeAction("AddEffect(Hero)")]
        // public void AddEffects()
        // {
        //     if (EditorApplication.isPlaying == true)
        //     {
        //         var mainGamePlayData = ApplicationController.Instance.GetGamePlayController().GetGamePlayData<MainGamePlayData>();
        //         var effectManager = ApplicationController.Instance.GetMonobehaviourLifeCycle<EffectManager>();
        //         effectManager.AddRequestedEffects(mainGamePlayData.hero, effects, name);
        //     }
        //     else
        //     {
        //         EditorUtility.DisplayDialog("Effect Group", "只有 PlayMode 時可以使用", "OK");
        //     }
        // }
    }
}