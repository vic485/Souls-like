using UnityEngine;
using Gazzotto.Controller;
using Gazzotto.Stats;

namespace Gazzotto.Managers
{
    public static class StatsCalculations
    {
        public static int CalculateBaseDamage(WeaponStats wStats, CharacterStats cStats, float multiplier = 1f)
        {
            float physical = (wStats.physical * multiplier) - cStats.physical;
            float strike = (wStats.strike * multiplier) - cStats.vs_strike;
            float slash = (wStats.slash * multiplier) - cStats.vs_slash;
            float thrust = (wStats.thrust * multiplier) - cStats.vs_thrust;

            float sum = physical + strike + slash + thrust;

            float magic = (wStats.magic * multiplier) - cStats.magic;
            float fire = (wStats.fire * multiplier) - cStats.fire;
            float lighting = (wStats.lighting * multiplier) - cStats.lighting;
            float dark = (wStats.dark * multiplier) - cStats.dark;

            sum += magic + fire + lighting + dark;

            if (sum <= 0)
                sum = 1f;

            return Mathf.RoundToInt(sum);
        }
    }
}