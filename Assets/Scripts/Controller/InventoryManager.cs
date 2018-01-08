using UnityEngine;
using System.Collections.Generic;
using Gazzotto.Items;
using Gazzotto.Managers;
using Gazzotto.Stats;
using Gazzotto.UI;

namespace Gazzotto.Controller
{
    public class InventoryManager : MonoBehaviour
    {
        public List<string> rh_weapons = new List<string>();
        public List<string> lh_weapons = new List<string>();

        public RuntimeWeapon rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public RuntimeWeapon leftHandWeapon;

        public GameObject parryCollider;

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;

            if (rh_weapons.Count > 0)
                rightHandWeapon = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(rh_weapons[0]));

            if (lh_weapons.Count > 0)
            {
                leftHandWeapon = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(lh_weapons[0]), true);
                hasLeftHandWeapon = true;
            }

            if (rightHandWeapon != null)
                EquipWeapon(rightHandWeapon);
            if (leftHandWeapon != null)
                EquipWeapon(leftHandWeapon, true);

            hasLeftHandWeapon = (leftHandWeapon != null);

            InitAllDamageColliders();
            CloseAllDamageColliders();
            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();
        }

        public void EquipWeapon(RuntimeWeapon weapon, bool isLeft = false)
        {
            string targetIdle = weapon.instance.oh_idle;
            targetIdle += (isLeft) ? "_l" : "_r";
            states.anim.SetBool(StaticStrings.mirror, isLeft);
            states.anim.Play(StaticStrings.changeWeapon);
            states.anim.Play(targetIdle);

            QuickSlot uiSlot = QuickSlot.singleton;
            uiSlot.UpdateSlot((isLeft) ? QSlotType.lh : QSlotType.rh, weapon.instance.icon);

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
            if (rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.OpenDamageColliders();

            if (leftHandWeapon.w_hook != null)
                leftHandWeapon.w_hook.OpenDamageColliders();
        }

        public void CloseAllDamageColliders()
        {
            if (rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.CloseDamageColliders();

            if (leftHandWeapon.w_hook != null)
                leftHandWeapon.w_hook.CloseDamageColliders();
        }

        public void InitAllDamageColliders()
        {
            if (rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.InitDamageColliders(states);

            if (leftHandWeapon.w_hook != null)
                leftHandWeapon.w_hook.InitDamageColliders(states);
        }

        public void CloseParryCollider()
        {
            parryCollider.SetActive(false);
        }

        public void OpenParryCollider()
        {
            parryCollider.SetActive(true);
        }

        public RuntimeWeapon WeaponToRuntimeWeapon(Weapon w, bool isLeft = false)
        {
            GameObject go = new GameObject();
            RuntimeWeapon inst = go.AddComponent<RuntimeWeapon>();

            inst.instance = new Weapon();
            StaticFunctions.DeepCopyWeapon(w, inst.instance);

            inst.weaponModel = Instantiate(inst.instance.modelPrefab) as GameObject;
            Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            inst.weaponModel.transform.parent = p;
            inst.weaponModel.transform.localPosition = (isLeft) ? inst.instance.l_model_pos : inst.instance.r_model_pos;
            inst.weaponModel.transform.localEulerAngles = (isLeft) ? inst.instance.l_model_eulers : inst.instance.r_model_eulers;
            inst.weaponModel.transform.localScale = inst.instance.model_scale;

            inst.w_hook = inst.weaponModel.GetComponentInChildren<WeaponHook>();
            inst.w_hook.InitDamageColliders(states);

            return inst;
        }
    }

    [System.Serializable]
    public class Item
    {
        //public int itemId;
        public string itemName;
        public string itemDescription;
        public Sprite icon;
    }

    [System.Serializable]
    public class Weapon : Item
    {
        public string oh_idle;
        public string th_idle;

        public List<Action> actions;
        public List<Action> two_handedActions;

        public float parryMultiplier;
        public float backstabMultiplier;
        public bool leftHandMirror;

        public GameObject modelPrefab;

        public Action GetAction(List<Action> l, ActionInput inp)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].input == inp)
                    return l[i];
            }

            return null;
        }

        public Vector3 r_model_pos;
        public Vector3 l_model_pos;
        public Vector3 r_model_eulers;
        public Vector3 l_model_eulers;
        public Vector3 model_scale;
    }
}