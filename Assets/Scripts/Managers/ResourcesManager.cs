using UnityEngine;
using System.Collections.Generic;
using Gazzotto.Controller;

namespace Gazzotto.Managers
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager singleton;

        public List<Weapon> weaponList = new List<Weapon>();
        Dictionary<string, int> weapon_dict = new Dictionary<string, int>();

        private void Awake()
        {
            if (singleton == null)
                singleton = this;
            else if (singleton != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < weaponList.Count; i++)
            {
                if (string.IsNullOrEmpty(weaponList[i].weaponId))
                    continue;

                if (!weapon_dict.ContainsKey(weaponList[i].weaponId))
                    weapon_dict.Add(weaponList[i].weaponId, i);
                else
                    Debug.Log(weaponList[i].weaponId + " is a duplicate id");
            }
        }

        public Weapon GetWeapon(string id)
        {
            int index = -1;
            if (weapon_dict.TryGetValue(id, out index))
            {
                return weaponList[index];
            }

            return null;
        }
    }
}