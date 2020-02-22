﻿namespace SaintCoinach.Cmd
{
    public enum ItemAttribute : byte
    {
        Strength = 1,
        Dexterity = 2,
        Vitality = 3,
        Intelligence = 4,
        Mind = 5,
        Piety = 6,
        HP = 7,
        MP = 8,
        TP = 9,
        GP = 10,
        CP = 11,
        Physical_Damage = 12,
        Magic_Damage = 13,
        Delay = 14,
        Additional_Effect_ = 0xF,
        Attack_Speed = 0x10,
        Block_Rate = 17,
        Block_Strength = 18,
        Parry = 19,
        Attack_Power = 20,
        Defense = 21,
        Accuracy = 22,
        Evasion = 23,
        Magic_Defense = 24,
        Critical_Hit_Power = 25,
        Critical_Hit_Resilience = 26,
        Critical_Hit_Rate = 27,
        Critical_Hit_Evasion = 28,
        Slashing_Resistance = 29,
        Piercing_Resistance = 30,
        Blunt_Resistance = 0x1F,
        Projectile_Resistance = 0x20,
        Attack_Magic_Potency = 33,
        Healing_Magic_Potency = 34,
        Enhancement_Magic_Potency = 35,
        Enfeebling_Magic_Potency = 36,
        Fire_Resistance = 37,
        Ice_Resistance = 38,
        Wind_Resistance = 39,
        Earth_Resistance = 40,
        Lightning_Resistance = 41,
        Water_Resistance = 42,
        Magic_Resistance = 43,
        Determination = 44,
        Skill_Speed = 45,
        Spell_Speed = 46,
        Morale = 48,
        Enmity = 49,
        Enmity_Reduction = 50,
        Resting_HP = 51,
        Resting_MP = 52,
        Regen = 53,
        Refresh = 54,
        Movement_Speed = 55,
        Spikes = 56,
        Slow_Resistance = 57,
        Petrification_Resistance = 58,
        Paralysis_Resistance = 59,
        Silence_Resistance = 60,
        Blind_Resistance = 61,
        Poison_Resistance = 62,
        Stun_Resistance = 0x3F,
        Sleep_Resistance = 0x40,
        Bind_Resistance = 65,
        Heavy_Resistance = 66,
        Doom_Resistance = 67,
        Reduced_Durability_Loss = 68,
        Increased_Spiritbond_Gain = 69,
        Craftsmanship = 70,
        Control = 71,
        Gathering = 72,
        Perception = 73
    }
}