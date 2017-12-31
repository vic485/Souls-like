using UnityEngine;
using Gazzotto.Controller;
using Gazzotto.Enemies;

namespace Gazzotto.Items
{
    public class ParryCollider : MonoBehaviour
    {
        StateManager states;
        EnemyStates eStates;

        public float maxTimer = 0.6f;
        float timer;

        public void InitPlayer(StateManager st)
        {
            states = st;
        }

        public void InitEnemy(EnemyStates eSt)
        {
            eStates = eSt;
        }

        private void Update()
        {
            if (states)
            {
                timer += states.delta;

                if (timer > maxTimer)
                {
                    timer = 0;
                    gameObject.SetActive(false);
                }
            }

            if (eStates)
            {
                timer += eStates.delta;

                if (timer > maxTimer)
                {
                    timer = 0;
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //DamageCollider dc = other.GetComponent<DamageCollider>();
            //if (dc == null)
            //    return;

            if (states)
            {
                EnemyStates e_st = other.transform.GetComponentInParent<EnemyStates>();

                if (e_st != null)
                {
                    e_st.CheckForParry(transform.root, states);
                }
            }

            if (eStates)
            {
                // TODO: check for player
            }
        }
    }
}