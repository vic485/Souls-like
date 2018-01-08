using UnityEngine;
using System.Collections.Generic;
using Gazzotto.Controller;

namespace Gazzotto.Inventory
{
    public class WeaponScriptableObject : ScriptableObject
    {
        public List<Weapon> weapons_all = new List<Weapon>();
    }
}