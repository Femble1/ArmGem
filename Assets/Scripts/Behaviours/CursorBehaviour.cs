using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CursorBehaviour : MonoBehaviour
{
    public float current_direction = 0;

    Vector3 next_pos, destination, direction, ray_dir;
    
    float speed = 1000f;
    float ray_lenght2 = 5.0f;
    public float move_cd = 0;
    bool move_pressed;
    bool can_move;
    bool calculate_tiles;
    bool game_start;
    bool melee_movement = false;
    public bool on = true;
    public bool attack_directing;
    [SerializeField]
    bool up;
    [SerializeField]
    bool down;
    [SerializeField]
    bool left;
    [SerializeField]
    bool right;
    bool sorted_by_speed = false;
    public EnabledTile clicked_tile = null;
    
    public bool ep_cost_informed;

    public GameObject cursor_sprite;
    public GameObject cursor_effect;
    public GameObject cursor_effect1;
    public GameObject cursor_effect2;
    public GameObject cursor_effect3;

    public GameObject UI;

    public GameObject SkillBox;

    public List<Transform> DisabledTiles;
    public List<GameObject> EnabledTiles;
    public List<GameObject> Characters;


    // Start is called before the first frame update
    void Start()
    {
        //Creating Lists
        EnabledTiles = new List<GameObject>();
        DisabledTiles = new List<Transform>();
        Characters = new List<GameObject>();
        game_start = false;
        on = true;
        
        current_direction = 0;
        next_pos = Vector3.left;
        destination = transform.position;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            calculate();
            move();
            if (player_interaction())
            {
                UI.GetComponent<UI>().create_skill_buttons = true;
            }
        }

        if (game_start)
        {
            turn_start();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            game_start = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            turn_end();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (attack_directing == true)
            {
                skill_go_back();
            }
        }
        on_tile_click();
    }

    //Caculating position in relation to objects in level
    void calculate()
    {
        if (calculate_tiles == true)
        {
            //Distance from disabled tiles
            foreach(Transform DisabledTile in DisabledTiles)
            {
                float distance_tile = Vector3.Distance(destination, DisabledTile.position);
                if (distance_tile < 0.9f)
                {
                    destination = destination + next_pos;
                }
            }
            
        }
    }

    //Movement Functions
    void move()
    {
        float distance_3 = Vector3.Distance(transform.position, destination);
        if (distance_3 < 0.09f)
        {
            if (!melee_movement)
            {
                cursor_sprite.GetComponent<Renderer>().enabled = true;
                cursor_effect.GetComponent<Renderer>().enabled = true;
                cursor_effect1.GetComponent<Renderer>().enabled = true;
                cursor_effect2.GetComponent<Renderer>().enabled = true;
                cursor_effect3.GetComponent<Renderer>().enabled = true;
            }
            gravity();
        }
        else
        {
            if (!melee_movement)
            {
                cursor_sprite.GetComponent<Renderer>().enabled = false;
                cursor_effect.GetComponent<Renderer>().enabled = false;
                cursor_effect1.GetComponent<Renderer>().enabled = false;
                cursor_effect2.GetComponent<Renderer>().enabled = false;
                cursor_effect3.GetComponent<Renderer>().enabled = false;
            }
        }

        if (transform.position == destination)
        {
            calculate_tiles = false;
            GameObject[] skill_boxes = GameObject.FindGameObjectsWithTag("SkillBox");
            foreach(GameObject SkillBox in skill_boxes)
            {
                SkillBox.GetComponent<SkillBox>().gravity_on = true;
            }
            move_cd -= 10f * Time.deltaTime;
            if (move_cd <= 0) move_cd = 0;
        }
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        Vector3 skill_direction = new Vector3(0,0,0);
        
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow))
            {
                
            }
            else 
            {
                move_cd = 0;
                move_pressed = false;
            }
        }
        if (move_cd == 0)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (!melee_movement)
                {
                    next_pos = Vector3.right;
                    current_direction = 1;
                    can_move = true;
                    move_cd = 1;
                }
                if (melee_movement && right && current_direction != 1)
                {
                    skill_direction = Characters[0].GetComponent<CharacterBehaviour>().transform.position + Vector3.right - new Vector3(0, 0.25f, 0);
                    current_direction = 1;
                    can_move = true;
                }
                move_pressed = true;
                calculate_tiles = true;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (!melee_movement)
                {
                    next_pos = Vector3.back;
                    current_direction = 2;
                    can_move = true;
                    move_cd = 1;
                }
                if (melee_movement && down && current_direction != 2)
                {
                    skill_direction = Characters[0].GetComponent<CharacterBehaviour>().transform.position + Vector3.back - new Vector3(0, 0.25f, 0);
                    current_direction = 2;
                    can_move = true;
                }
                move_pressed = true;
                calculate_tiles = true;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            { 
                if (!melee_movement)
                {
                    next_pos = Vector3.left;
                    current_direction = 3;
                    can_move = true;
                    move_cd = 1;
                }
                if (melee_movement && left && current_direction != 3)
                {
                    skill_direction = Characters[0].GetComponent<CharacterBehaviour>().transform.position + Vector3.left - new Vector3(0, 0.25f, 0);
                    current_direction = 3;
                    can_move = true;
                }
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    move_cd = 0;
                }
                move_pressed = true;
                calculate_tiles = true;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (!melee_movement)
                {
                    next_pos = Vector3.forward;
                    current_direction = 4;
                    can_move = true;
                    move_cd = 1;
                }
                if (melee_movement && up && current_direction != 4)
                {
                    skill_direction = Characters[0].GetComponent<CharacterBehaviour>().transform.position + Vector3.forward - new Vector3(0, 0.25f, 0);
                    current_direction = 4;
                    can_move = true;
                }
                move_pressed = true;
                calculate_tiles = true;
            }
        }
        
        

        if(Vector3.Distance(destination,transform.position)<=0.000001f)
        {
            switch(current_direction)
            {
                case 1: //player_sprite.GetComponent<SpriteRenderer>().sprite = player_sprite.GetComponent<FourSideSprites>().right_sprite;
                ray_dir = new Vector3(90, 0, 0);
                break;
                case 2: //player_sprite.GetComponent<SpriteRenderer>().sprite = player_sprite.GetComponent<FourSideSprites>().down_sprite;
                ray_dir = new Vector3(0, 0, -90);
                break;
                case 3: //player_sprite.GetComponent<SpriteRenderer>().sprite = player_sprite.GetComponent<FourSideSprites>().left_sprite;
                ray_dir = new Vector3(-90, 0, 0);
                break;
                case 4: //player_sprite.GetComponent<SpriteRenderer>().sprite = player_sprite.GetComponent<FourSideSprites>().up_sprite;
                ray_dir = new Vector3(0, 0, 90);
                break;
            }
            
            if (can_move && wall())
            {
                if (!melee_movement)
                {
                    destination = transform.position + next_pos;
                }
                if (melee_movement)
                {
                    destination = skill_direction;
                }
                can_move = false;
            }
        }
    }

    public void goto_character(GameObject character)
    {
        transform.position = character.transform.position - new Vector3(0, 0.25f, 0);
        destination = character.transform.position - new Vector3(0, 0.25f, 0);
    }

    //Checking for ground
    public LayerMask mask;
    void gravity()
    {
        Transform upmost_tile = transform;
        if(Physics.RaycastAll(transform.position - new Vector3(0, 0.75f, 0), Vector3.up, Mathf.Infinity) != null)
        {
            RaycastHit[] tiles = Physics.RaycastAll(transform.position - new Vector3(0, 0.75f, 0), Vector3.up, Mathf.Infinity);
            float height = -0.5f;

            foreach(RaycastHit tile in tiles)
            {
                if (tile.collider.tag == "Ground")
                {
                    if (tile.collider.transform.position.y > height)
                    {
                        Debug.Log(height);
                        upmost_tile = tile.collider.transform;
                        height = upmost_tile.position.y;
                        Debug.Log(height);
                    }
                }
            }
            if (upmost_tile != transform)
            {
                destination.y += upmost_tile.transform.position.y - transform.position.y + 0.5f;
            }
        }

        RaycastHit hit;
        if (upmost_tile == transform)
        {
            if(Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask))
            {
                if (hit.collider.tag == "Ground")
                {
                    if (transform.position.y != hit.collider.transform.position.y - 0.5f)
                    {
                        destination.y -= transform.position.y - hit.collider.transform.position.y -0.5f; 
                    }
                }
            }
        }
    }

    //Hitting walls
    bool wall()
    {
        Ray myRay2 = new Ray(transform.position + new Vector3(0, 0.5f, 0), ray_dir);
        RaycastHit hit2;
        float distance_4;

        Debug.DrawRay(myRay2.origin, myRay2.direction, Color.red);

        if(Physics.Raycast(myRay2, out hit2, ray_lenght2))
        {
            if(hit2.collider.tag == "Wall")
            {
                distance_4 = Vector3.Distance(transform.position, hit2.collider.GetComponent<Transform>().position);
                if(distance_4 <= 1.5f)
                {
                    return false;
                }
            }
            
        }
        return true;
    }

    //Interactions
    bool player_interaction()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (GameObject Character in Characters)
            {
                float distance_tile = Vector3.Distance(transform.position, Character.transform.position);
                if (distance_tile < 0.01f && Character.GetComponent<CharacterBehaviour>().npc == false)
                {
                    UI.GetComponent<UI>().character_highlighted = Character;
                    return true;
                }
            }
        }
        return false;
    }

    public void on_tile_click()
    {
        if (Input.GetKeyUp(KeyCode.A) && Characters[0].GetComponent<CharacterBehaviour>().skill_turn == false)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1))
            {
                if (hit.collider.tag == "Ground" || hit.collider.tag == "EffectTile")
                {
                    EnabledTile t = null;
                    if (hit.collider.tag == "EffectTile")
                    {
                        t = hit.collider.GetComponentInParent<EnabledTile>();
                    }
                    else t = hit.collider.GetComponent<EnabledTile>();

                    if (t.selectable)
                    {
                        clicked_tile = t;
                    }
                }
            }
        }
    }

    //Sorting turn function
    private int sort_turn(GameObject a, GameObject b)
    {
        return b.GetComponent<CharacterBehaviour>().agility.CompareTo(a.GetComponent<CharacterBehaviour>().agility);
    }
    void assign_teams_and_positions()
    {
        //Assigning teams
        if (Characters[0].GetComponent<Collider>().tag == "Enemy")
        {
            foreach(GameObject Character in Characters)
            {
                if (Character.GetComponent<Collider>().tag == "Enemy") Character.GetComponent<Collider>().tag = "Team";
                else if (Character.GetComponent<Collider>().tag == "Team") Character.GetComponent<Collider>().tag = "Enemy";
                else if (Character.GetComponent<Collider>().tag == "Self") Character.GetComponent<Collider>().tag = "Enemy";
            }
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("Self"))
            {
                GameObject.FindGameObjectWithTag("Self").GetComponent<Collider>().tag = "Team";
            }
        }
        //Defining current tile
        foreach(GameObject Character in Characters)
        {
            Character.GetComponent<CharacterBehaviour>().get_current_tile(false);
        }
    }
    void turn_start()
    {
        if (sorted_by_speed == false)
        {
            Characters.Sort(sort_turn);
            sorted_by_speed = true;
        }
        Characters[Characters.Count - 1].GetComponent<CharacterBehaviour>().turn = false;
        goto_character(Characters[0]);
        assign_teams_and_positions();
        Characters[0].GetComponent<Collider>().tag = "Self";
        Characters[0].GetComponent<CharacterBehaviour>().turn = true;
        UI.GetComponent<UI>().create_portrait_turn(Characters);
        game_start = false;
    }

    public void turn_end()
    {
        //Killing all tweens
        DOTween.KillAll(true);
        //Resetting player skill turn
        Characters[0].GetComponent<CharacterBehaviour>().skill_turn = false;
        Characters[0].GetComponent<CharacterBehaviour>().chosing_skill = false;
        //Updating player gems
        Characters[0].GetComponent<CharacterBehaviour>().update_gems();
        //Updating turn list
        GameObject first_character = Characters[0];
        Characters.RemoveAt(0);
        Characters.Add(first_character);
        //Destroying all buttons
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("FirstLayerButton");
        foreach(GameObject SkillGemButton in buttons)
        {
            Destroy(SkillGemButton);
        }
        //Attackable false
        foreach(GameObject EnabledTile in EnabledTiles)
        {
            EnabledTile.GetComponent<EnabledTile>().attackable = false;
        }
        //Destroying all skill boxes
        GameObject[] boxes = GameObject.FindGameObjectsWithTag("SkillBox");
        foreach(GameObject SkillBox in boxes)
        {
            Destroy(SkillBox);
        }
        //Destroying killed portraits
        GameObject[] portraits = GameObject.FindGameObjectsWithTag("Portrait");
        foreach(GameObject portrait in portraits)
        {
            if (portrait.GetComponent<ScrollPortrait>().destroy == true)
            {
                UI.GetComponent<UI>().portraits.Remove(portrait);
                Destroy(portrait);

            } 
            
        }
        //Setting normal cursor movement
        on = true;
        melee_movement = false;
        attack_directing = false;
        
        turn_start();
        
        ep_cost_informed = false;
    }

    public void create_skill_box(int active_skill)
    {
        up = false;
        down = false;
        right = false;
        left = false;
        Renderer[] components = GetComponentsInChildren<Renderer>();
        foreach(Renderer component in components)
        {
            component.enabled = false;
        }
        attack_directing = true;
        calculate_tiles = true;
        CharacterBehaviour skill_user = Characters[0].GetComponent<CharacterBehaviour>();
        Skill used_skill = skill_user.Skills[active_skill];
        RaycastHit hit;
        if (used_skill.Type == Skill.skill_type.Melee)
        {
            transform.position = skill_user.transform.position - new Vector3(0, 0.25f, 0) + Vector3.forward;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity) || Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Ground") up = true;
            }
            transform.position = skill_user.transform.position - new Vector3(0, 0.25f, 0) + Vector3.back;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity) || Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Ground") down = true;
            }
            transform.position = skill_user.transform.position - new Vector3(0, 0.25f, 0) + Vector3.right;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity) || Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Ground") right = true;
            }
            transform.position = skill_user.transform.position - new Vector3(0, 0.25f, 0) + Vector3.left;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity) || Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Ground") left = true;
            }
            if (up)
            {
                destination = skill_user.transform.position + Vector3.forward - new Vector3(0, 0.25f, 0);
                current_direction = 4;
            }
            if (!up && !left && !right)
            {
                destination = skill_user.transform.position + Vector3.back - new Vector3(0, 0.25f, 0);
                current_direction = 2;
            }
            if (!up && !left)
            {
                destination = skill_user.transform.position + Vector3.right - new Vector3(0, 0.25f, 0);
                current_direction = 1;
            }
            if (!up)
            {
                destination = skill_user.transform.position + Vector3.left - new Vector3(0, 0.25f, 0);
                current_direction = 3;
            }
            
            melee_movement = true;

            switch(used_skill.Format)
            {
                case Skill.skill_format.Square: 
                switch(used_skill.FormatSize)
                {
                    case 1: 
                    GameObject skill_box = Instantiate(SkillBox) as GameObject;
                    SkillBox new_skill_box = skill_box.GetComponent<SkillBox>();
                    new_skill_box.transform.SetParent(transform, false);
                    new_skill_box.skill_name = used_skill.SkillName;
                    new_skill_box.skill_gem = used_skill.SkillOrGem;
                    new_skill_box.power = used_skill.Power;
                    new_skill_box.range = used_skill.Range;
                    new_skill_box.height = used_skill.Height;
                    new_skill_box.damage = used_skill.Damage;
                    new_skill_box.ep_cost =used_skill.ep_cost;
                    new_skill_box.skill_effect1 = used_skill.Effect1;
                    new_skill_box.skill_effect2 = used_skill.Effect2;
                    new_skill_box.skill_effect3 = used_skill.Effect3;
                    new_skill_box.skill_damage = used_skill.DamageType;
                    new_skill_box.character_highlighted = Characters[0];
                    new_skill_box.check_for_enemies = true;
                    break;
                }
                break;
                case Skill.skill_format.Horizontal:
                switch(used_skill.FormatSize)
                {
                    case 1:
                    for(int i = 0; i < 3; i++)
                    {
                        GameObject skill_box = Instantiate(SkillBox) as GameObject;
                        SkillBox new_skill_box = skill_box.GetComponent<SkillBox>();
                        new_skill_box.transform.SetParent(transform, false);
                        new_skill_box.skill_box_number = i;
                        new_skill_box.skill_name = used_skill.SkillName;
                        new_skill_box.skill_gem = used_skill.SkillOrGem;
                        new_skill_box.power = used_skill.Power;
                        new_skill_box.range = used_skill.Range;
                        new_skill_box.height = used_skill.Height;
                        new_skill_box.damage = used_skill.Damage;
                        new_skill_box.ep_cost = used_skill.ep_cost;
                        new_skill_box.skill_effect1 = used_skill.Effect1;
                        new_skill_box.skill_effect2 = used_skill.Effect2;
                        new_skill_box.skill_effect3 = used_skill.Effect3;
                        new_skill_box.skill_damage = used_skill.DamageType;
                        new_skill_box.skill_format = used_skill.Format;
                        new_skill_box.character_highlighted = Characters[0];
                        new_skill_box.check_for_enemies = true;
                    }
                    break;
                }
                break;
            }
        }
        if (used_skill.Type == Skill.skill_type.Ranged)
        {

        }
    }

    public void skill_go_back()
    {
        Renderer[] components = GetComponentsInChildren<Renderer>();
        foreach(Renderer component in components)
        {
            component.enabled = true;
        }
        GetComponent<Renderer>().enabled = false;
        attack_directing = false;
        melee_movement = false;
        on = false;
        GameObject[] skill_boxes = GameObject.FindGameObjectsWithTag("SkillBox");
        foreach(GameObject SkillBox in skill_boxes)
        {
            Destroy(SkillBox);
        }
        foreach(GameObject Character in Characters)
        {
            Character.GetComponent<CharacterBehaviour>().on_lock = false;
        }
        goto_character(Characters[0]);
        UI.GetComponent<UI>().last_button_clicked.GetComponent<SkillGemButton>().select = true;
    }

    public void ep_drain(int ep_cost)
    {
        if (!ep_cost_informed)
        {
            Characters[0].GetComponent<CharacterBehaviour>().ep -= ep_cost;
            ep_cost_informed = true;
        }
        
    }
}
