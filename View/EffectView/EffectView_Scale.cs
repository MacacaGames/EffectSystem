using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MacacaGames.EffectSystem
{
    public class EffectView_Scale : EffectViewBase
    {
        [Header("GameObjects")]

        [SerializeField]
        float scaleSec = 0.5F;

        [SerializeField]
        GameObject[] onStartParticle = new GameObject[0];

        [SerializeField]
        float onStartScale = 1;
        [SerializeField]
        GameObject[] onActiveParticle = new GameObject[0];
        [SerializeField]
        float onAciveScale = 1;
        [SerializeField]
        GameObject[] onDeactiveParticle = new GameObject[0];
        [SerializeField]
        float onDeactiveScale = 1;

        [SerializeField]
        GameObject[] onEndParticle = new GameObject[0];
        [SerializeField]
        float onEndScale = 1;
        [SerializeField]
        GameObject[] onCooldownEndParticle = new GameObject[0];
        [SerializeField]
        float onCooldownEndScale = 1;

        List<(GameObject, Vector3)> recoverHistory = new List<(GameObject, Vector3)>();


        private void OnEnable()
        {
            if (recoverHistory.Count != 0)
            {
                foreach (var h in recoverHistory)
                {
                    h.Item1.transform.DOScale(h.Item2, 0);
                }

                recoverHistory.Clear();
            }

        }
        public override void OnStart()
        {
            base.OnStart();

            foreach (var p in onStartParticle)
            {
                p.transform.DOScale(onStartScale, scaleSec);
                recoverHistory.Add((p.gameObject, p.transform.localScale));
            }
        }

        public override void OnActive()
        {
            base.OnActive();

            foreach (var p in onActiveParticle)
            {
                p.transform.DOScale(onAciveScale, scaleSec);
                recoverHistory.Add((p.gameObject, p.transform.localScale));
            }
        }

        public override void OnDeactive()
        {
            base.OnDeactive();

            foreach (var p in onDeactiveParticle)
            {
                p.transform.DOScale(onDeactiveScale, scaleSec);
                recoverHistory.Add((p.gameObject, p.transform.localScale));
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();

            foreach (var p in onEndParticle)
            {
                p.transform.DOScale(onEndScale, scaleSec);
                recoverHistory.Add((p.gameObject, p.transform.localScale));
            }
        }

        public override void OnCooldownEnd()
        {
            base.OnCooldownEnd();

            foreach (var p in onCooldownEndParticle)
            {
                p.transform.DOScale(onEndScale, scaleSec);
                recoverHistory.Add((p.gameObject, p.transform.localScale));
            }
        }
    }
}