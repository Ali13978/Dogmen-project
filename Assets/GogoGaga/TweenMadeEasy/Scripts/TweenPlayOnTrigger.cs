using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.TME
{
    public class TweenPlayOnTrigger : MonoBehaviour
    {
        public enum WHICHTYPE { onEnter, onExit };

        public WHICHTYPE type;
        public LeantweenCustomAnimator[] Animations;



        private void OnTriggerEnter(Collider other)
        {
            if (type == WHICHTYPE.onEnter)
                for (int i = 0; i < Animations.Length; i++)
                {
                    if (Animations[i] != null)
                        Animations[i].PlayAnimation();
                }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (type == WHICHTYPE.onEnter)
                for (int i = 0; i < Animations.Length; i++)
                {
                    if (Animations[i] != null)
                        Animations[i].PlayAnimation();
                }
        }


        private void OnTriggerExit(Collider other)
        {
            if (type == WHICHTYPE.onExit)
                for (int i = 0; i < Animations.Length; i++)
                {
                    if (Animations[i] != null)
                        Animations[i].PlayAnimation();
                }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (type == WHICHTYPE.onExit)
                for (int i = 0; i < Animations.Length; i++)
                {
                    if (Animations[i] != null)
                        Animations[i].PlayAnimation();
                }
        }

    }
}