using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MacacaGames.EffectSystem
{
    public class EffectView_StateIcon : EffectViewBase
    {
        [SerializeField]
        RectTransform viewRoot;

        [SerializeField]
        Image timeProgress;

        private void OnEnable()
        {
        }

        public override void OnStart()
        {
            base.OnStart();

        }

        public override void OnActive()
        {
            base.OnActive();

        }

        public override void OnDeactive()
        {
            base.OnDeactive();
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }

        public override void OnCooldownEnd()
        {
            base.OnCooldownEnd();
        }

        public override void OnTick()
        {
            base.OnTick();
            timeProgress.fillAmount = effectInstance.condition.maintainTimeTimer.CurrentTime /
                                      effectInstance.info.maintainTime;
            Debug.Log("OnTick");
        }
    }
}