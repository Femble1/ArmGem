using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Main Character", menuName = "Character")]
public class CharacterSO : ScriptableObject
{
    public int lvl;
    public int class_lvl;
    public int exp;
    public int class_exp;
    public int max_hp;
    public int force;
    public int spirit;
    public int defense;
    public int resistance;
    public int mastership;
    public int agility;
    public int movement;
    public int max_hp_up;
    public int force_up;
    public int spirit_up;
    public int defense_up;
    public int resistance_up;
    public int mastership_up;
    public int agility_up;
    public Sprite portrait;
}
