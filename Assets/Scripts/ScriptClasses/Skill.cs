using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Inventory")]
public class Skill : ScriptableObject
{
    public enum skill_gem {Skill, WeaponGem, ArmorGem};
    public skill_gem SkillOrGem = skill_gem.Skill;
    public string SkillName = "New Skill";
    public string Description = "New Skill Description";
    public string PassiveDescription = "New Skill Passive Description";
    public int Power = 0;
    public int Range = 0;
    public float Height = 0;
    public int ep_cost = 0;
    public bool Damage = false;
    public enum skill_effect {none, stun, bleed, burn, oddercurse, crippled, frc_debuff, spr_debuff, def_debuff, res_debuff, mas_debuff, agi_debuff, counter, double_attack, evasion, dbl_dmg, dbl_mag_dmg, dbl_phy_dmg, frc_buff, spr_buff, def_buff, res_buff, mas_buff, agi_buff};
    public skill_effect Effect1 = skill_effect.none;
    public skill_effect Effect2 = skill_effect.none;
    public skill_effect Effect3 = skill_effect.none;
    public skill_effect ActivationEffect1 = skill_effect.none;
    public skill_effect ActivationEffect2 = skill_effect.none;
    public skill_effect ActivationEffect3 = skill_effect.none;
    public enum skill_damage {Physical, Magical, True, None};
    public skill_damage DamageType = skill_damage.None;
    public enum skill_format {Square, Vertical, Horizontal, Plus, Cross};
    public skill_format Format = skill_format.Square;
    public int FormatSize = 0;
    public enum skill_type {Melee, Ranged};
    public skill_type Type = skill_type.Melee;
    public int Force = 0;
    public int Spirit = 0;
    public int Defense = 0;
    public int Resistance = 0;
    public int Mastership = 0;
    public int Agility = 0;
}
