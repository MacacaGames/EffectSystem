using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public class EffectView_MultiController : EffectViewBase
    {

        [SerializeField]
        EffectViewBase[] effectViews = new EffectViewBase[0];



        public override void OnStart()
        {
            base.OnStart();

            foreach (var el in effectViews)
            {
                el.OnStart();
            }
        }

        public override void OnActive()
        {
            base.OnActive();

            foreach (var el in effectViews)
            {
                el.OnActive();
            }
        }

        public override void OnDeactive()
        {
            base.OnDeactive();

            foreach (var el in effectViews)
            {
                el.OnDeactive();
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();

            foreach (var el in effectViews)
            {
                el.OnEnd();
            }
        }

        public override void OnColdDownEnd()
        {
            base.OnEnd();

            foreach (var el in effectViews)
            {
                el.OnColdDownEnd();
            }
        }

#if (UNITY_EDITOR)

        public EffectViewBase[] GetEffectViews() => effectViews;

#endif


    }
}