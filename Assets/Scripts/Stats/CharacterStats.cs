using UnityEngine;

namespace Gazzotto.Stats
{
    [System.Serializable]
    public class CharacterStats
    {
        [Header("Base Power")]
        public int hp = 100;
        public int fp = 100;
        public int stamina = 100;
        public float equipLoad = 20f;
        public float poise = 20;
        public int itemDiscover = 111;

        [Header("Attack Power")]
        public int r_weapon_1 = 51;
        public int r_weapon_2 = 51;
        public int r_weapon_3 = 51;
        public int l_weapon_1 = 51;
        public int l_weapon_2 = 51;
        public int l_weapon_3 = 51;

        [Header("Defence")]
        public int physical = 87;
        public int vs_strike = 87;
        public int vs_slash = 87;
        public int vs_thrust = 87;
        public int magic = 30;
        public int fire = 30;
        public int lighting = 30;
        public int dark = 30;

        [Header("Resistances")]
        public int bleed = 100;
        public int poison = 100;
        public int frost = 100;
        public int curse = 100;

        public int attunementSlots = 0;
    }

    [System.Serializable]
    public class Attributes
    {
        public int level = 1;
        public int souls = 0;
        public int vigor = 11;
        public int attunement = 11;
        public int endurance = 11;
        public int vitality = 11;
        public int strength = 11;
        public int dexterity = 11;
        public int intelligence = 11;
        public int failth = 11;
        public int luck = 11;
    }

    [System.Serializable]
    public class WeaponStats
    {
        public int physical;
        public int strike;
        public int slash;
        public int thrust;
        public int magic = 0;
        public int fire = 0;
        public int lighting = 0;
        public int dark = 0;
    }
}