using UnityEngine;
using Gazzotto.Controller;
using Gazzotto.Enemies;

namespace Gazzotto.Items
{
    public class DamageCollider : MonoBehaviour
    {
        StateManager states;

        public void Init(StateManager st)
        {
            states = st;
        }

        private void OnTriggerEnter(Collider other)
        {
            EnemyStates eStates = other.transform.GetComponentInParent<EnemyStates>();

            if (eStates == null)
                return;

            eStates.DoDamage(states.currentAction);
        }
    }
}