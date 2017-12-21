using UnityEngine;
using System.Collections.Generic;

namespace Gazzotto.Controller
{
    public class InventoryManager : MonoBehaviour
    {
        public Weapon curWeapon;

        public void Init()
        {

        }
    }

    [System.Serializable]
    public class Weapon
    {
        public List<Action> actions;
        public List<Action> two_handedActions;
    }
}