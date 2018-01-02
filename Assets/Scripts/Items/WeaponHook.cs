using UnityEngine;
using Gazzotto.Controller;

namespace Gazzotto.Items
{
    public class WeaponHook : MonoBehaviour
    {
        public GameObject[] damageCollider;

        public void OpenDamageColliders()
        {
            for (int i = 0; i < damageCollider.Length; i++)
            {
                damageCollider[i].SetActive(true);
            }
        }

        public void CloseDamageColliders()
        {
            for (int i = 0; i < damageCollider.Length; i++)
            {
                damageCollider[i].SetActive(false);
            }
        }

        public void InitDamageColliders(StateManager states)
        {
            for (int i = 0; i < damageCollider.Length; i++)
            {
                damageCollider[i].GetComponent<DamageCollider>().Init(states);
            }
        }
    }
}