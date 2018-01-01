using UnityEngine;
using System.Collections.Generic;
using Gazzotto.Enemies;

namespace Gazzotto.Managers
{
    public class EnemyManager : MonoBehaviour
    {
        public List<EnemyTarget> enemyTargets = new List<EnemyTarget>();

        public static EnemyManager singleton;

        private void Awake()
        {
            if (singleton == null)
                singleton = this;
            else if (singleton != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public EnemyTarget GetEnemy(Vector3 from)
        {
            EnemyTarget r = null;
            float minDist = float.MaxValue;

            for (int i = 0; i < enemyTargets.Count; i++)
            {
                float tDist = Vector3.Distance(from, enemyTargets[i].transform.position);

                if (tDist < minDist)
                {
                    minDist = tDist;
                    r = enemyTargets[i];
                }
            }

            return r;
        }
    }
}