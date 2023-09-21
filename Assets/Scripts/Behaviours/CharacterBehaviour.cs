using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterBehaviour : Movement
{
    public GameObject cursor;
    public CharacterSO stats;
    public Skill[] Skills = new Skill[8];
    public int looking_direction = 1;
    public bool on_lock;
    public bool npc;
    public bool gem_user;
    GameObject target;
    public GameObject last_damager;
    public bool clone;
    public bool turn;
    public bool skill_turn;
    public bool chosing_skill;
    public bool performing_attack;
    public bool armor_gem_evoking = false;
    public bool weapon_gem_evoking = false;
    public bool armor_gem_evoked = false;
    public bool weapon_gem_evoked = false;
    //Status disorders and buffs
    public bool stun, bleed, burn, oddercurse, crippled, frc_debuff, spr_debuff, def_debuff, res_debuff, mas_debuff, agi_debuff,
    counter, double_attack, evasion,dbl_dmg, dbl_mag_dmg, dbl_phy_dmg, frc_buff, spr_buff, def_buff, res_buff, mas_buff, agi_buff;
    //Stats
    public int lvl;
    public int class_lvl;
    public int exp;
    public int class_exp;
    public int max_hp;
    public int ep;
    public int hp;
    public int force;
    public int spirit;
    public int defense;
    public int resistance;
    public int mastership;
    public int agility;
    public int max_hp_up;
    public int force_up;
    public int spirit_up;
    public int defense_up;
    public int resistance_up;
    public int mastership_up;
    public int agility_up;
    public int movement;
    public int overall;
    //Events
    public UnityEvent<int> HealthChange;
    public UnityEvent<int> EpChange;
    public UnityEvent<string> GemChange;
    public UnityEvent<int> LvlChange;
    public UnityEvent<int> ExpChange;
    public UnityEvent<int> ClassLvlChange;
    public UnityEvent<int> ClassExpChange;
    //Assigning stats when reloading object
    void OnEnable()
    {
        update_status();
        overall = max_hp/3 + force + spirit + defense + resistance + mastership + agility + movement;
    }
    // Start is called before the first frame update
    void Start()
    {
        hp = max_hp;
        init();
        if (clone == false)
        {
            cursor.GetComponent<CursorBehaviour>().Characters.Add(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (turn && !moving && !skill_turn)
        {
            find_selectable_tiles(movement);
            if (!npc)
            {
                check_cursor_click();
            }
            else 
            {
                find_nearest_target();
                calculate_path();
                actual_target_tile.target = true;
            }
        }
        else if (turn && moving && !skill_turn)
        {
            walk();
        }
        else if (turn && skill_turn && !chosing_skill && !performing_attack)
        {
            chosing_skill = true;
            if (!npc)
            {
                GameObject UI = GameObject.Find("CameraCanvas");
                UI.GetComponent<UI>().character_highlighted = gameObject;
                UI.GetComponent<UI>().create_skill_buttons = true;
                cursor.GetComponent<CursorBehaviour>().on = false;
            }
            else 
            {
                actual_target_tile.target = false;
                if (!search_for_attackable_units())
                {
                    cursor.GetComponent<CursorBehaviour>().turn_end();
                }
            }
        }
        else if (turn && skill_turn && chosing_skill && !GameObject.FindGameObjectWithTag("SecondLayerButton") && !GameObject.Find("SkillBox(Clone)") && Input.GetKeyUp(KeyCode.D) && !performing_attack)
        {
            if (!npc)
            {
                go_back(gameObject);
            }
        }
    }

    //Player Actions
    public void check_cursor_click()
    {
        EnabledTile t = cursor.GetComponent<CursorBehaviour>().clicked_tile;
        if (t != null)
        {
            move_to_tile(t);
        }
    }

    public void update_status()
    {
        lvl = stats.lvl;
        class_lvl = stats.class_lvl;
        exp = stats.exp;
        class_exp = stats.class_exp;
        max_hp = stats.max_hp;
        force = stats.force;
        spirit = stats.spirit;
        defense = stats.defense;
        resistance = stats.resistance;
        mastership = stats.mastership;
        agility = stats.agility;
        max_hp_up = stats.max_hp_up;
        force_up = stats.force_up;
        spirit_up = stats.spirit_up;
        defense_up = stats.defense_up;
        resistance_up = stats.resistance_up;
        mastership_up = stats.mastership_up;
        agility_up = stats.agility_up;
        movement = stats.movement;
    }

    public void gem_evoke_restore(int gem_number, bool evoke)
    {
        if (evoke == true)
        {
            force += Skills[gem_number].Force;
            spirit += Skills[gem_number].Spirit;
            defense += Skills[gem_number].Defense;
            resistance += Skills[gem_number].Resistance;
            mastership += Skills[gem_number].Mastership;
            agility += Skills[gem_number].Agility;
        }
        else
        {
            force -= Skills[gem_number].Force;
            spirit -= Skills[gem_number].Spirit;
            defense -= Skills[gem_number].Defense;
            resistance -= Skills[gem_number].Resistance;
            mastership -= Skills[gem_number].Mastership;
            agility -= Skills[gem_number].Agility;
        }
    }

    //NPC Actions
    void calculate_path()
    {  
        EnabledTile target_tile = get_target_tile(target);
        find_path(target_tile);
    }
    void find_nearest_target()
    {
        // PROB-LOG: Searching closest tile for attack but can't consider allies as obstacles!!
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach(GameObject target in targets)
        {
            float d = Vector3.Distance(transform.position, target.transform.position);

            if (d < distance)
            {
                distance = d;
                nearest = target;
            }
        }

        target = nearest;
    }
    bool search_for_attackable_units()
    {
        foreach(GameObject EnabledTile in cursor.GetComponent<CursorBehaviour>().EnabledTiles)
        {
            EnabledTile tile = EnabledTile.GetComponent<EnabledTile>();
            if (tile.attackable == true && Vector2.Distance(new Vector2(tile.transform.position.x, tile.transform.position.z), new Vector2(current_tile.transform.position.x, current_tile.transform.position.z)) < 1.1)
            {
                BattleSceneBehaviour scene_manager = GameObject.Find("BattleSceneManager").GetComponent<BattleSceneBehaviour>();
                RaycastHit hit;
                CharacterBehaviour enemy;

                if (Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                {
                    scene_manager.add_target(hit.transform.gameObject);
                    enemy = hit.transform.gameObject.GetComponent<CharacterBehaviour>();
                    int dmg;
                    switch(Skills[0].DamageType)
                    {
                        case Skill.skill_damage.Physical:
                        dmg = (force * Skills[0].Power) / enemy.defense;
                        GameObject.FindGameObjectWithTag("BattleSceneManager").GetComponent<BattleSceneBehaviour>().add_damage(dmg);
                        break;
                        case Skill.skill_damage.Magical:
                        dmg = (spirit * Skills[0].Power) / enemy.resistance;
                        GameObject.FindGameObjectWithTag("BattleSceneManager").GetComponent<BattleSceneBehaviour>().add_damage(dmg);
                        break;
                        case Skill.skill_damage.True:
                        int average = (spirit + force) / 10;
                        dmg = average * Skills[0].Power;
                        GameObject.FindGameObjectWithTag("BattleSceneManager").GetComponent<BattleSceneBehaviour>().add_damage(dmg);
                        break;
                    }
                }

                
                scene_manager.invoke_battle(true);
                return true;
            }
        }
        return false;
    }
    public void death_check()
    {
        if (hp <= 0) 
        {
            CharacterBehaviour killer = last_damager.GetComponent<CharacterBehaviour>();
            int exp_granted = 0;
            int class_exp_granted = 0;

            GameObject.FindGameObjectWithTag("BattleSceneManager").GetComponent<BattleSceneBehaviour>().killed.Add(gameObject);
            if (killer.overall < overall)
            {
                exp_granted = 20 - ((killer.lvl - lvl) * 5) + (overall - killer.overall)/2;
            }
            else 
            {
                exp_granted = 20 - ((killer.lvl - lvl) * 5);
                class_exp_granted = (exp_granted * 2) - killer.class_lvl;
            }

            killer.exp += exp_granted;
            killer.class_exp += class_exp_granted;
        
            if (killer.exp >= 100)
            {
                killer.lvl++;
                killer.exp -= 100;
                killer.lvl_up();
            }
            if (killer.class_exp >= 100)
            {
                killer.class_lvl++;
                killer.class_exp -= 100;
                killer.class_lvl_up();
            }

            killer.ExpChange.Invoke(killer.exp);
            killer.ClassExpChange.Invoke(killer.class_exp);
        }
    }
    public void lvl_up()
    {
        for (int i = 0; i < 7; i++)
        {
            int random = Random.Range(0, 100);
            switch(i)
            {
                case 0: if (max_hp_up > random) max_hp ++; break;
                case 1: if (force_up > random) force ++; break;
                case 2: if (spirit_up > random) spirit ++; break;
                case 3: if (defense_up > random) defense ++; break;
                case 4: if (resistance_up > random) resistance ++; break;
                case 5: if (mastership_up > random) mastership ++; break;
                case 6: if (agility_up > random) agility ++; break;
            }
        }
    }
    public void class_lvl_up()
    {

    }
    public void update_gems()
    {
        if (weapon_gem_evoking == true && weapon_gem_evoked == false)
        {
            weapon_gem_evoked = true;
        }
        else if (weapon_gem_evoking == false && weapon_gem_evoked == true)
        {
            weapon_gem_evoked = false;
        }

        if (armor_gem_evoking == true && armor_gem_evoked == false)
        {
            armor_gem_evoked = true;
        }
        else if (armor_gem_evoking == false && armor_gem_evoked == true)
        {
            armor_gem_evoked = false;
        }
    }
    
    public void take_damage(int dmg)
    {
        hp -= dmg;
        HealthChange.Invoke(hp);
    }
}
