using UnityEngine;
using Gazzotto.Enemies;

namespace Gazzotto.Items
{
    public class DamageCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            EnemyStates eStates = other.transform.GetComponentInParent<EnemyStates>();

            if (eStates == null)
                return;

            // do damage
            eStates.DoDamage(35);
        }
    }
}