using UnityEngine;
using System.Collections.Generic;
using Gazzotto.Controller;

namespace Gazzotto.Managers
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager singleton;

        public List<Weapon> weaponList = new List<Weapon>();

        private void Awake()
        {
            if (singleton == null)
                singleton = this;
            else if (singleton != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }
    }
}