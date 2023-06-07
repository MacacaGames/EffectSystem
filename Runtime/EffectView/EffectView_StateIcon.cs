using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rayark.Mast;
using Coroutine = Rayark.Mast.Coroutine;
using DG.Tweening;
using MacacaGames.GameSystem;

namespace MacacaGames.EffectSystem
{
    public class EffectView_StateIcon : EffectViewBase
    {
        [SerializeField]
        RectTransform viewRoot;

        [SerializeField]
        Image timeProgress;

        enum CountTime
        {
            None = 0,
            MaintainTime = 1,
        }

        [System.Flags]
        enum DisplayTrigger
        {
            None = 0,
            OnStart = 1 << 1,
            OnActive = 1 << 2,
            OnDeactive = 1 << 3,
            OnEnd = 1 << 4,
            OnCDEnd = 1 << 5,
        }

        [SerializeField]
        DisplayTrigger openTrigger;

        [SerializeField]
        DisplayTrigger closeTrigger;

        [SerializeField]
        CountTime countTime;

        private void OnEnable()
        {
        }

        public override void OnStart()
        {
            gameObject.SetActive(false);

            base.OnStart();

            CompareTriggerFlag(DisplayTrigger.OnStart);

            if (countTime == CountTime.None)
            {
                timeProgress.fillAmount = 0F;
            }
        }

        public override void OnActive()
        {
            base.OnActive();

            CompareTriggerFlag(DisplayTrigger.OnActive);

            if (countTime == CountTime.MaintainTime)
            {
                if (info.activeMaintainTime > 0F)
                {
                    displayTimeCoroutine = new Rayark.Mast.Coroutine(DisplayTimeProgress(info.activeMaintainTime));
                    ApplicationController.Instance.GetGamePlayController().AddToUpdateExecuter(displayTimeCoroutine);
                }
                else
                {
                    timeProgress.fillAmount = 0F;
                }
            }

        }

        public override void OnDeactive()
        {
            base.OnDeactive();

            CompareTriggerFlag(DisplayTrigger.OnDeactive);

            if (countTime == CountTime.MaintainTime)
            {
                if (info.activeMaintainTime > 0F)
                {
                    ApplicationController.Instance.GetGamePlayController().RemoveFromUpdateExecuter(displayTimeCoroutine);
                }
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();

            CompareTriggerFlag(DisplayTrigger.OnEnd);
        }

        public override void OnColdDownEnd()
        {
            base.OnColdDownEnd();

            CompareTriggerFlag(DisplayTrigger.OnCDEnd);
        }

        Coroutine displayTimeCoroutine;
        IEnumerator DisplayTimeProgress(float time)
        {
            float t = 0F;

            while (t < time)
            {
                t += Coroutine.Delta;

                timeProgress.fillAmount = 1F - (t / time);

                yield return null;
            }
        }

        void CompareTriggerFlag(DisplayTrigger flag)
        {
            if (openTrigger.HasFlag(flag))
            {
                gameObject.SetActive(true);
                viewRoot.DOKill();
                viewRoot.DOScale(1F, 0.3F).From(0.8F).SetEase(Ease.OutBack);
            }

            if (closeTrigger.HasFlag(flag))
            {
                viewRoot.DOKill();
                viewRoot.DOScale(0F, 0.3F).SetEase(Ease.InBack).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            }
        }

    }
}