using System.Collections;
using System.Collections.Generic;
using MacacaGames.EffectSystem.Model;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    [DisallowMultipleComponent]
    public abstract class EffectViewBase : MonoBehaviour
    {
        protected EffectInfo info;
        public EffectViewInfo viewInfo { get; protected set; }


        [System.Serializable]
        struct AudioInfo
        {
            //[EventRef]
            public string audioName;
        }

        //[SerializeField]  //不常用到，所以先不顯示
        AudioInfo[] onStartAudio = new AudioInfo[0];
        [Header("Audio")]
        [SerializeField]
        AudioInfo[] onActiveAudio = new AudioInfo[0];
        [SerializeField]
        AudioInfo[] onDeactiveAudio = new AudioInfo[0];
        //[SerializeField]  //不常用到，所以先不顯示
        AudioInfo[] onEndAudio = new AudioInfo[0];
        //[SerializeField]  //不常用到，所以先不顯示
        AudioInfo[] onColdDownEndAudio = new AudioInfo[0];
        AudioInfo[] OnEffectApplyAudio = new AudioInfo[0];


        public virtual void Init(EffectInfo info, EffectViewInfo viewInfo)
        {
            this.info = info;
            this.viewInfo = viewInfo;
        }

        public virtual void OnStart()
        {
            foreach (var audioInfo in onStartAudio)
            {
                //AudioController.Instance.PlayOneShot(audioInfo.audioName);
                Debug.Log("FMODUnity and PlayOneShot is not implemented, plaease fix it !!!!");
            }
        }

        public virtual void OnActive()
        {
            foreach (var audioInfo in onActiveAudio)
            {
                //AudioController.Instance.PlayOneShot(audioInfo.audioName);
                Debug.Log("FMODUnity and PlayOneShot is not implemented, plaease fix it !!!!");
            }
        }

        public virtual void OnDeactive()
        {
            foreach (var audioInfo in onDeactiveAudio)
            {
                //AudioController.Instance.PlayOneShot(audioInfo.audioName);
                Debug.Log("FMODUnity and PlayOneShot is not implemented, plaease fix it !!!!");
            }
        }

        public virtual void OnEnd()
        {
            foreach (var audioInfo in onEndAudio)
            {
                //AudioController.Instance.PlayOneShot(audioInfo.audioName);
                Debug.Log("FMODUnity and PlayOneShot is not implemented, plaease fix it !!!!");
            }
        }

        public virtual void OnColdDownEnd()
        {
            foreach (var audioInfo in onColdDownEndAudio)
            {
                //AudioController.Instance.PlayOneShot(audioInfo.audioName);
                Debug.Log("FMODUnity and PlayOneShot is not implemented, plaease fix it !!!!");
            }
        }

        public virtual void OnEffectApply()
        {
            foreach (var audioInfo in onColdDownEndAudio)
            {
                //AudioController.Instance.PlayOneShot(audioInfo.audioName);
                Debug.Log("FMODUnity and PlayOneShot is not implemented, plaease fix it !!!!");
            }
        }

    }
}