﻿using UnityEngine;

namespace Gazzotto.Controller
{
    public class AnimatorHook : MonoBehaviour
    {
        Animator anim;
        StateManager states;

        public float rm_multi;

        public void Init(StateManager st)
        {
            states = st;
            anim = st.anim;
        }

        private void OnAnimatorMove()
        {
            if (states.canMove)
                return;

            states.rigid.drag = 0;

            if (rm_multi == 0)
                rm_multi = 1;

            Vector3 delta = anim.deltaPosition;
            delta.y = 0;
            Vector3 v = (delta * rm_multi) / states.delta;
            states.rigid.velocity = v;
        }
    }
}