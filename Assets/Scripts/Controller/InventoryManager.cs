﻿using UnityEngine;
using System.Collections.Generic;
using Gazzotto.Items;
using Gazzotto.Managers;
using Gazzotto.Stats;
using Gazzotto.UI;

namespace Gazzotto.Controller
{
    public class InventoryManager : MonoBehaviour
    {
        public ItemInstance rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public ItemInstance leftHandWeapon;

        public GameObject parryCollider;

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;
            if (rightHandWeapon != null)
                EquipWeapon(rightHandWeapon.instance);
            if (leftHandWeapon != null)
                EquipWeapon(leftHandWeapon.instance, true);

            hasLeftHandWeapon = (leftHandWeapon != null);

            InitAllDamageColliders();
            CloseAllDamageColliders();
            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();
        }

        public void EquipWeapon(Weapon weapon, bool isLeft = false)
        {
            string targetIdle = weapon.oh_idle;
            targetIdle += (isLeft) ? "_l" : "_r";
            states.anim.SetBool(StaticStrings.mirror, isLeft);
            states.anim.Play(StaticStrings.changeWeapon);
            states.anim.Play(targetIdle);

            QuickSlot uiSlot = QuickSlot.singleton;
            uiSlot.UpdateSlot((isLeft) ? QSlotType.lh : QSlotType.rh, weapon.icon);

            weapon.weaponModel.SetActive(true);
        }

        public Weapon GetCurrentWeapon(bool isLeft)
        {
            if (isLeft)
                return leftHandWeapon.instance;
            else
                return rightHandWeapon.instance;
        }

        public void OpenAllDamageColliders()
        {
            if (rightHandWeapon.instance.w_hook != null)
                rightHandWeapon.instance.w_hook.OpenDamageColliders();

            if (leftHandWeapon.instance.w_hook != null)
                leftHandWeapon.instance.w_hook.OpenDamageColliders();
        }

        public void CloseAllDamageColliders()
        {
            if (rightHandWeapon.instance.w_hook != null)
                rightHandWeapon.instance.w_hook.CloseDamageColliders();

            if (leftHandWeapon.instance.w_hook != null)
                leftHandWeapon.instance.w_hook.CloseDamageColliders();
        }

        public void InitAllDamageColliders()
        {
            if (rightHandWeapon.instance.w_hook != null)
                rightHandWeapon.instance.w_hook.InitDamageColliders(states);

            if (leftHandWeapon.instance.w_hook != null)
                leftHandWeapon.instance.w_hook.InitDamageColliders(states);
        }

        public void CloseParryCollider()
        {
            parryCollider.SetActive(false);
        }

        public void OpenParryCollider()
        {
            parryCollider.SetActive(true);
        }
    }

    [System.Serializable]
    public class Weapon
    {
        public string weaponId;
        public Sprite icon;
        public string oh_idle;
        public string th_idle;

        public List<Action> actions;
        public List<Action> two_handedActions;

        public float parryMultiplier;
        public float backstabMultiplier;
        public bool leftHandMirror;
        public GameObject weaponModel;
        public WeaponHook w_hook;

        public Action GetAction(List<Action> l, ActionInput inp)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].input == inp)
                    return l[i];
            }

            return null;
        }

        public Vector3 model_pos;
        public Vector3 model_eulers;
        public Vector3 model_scale;
    }
}