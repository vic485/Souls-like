using UnityEngine;
using Gazzotto.Controller;
using Gazzotto.Inventory;

namespace Gazzotto.Utilities
{
    /// <summary>
    /// To Use: Create empty game object with this component attached under hand at 0, 0, 0.
    /// Make weapon prefab a child of this object and move to desired position.
    /// Drag child object into inspector as weapon model, and input the id to be the same as in inventory.
    /// Set if this is the left hand or not, and click save weapon.
    /// </summary>
    [ExecuteInEditMode]
    public class WeaponPlacer : MonoBehaviour
    {
        public string weaponId;

        public GameObject weaponModel;

        public bool leftHand;
        public bool saveWeapon;

        private void Update()
        {
            if (!saveWeapon)
                return;
            saveWeapon = false;

            if (weaponModel == null)
                return;
            if (string.IsNullOrEmpty(weaponId))
                return;

            WeaponScriptableObject obj = Resources.Load("Gazzotto.Inventory.WeaponScriptableObject") as WeaponScriptableObject;

            if (obj == null)
                return;

            for (int i = 0; i < obj.weapons_all.Count; i++)
            {
                if (obj.weapons_all[i].itemName.Equals(weaponId))
                {
                    Weapon w = obj.weapons_all[i];
                    if (leftHand)
                    {
                        w.l_model_eulers = weaponModel.transform.localEulerAngles;
                        w.l_model_pos = weaponModel.transform.localPosition;
                    }
                    else
                    {
                        w.r_model_eulers = weaponModel.transform.localEulerAngles;
                        w.r_model_pos = weaponModel.transform.localPosition;
                    }

                    w.model_scale = weaponModel.transform.localScale;
                    print("Successfully saved " + ((leftHand) ? "left hand" : "right hand") + " weapon position.");
                    return;
                }
            }

            Debug.Log(weaponId + " was not found in inventory.");
        }
    }
}