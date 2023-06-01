using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacacaGames.EffectSystem
{
    public class AutoDestroySelf_BehaviourState : StateMachineBehaviour
    {
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Destroy(animator.gameObject);
        }
    }
}