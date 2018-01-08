using UnityEngine;
using System.Collections.Generic;
using Gazzotto.Controller;
using Gazzotto.Inventory;

namespace Gazzotto.Managers
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager singleton;
        Dictionary<string, int> item_ids = new Dictionary<string, int>();

        public List<Weapon> weaponList = new List<Weapon>();

        private void Awake()
        {
            if (singleton == null)
                singleton = this;
            else if (singleton != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            LoadIds();
        }

        void LoadIds()
        {
            WeaponScriptableObject obj = Resources.Load("Gazzotto.Inventory.WeaponScriptableObject") as WeaponScriptableObject;

            for (int i = 0; i < obj.weapons_all.Count; i++)
            {
                if (item_ids.ContainsKey(obj.weapons_all[i].itemName))
                {
                    Debug.LogWarning("Item is already in the dictionary!");
                }
                else
                {
                    item_ids.Add(obj.weapons_all[i].itemName, i);
                }
            }
        }

        int GetItemIDFromString(string id)
        {
            int index;
            if (item_ids.TryGetValue(id, out index))
                return index;

            return -1;
        }

        public Weapon GetWeapon(string id)
        {
            WeaponScriptableObject obj = Resources.Load("Gazzotto.Inventory.WeaponScriptableObject") as WeaponScriptableObject;

            int index = GetItemIDFromString(id);

            if (index == -1)
                return null;

            return obj.weapons_all[index];
        }
    }
}