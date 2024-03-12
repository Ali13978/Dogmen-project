using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.TME
{
    public class TweenPlayOnMouseHover : MonoBehaviour
    {
        public enum MOUSE_HOVER_TYPE { onMouseEnter, onMouseExit, OnMouseClickDown, OnMouseClickUp };

        [Header("Please add some collider to have the mouse over function work"), Space(20)]

        public MOUSE_HOVER_TYPE type;
        public LeantweenCustomAnimator[] Animations;


        private void OnMouseEnter()
        {
            if (type == MOUSE_HOVER_TYPE.onMouseEnter)
            {
                for (int i = 0; i < Animations.Length; i++)
                {
                    if (Animations[i] != null)
                        Animations[i].PlayAnimation();
                }
            }

        }

        private void OnMouseDown()
        {
            if (type == MOUSE_HOVER_TYPE.OnMouseClickDown)
            {
                for (int i = 0; i < Animations.Length; i++)
                {
                    if (Animations[i] != null)
                        Animations[i].PlayAnimation();
                }
            }
        }

        private void OnMouseUp()
        {
            if (type == MOUSE_HOVER_TYPE.OnMouseClickUp)
            {
                for (int i = 0; i < Animations.Length; i++)
                {
                    if (Animations[i] != null)
                        Animations[i].PlayAnimation();
                }
            }
        }

        private void OnMouseExit()
        {
            if (type == MOUSE_HOVER_TYPE.onMouseExit)
            {
                for (int i = 0; i < Animations.Length; i++)
                {
                    if (Animations[i] != null)
                        Animations[i].PlayAnimation();
                }
            }
        }


    }
}