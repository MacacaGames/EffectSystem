﻿using System.Collections;
using System.Collections.Generic;
using MacacaGames.EffectSystem.Model;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public abstract class EffectViewBase
#if !Server
                                        : MonoBehaviour
#endif
    {
        protected EffectInfo info;
        public EffectViewInfo viewInfo { get; protected set; }


        [System.Serializable]
        struct AudioInfo
        {
            public string audioName;
        }

        //[SerializeField]  //不常用到，所以先不顯示
        AudioInfo[] onStartAudio = new AudioInfo[0];
#if !Server
        [Header("Audio")] [SerializeField]
#endif
        AudioInfo[] onActiveAudio = new AudioInfo[0];
        
#if !Server
        [SerializeField]
#endif
        AudioInfo[] onDeactiveAudio = new AudioInfo[0];

        //[SerializeField]  //不常用到，所以先不顯示
        AudioInfo[] onEndAudio = new AudioInfo[0];

        //[SerializeField]  //不常用到，所以先不顯示
        AudioInfo[] onCooldownEndAudio = new AudioInfo[0];
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

        public virtual void OnCooldownEnd()
        {
            foreach (var audioInfo in onCooldownEndAudio)
            {
                //AudioController.Instance.PlayOneShot(audioInfo.audioName);
                Debug.Log("FMODUnity and PlayOneShot is not implemented, plaease fix it !!!!");
            }
        }

        public virtual void OnEffectApply()
        {
            foreach (var audioInfo in onCooldownEndAudio)
            {
                //AudioController.Instance.PlayOneShot(audioInfo.audioName);
                Debug.Log("FMODUnity and PlayOneShot is not implemented, plaease fix it !!!!");
            }
        }
    }
}