using UnityEngine;
using Gazzotto.Controller;
using Gazzotto.Stats;

namespace Gazzotto.Managers
{
    public static class StatsCalculations
    {
        public static int CalculateBaseDamage(WeaponStats wStats, CharacterStats cStats)
        {
            int physical = wStats.physical - cStats.physical;
            int strike = wStats.strike - cStats.vs_strike;
            int slash = wStats.slash - cStats.vs_slash;
            int thrust = wStats.thrust - cStats.vs_thrust;

            int sum = physical + strike + slash + thrust;

            int magic = wStats.magic - cStats.magic;
            int fire = wStats.fire - cStats.fire;
            int lighting = wStats.lighting - cStats.lighting;
            int dark = wStats.dark - cStats.dark;

            sum += magic + fire + lighting + dark;

            if (sum <= 0)
                sum = 1;

            return sum;
        }
    }
}