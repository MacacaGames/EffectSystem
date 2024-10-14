using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public class EffectView_Animator : EffectViewBase
    {
        [SerializeField]
        Animator animator;

        private void OnEnable()
        {
            if (animator == null)
            {
                Debug.LogWarning("[EffectView_Animator] 沒有設定Animator，將嘗試取得");
                animator = GetComponent<Animator>();
            }
        }

        void ResetAnimatorTrigger()
        {
            animator.ResetTrigger("OnStart");
            animator.ResetTrigger("OnActive");
            animator.ResetTrigger("OnDeactive");
            animator.ResetTrigger("OnEnd");
            animator.ResetTrigger("OnCDEnd");
        }

        public override void OnStart()
        {
            ResetAnimatorTrigger();

            base.OnStart();
            animator.SetTrigger("OnStart");

        }

        public override void OnActive()
        {
            ResetAnimatorTrigger();

            base.OnActive();
            animator.SetTrigger("OnActive");

        }

        public override void OnDeactive()
        {
            ResetAnimatorTrigger();

            base.OnDeactive();
            animator.SetTrigger("OnDeactive");

        }

        public override void OnEnd()
        {
            ResetAnimatorTrigger();

            base.OnEnd();
            animator.SetTrigger("OnEnd");

        }

        public override void OnCooldownEnd()
        {
            ResetAnimatorTrigger();

            base.OnEnd();
            animator.SetTrigger("OnCDEnd");

        }

    }
}