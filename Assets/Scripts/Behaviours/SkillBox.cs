using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBox : MonoBehaviour
{
    public bool gravity_on;
    public bool check_for_enemies;
    public int skill_box_number;
    public Skill.skill_gem skill_gem;
    public string skill_name;
    public int power;
    public int range;
    public int ep_cost;
    public float height;
    public bool damage;
    public bool miss;
    bool on_lock;
    public Skill.skill_effect skill_effect1;
    public Skill.skill_effect skill_effect2;
    public Skill.skill_effect skill_effect3;
    public Skill.skill_damage skill_damage;
    public Skill.skill_format skill_format;
    public GameObject character_highlighted;


    // Start is called before the first frame update
    void Start()
    {
        adjust_position();
        GetComponent<Renderer>().sharedMaterial.SetInteger("_Shine", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            gravity_on = false;
            foreach(GameObject Character in GetComponentInParent<CursorBehaviour>().Characters)
            {
                Character.GetComponent<CharacterBehaviour>().on_lock = false;
            }
            on_lock = false;
            check_for_enemies = true;
            Renderer[] components = GetComponentsInChildren<Renderer>();
            foreach(Renderer component in components)
            {
                if (component.material.HasProperty("_EffectColor"))
                {
                    component.enabled = false;
                }
            }
            
        }
        if (gravity_on == false) adjust_position();
        if (gravity_on == true)
        {
            gravity();
            calculate();
            find_enemy();
        }
        if (on_lock)
        {
            Renderer[] components = GetComponentsInChildren<Renderer>();
            foreach(Renderer component in components)
            {
                if (component.material.HasProperty("_EffectColor"))
                {
                    component.material.SetColor("_EffectColor", new Color(20f, 0.05f, 0.05f, 1));
                }
            }
        }
        else
        {
            Renderer[] components = GetComponentsInChildren<Renderer>();
            foreach(Renderer component in components)
            {
                if (component.material.HasProperty("_EffectColor"))
                {
                    component.material.SetColor("_EffectColor", new Color(1f, 0.05f, 0.05f, 1));
                }
            }
        }

        can_execute_attack();
    }

    public void adjust_position()
    {
        GameObject cursor = GameObject.Find("Cursor");
        switch(skill_format)
        {
            case Skill.skill_format.Horizontal:
            switch(skill_box_number)
            {
                case 1:
                switch(GetComponentInParent<CursorBehaviour>().current_direction)
                {
                    case 1: transform.position = cursor.GetComponent<Transform>().position + Vector3.forward; break;
                    case 2: transform.position = cursor.GetComponent<Transform>().position + Vector3.right; break;
                    case 3: transform.position = cursor.GetComponent<Transform>().position + Vector3.forward; break;
                    case 4: transform.position = cursor.GetComponent<Transform>().position + Vector3.right; break;
                }
                break;
                case 2:
                switch(GetComponentInParent<CursorBehaviour>().current_direction)
                {
                    case 1: transform.position = cursor.GetComponent<Transform>().position + Vector3.back; break;
                    case 2: transform.position = cursor.GetComponent<Transform>().position + Vector3.left; break;
                    case 3: transform.position = cursor.GetComponent<Transform>().position + Vector3.back; break;
                    case 4: transform.position = cursor.GetComponent<Transform>().position + Vector3.left; break;
                }
                break;
            }
            break;
        }
    }
    private void find_enemy()
    {
        check_for_enemies = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach(Collider collider in colliders)
        {
            if(collider.tag == "Enemy" && collider.transform.position.y <= character_highlighted.transform.position.y + (height/2) && collider.transform.position.y >= character_highlighted.transform.position.y - (height/2))
            {
                collider.GetComponent<CharacterBehaviour>().on_lock = true;
                on_lock = true;
            }
        }
    }

    void gravity()
    {
        Ray myRay = new Ray(transform.position, new Vector3(0,-90,0));
        RaycastHit hit;
        float tile_scale = 0f;


        if(Physics.Raycast(myRay, out hit, 1f))
        {
            if(hit.collider.tag == "Ground")
            {
                gravity_on = false;
                Renderer[] components = GetComponentsInChildren<Renderer>();
                tile_scale = hit.collider.transform.localScale.y;
                foreach(Renderer component in components)
                {
                    if (component.material.HasProperty("_EffectColor"))
                    {
                        component.enabled = true;
                    }
                }
                return;
            }
        }
        transform.position -= new Vector3(0, tile_scale, 0);
    }
    void calculate()
    {
        //Distance from enabled tiles
        foreach(GameObject EnabledTile in GetComponentInParent<CursorBehaviour>().EnabledTiles)
        {
            float distance_tile = Vector3.Distance(transform.position, EnabledTile.GetComponent<Transform>().position);
            if (distance_tile < 0.4f)
            {
                /*if (EnabledTile.GetComponent<Transform>().localScale.y == 0.5f)
                {
                    transform.position += new Vector3(0, 0.5f, 0);
                }
                if (EnabledTile.GetComponent<Transform>().localScale.y == 0.25f)
                {
                    transform.position += new Vector3(0, 0.25f, 0);
                }*/
                transform.position += new Vector3(0, EnabledTile.GetComponent<Transform>().localScale.y, 0);
            }
        }
    }
    public void can_execute_attack()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            bool enemy_in_range = false;
            BattleSceneBehaviour scene_manager = GameObject.Find("BattleSceneManager").GetComponent<BattleSceneBehaviour>();
            foreach(GameObject Character in GetComponentInParent<CursorBehaviour>().Characters)
            {
                if (Character.GetComponent<CharacterBehaviour>().on_lock && on_lock)
                {
                    scene_manager.add_target(Character);
                    enemy_in_range = true;
                    Character.GetComponent<CharacterBehaviour>().on_lock = false;
                    if (!miss)
                    {
                        if (skill_effect1 != Skill.skill_effect.none)
                        {
                            switch(skill_effect1)
                            {
                                case Skill.skill_effect.stun: Character.GetComponent<CharacterBehaviour>().stun = true; break;
                                case Skill.skill_effect.bleed: Character.GetComponent<CharacterBehaviour>().bleed = true; break;
                                case Skill.skill_effect.burn: Character.GetComponent<CharacterBehaviour>().burn = true; break;
                                case Skill.skill_effect.oddercurse: Character.GetComponent<CharacterBehaviour>().oddercurse = true; break;
                                case Skill.skill_effect.crippled: Character.GetComponent<CharacterBehaviour>().crippled = true; break;
                                case Skill.skill_effect.frc_debuff: Character.GetComponent<CharacterBehaviour>().frc_debuff = true; break;
                                case Skill.skill_effect.spr_debuff: Character.GetComponent<CharacterBehaviour>().spr_debuff = true; break;
                                case Skill.skill_effect.def_debuff: Character.GetComponent<CharacterBehaviour>().def_debuff = true; break;
                                case Skill.skill_effect.res_debuff: Character.GetComponent<CharacterBehaviour>().res_debuff = true; break;
                                case Skill.skill_effect.mas_debuff: Character.GetComponent<CharacterBehaviour>().mas_debuff = true; break;
                                case Skill.skill_effect.agi_debuff: Character.GetComponent<CharacterBehaviour>().agi_debuff = true; break;
                                case Skill.skill_effect.counter: Character.GetComponent<CharacterBehaviour>().counter = true; break;
                                case Skill.skill_effect.double_attack: Character.GetComponent<CharacterBehaviour>().double_attack = true; break;
                                case Skill.skill_effect.evasion: Character.GetComponent<CharacterBehaviour>().evasion = true; break;
                                case Skill.skill_effect.dbl_dmg: Character.GetComponent<CharacterBehaviour>().dbl_dmg = true; break;
                                case Skill.skill_effect.dbl_mag_dmg: Character.GetComponent<CharacterBehaviour>().dbl_mag_dmg = true; break;
                                case Skill.skill_effect.dbl_phy_dmg: Character.GetComponent<CharacterBehaviour>().dbl_phy_dmg = true; break;
                                case Skill.skill_effect.frc_buff: Character.GetComponent<CharacterBehaviour>().frc_buff = true; break;
                                case Skill.skill_effect.spr_buff: Character.GetComponent<CharacterBehaviour>().spr_buff = true; break;
                                case Skill.skill_effect.def_buff: Character.GetComponent<CharacterBehaviour>().def_buff = true; break;
                                case Skill.skill_effect.res_buff: Character.GetComponent<CharacterBehaviour>().res_buff = true; break;
                                case Skill.skill_effect.mas_buff: Character.GetComponent<CharacterBehaviour>().mas_buff = true; break;
                                case Skill.skill_effect.agi_buff: Character.GetComponent<CharacterBehaviour>().agi_buff = true; break;
                            }
                        }
                        //Searching for status disorders and buffs
                        if (skill_effect2 != Skill.skill_effect.none)
                        {
                            switch(skill_effect2)
                            {
                                case Skill.skill_effect.stun: Character.GetComponent<CharacterBehaviour>().stun = true; break;
                                case Skill.skill_effect.bleed: Character.GetComponent<CharacterBehaviour>().bleed = true; break;
                                case Skill.skill_effect.burn: Character.GetComponent<CharacterBehaviour>().burn = true; break;
                                case Skill.skill_effect.oddercurse: Character.GetComponent<CharacterBehaviour>().oddercurse = true; break;
                                case Skill.skill_effect.crippled: Character.GetComponent<CharacterBehaviour>().crippled = true; break;
                                case Skill.skill_effect.frc_debuff: Character.GetComponent<CharacterBehaviour>().frc_debuff = true; break;
                                case Skill.skill_effect.spr_debuff: Character.GetComponent<CharacterBehaviour>().spr_debuff = true; break;
                                case Skill.skill_effect.def_debuff: Character.GetComponent<CharacterBehaviour>().def_debuff = true; break;
                                case Skill.skill_effect.res_debuff: Character.GetComponent<CharacterBehaviour>().res_debuff = true; break;
                                case Skill.skill_effect.mas_debuff: Character.GetComponent<CharacterBehaviour>().mas_debuff = true; break;
                                case Skill.skill_effect.agi_debuff: Character.GetComponent<CharacterBehaviour>().agi_debuff = true; break;
                                case Skill.skill_effect.counter: Character.GetComponent<CharacterBehaviour>().counter = true; break;
                                case Skill.skill_effect.double_attack: Character.GetComponent<CharacterBehaviour>().double_attack = true; break;
                                case Skill.skill_effect.evasion: Character.GetComponent<CharacterBehaviour>().evasion = true; break;
                                case Skill.skill_effect.dbl_dmg: Character.GetComponent<CharacterBehaviour>().dbl_dmg = true; break;
                                case Skill.skill_effect.dbl_mag_dmg: Character.GetComponent<CharacterBehaviour>().dbl_mag_dmg = true; break;
                                case Skill.skill_effect.dbl_phy_dmg: Character.GetComponent<CharacterBehaviour>().dbl_phy_dmg = true; break;
                                case Skill.skill_effect.frc_buff: Character.GetComponent<CharacterBehaviour>().frc_buff = true; break;
                                case Skill.skill_effect.spr_buff: Character.GetComponent<CharacterBehaviour>().spr_buff = true; break;
                                case Skill.skill_effect.def_buff: Character.GetComponent<CharacterBehaviour>().def_buff = true; break;
                                case Skill.skill_effect.res_buff: Character.GetComponent<CharacterBehaviour>().res_buff = true; break;
                                case Skill.skill_effect.mas_buff: Character.GetComponent<CharacterBehaviour>().mas_buff = true; break;
                                case Skill.skill_effect.agi_buff: Character.GetComponent<CharacterBehaviour>().agi_buff = true; break;
                            }
                        }
                        //Searching for status disorders and buffs
                        if (skill_effect3 != Skill.skill_effect.none)
                        {
                            switch(skill_effect3)
                            {
                                case Skill.skill_effect.stun: Character.GetComponent<CharacterBehaviour>().stun = true; break;
                                case Skill.skill_effect.bleed: Character.GetComponent<CharacterBehaviour>().bleed = true; break;
                                case Skill.skill_effect.burn: Character.GetComponent<CharacterBehaviour>().burn = true; break;
                                case Skill.skill_effect.oddercurse: Character.GetComponent<CharacterBehaviour>().oddercurse = true; break;
                                case Skill.skill_effect.crippled: Character.GetComponent<CharacterBehaviour>().crippled = true; break;
                                case Skill.skill_effect.frc_debuff: Character.GetComponent<CharacterBehaviour>().frc_debuff = true; break;
                                case Skill.skill_effect.spr_debuff: Character.GetComponent<CharacterBehaviour>().spr_debuff = true; break;
                                case Skill.skill_effect.def_debuff: Character.GetComponent<CharacterBehaviour>().def_debuff = true; break;
                                case Skill.skill_effect.res_debuff: Character.GetComponent<CharacterBehaviour>().res_debuff = true; break;
                                case Skill.skill_effect.mas_debuff: Character.GetComponent<CharacterBehaviour>().mas_debuff = true; break;
                                case Skill.skill_effect.agi_debuff: Character.GetComponent<CharacterBehaviour>().agi_debuff = true; break;
                                case Skill.skill_effect.counter: Character.GetComponent<CharacterBehaviour>().counter = true; break;
                                case Skill.skill_effect.double_attack: Character.GetComponent<CharacterBehaviour>().double_attack = true; break;
                                case Skill.skill_effect.evasion: Character.GetComponent<CharacterBehaviour>().evasion = true; break;
                                case Skill.skill_effect.dbl_dmg: Character.GetComponent<CharacterBehaviour>().dbl_dmg = true; break;
                                case Skill.skill_effect.dbl_mag_dmg: Character.GetComponent<CharacterBehaviour>().dbl_mag_dmg = true; break;
                                case Skill.skill_effect.dbl_phy_dmg: Character.GetComponent<CharacterBehaviour>().dbl_phy_dmg = true; break;
                                case Skill.skill_effect.frc_buff: Character.GetComponent<CharacterBehaviour>().frc_buff = true; break;
                                case Skill.skill_effect.spr_buff: Character.GetComponent<CharacterBehaviour>().spr_buff = true; break;
                                case Skill.skill_effect.def_buff: Character.GetComponent<CharacterBehaviour>().def_buff = true; break;
                                case Skill.skill_effect.res_buff: Character.GetComponent<CharacterBehaviour>().res_buff = true; break;
                                case Skill.skill_effect.mas_buff: Character.GetComponent<CharacterBehaviour>().mas_buff = true; break;
                                case Skill.skill_effect.agi_buff: Character.GetComponent<CharacterBehaviour>().agi_buff = true; break;
                            }
                        }
                    }
                    //Searching for status disorders and buffs
                    
                    //Calculate damage
                    if (damage)
                    {
                        int dmg;
                        switch(skill_damage)
                        {
                            case Skill.skill_damage.Physical:
                            dmg = (GetComponentInParent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().force * power) / Character.GetComponent<CharacterBehaviour>().defense;
                            GameObject.FindGameObjectWithTag("BattleSceneManager").GetComponent<BattleSceneBehaviour>().add_damage(dmg);
                            break;
                            case Skill.skill_damage.Magical:
                            dmg = (GetComponentInParent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().spirit * power) / Character.GetComponent<CharacterBehaviour>().resistance;
                            GameObject.FindGameObjectWithTag("BattleSceneManager").GetComponent<BattleSceneBehaviour>().add_damage(dmg);
                            break;
                            case Skill.skill_damage.True:
                            int average = (GetComponentInParent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().spirit + GetComponentInParent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().force) / 10;
                            dmg = average * power;
                            GameObject.FindGameObjectWithTag("BattleSceneManager").GetComponent<BattleSceneBehaviour>().add_damage(dmg);
                            break;
                        }
                    }
                    
                }
            }
            if (enemy_in_range && GetComponentInParent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().npc == false)
            {
                GetComponentInParent<CursorBehaviour>().ep_drain(ep_cost);
                GetComponentInParent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().EpChange.Invoke(GetComponentInParent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().ep);
                scene_manager.invoke_battle(true);
            }
        }
    }

    
}
