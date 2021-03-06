﻿using UnityEngine;
using Gazzotto.Managers;

namespace Gazzotto.Controller
{
    public class InputHandler : MonoBehaviour
    {
        float vertical;
        float horizontal;
        bool b_input;
        bool a_input;
        bool x_input;
        bool y_input;

        bool rb_input;
        float rt_axis;
        bool rt_input;
        bool lb_input;
        float lt_axis;
        bool lt_input;

        bool leftAxis_down;
        bool rightAxis_down;

        float b_timer;
        float rt_timer;
        float lt_timer;

        StateManager states;
        CameraManager camManager;

        float delta;

        private void Start()
        {
            UI.QuickSlot.singleton.Init();

            states = GetComponent<StateManager>();
            states.Init();

            camManager = CameraManager.singleton;
            camManager.Init(states);
        }

        private void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            states.FixedTick(delta);
            camManager.Tick(delta);
        }

        private void Update()
        {
            delta = Time.deltaTime;
            states.Tick(delta);
            ResetInputAndStates();
        }

        void GetInput()
        {
            vertical = Input.GetAxis(StaticStrings.Vertical);
            horizontal = Input.GetAxis(StaticStrings.Horizontal);
            b_input = Input.GetButton(StaticStrings.B);
            a_input = Input.GetButton(StaticStrings.A);
            y_input = Input.GetButtonUp(StaticStrings.Y);
            x_input = Input.GetButton(StaticStrings.X);
            rt_input = Input.GetButton(StaticStrings.RT);
            rt_axis = Input.GetAxis(StaticStrings.RT);
            if (rt_axis != 0)
                rt_input = true;

            lt_input = Input.GetButton(StaticStrings.LT);
            lt_axis = Input.GetAxis(StaticStrings.LT);
            if (lt_axis != 0)
                lt_input = true;

            rb_input = Input.GetButton(StaticStrings.RB);
            lb_input = Input.GetButton(StaticStrings.LB);

            rightAxis_down = Input.GetButtonUp(StaticStrings.L);

            if (b_input)
                b_timer += delta;
        }

        void UpdateStates()
        {
            states.horizontal = horizontal;
            states.vertical = vertical;

            Vector3 v = vertical * camManager.transform.forward;
            Vector3 h = horizontal * camManager.transform.right;
            states.moveDir = (v + h).normalized;
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            states.moveAmount = Mathf.Clamp01(m);

            if (x_input)
                b_input = false;

            if (b_input && b_timer > 0.5f)
            {
                states.run = (states.moveAmount > 0);
            }

            if (!b_input && b_timer > 0 && b_timer < 0.5f)
            {
                states.rollInput = true;
            }

            states.itemInput = x_input;
            states.rt = rt_input;
            states.lt = lt_input;
            states.rb = rb_input;
            states.lb = lb_input;

            if (y_input)
            {
                states.isTwoHanded = !states.isTwoHanded;
                states.HandleTwoHanded();
            }

            if (states.lockOnTarget != null)
            {
                if (states.lockOnTarget.eStates.isDead)
                {
                    states.lockOn = false;
                    states.lockOnTarget = null;
                    states.lockOnTransform = null;
                    camManager.lockon = false;
                    camManager.lockonTarget = null;
                }
            }
            else
            {
                states.lockOn = false;
                states.lockOnTarget = null;
                states.lockOnTransform = null;
                camManager.lockon = false;
                camManager.lockonTarget = null;
            }

            if (rightAxis_down)
            {
                states.lockOn = !states.lockOn;

                states.lockOnTarget = EnemyManager.singleton.GetEnemy(transform.position);
                if (states.lockOnTarget == null)
                    states.lockOn = false;

                camManager.lockonTarget = states.lockOnTarget;
                states.lockOnTransform = states.lockOnTarget.GetTarget();
                camManager.lockonTransform = states.lockOnTransform;
                camManager.lockon = states.lockOn;
            }
        }

        void ResetInputAndStates()
        {
            if (b_input == false)
                b_timer = 0;

            if (states.rollInput)
                states.rollInput = false;
            if (states.run)
                states.run = false;
        }
    }
}