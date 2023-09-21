using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    List<EnabledTile> selectable_tiles = new List<EnabledTile>();
    GameObject[] tiles;
    Stack<EnabledTile> path = new Stack<EnabledTile>();
    public EnabledTile current_tile;
    EnabledTile last_current_tile;
    public EnabledTile new_current_tile;
    public bool moving = false;
    public int move = 0;
    public int move_speed = 3;
    public float jump_height = 1f;
    bool falling_down = false;
    bool jumping_up = false;
    bool moving_edge = false;
    Vector3 jump_target;
    public float jump_velocity = 5f;
    float half_height = 0;
    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();
    //NPC Movement
    public EnabledTile actual_target_tile;
    protected void init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Ground");
        move = GetComponent<CharacterBehaviour>().movement;
        half_height = GetComponent<Collider>().bounds.extents.y;
    }   

    public void get_current_tile(bool mark_tile)
    {
        current_tile = get_target_tile(gameObject);
        if(mark_tile)
        {
            current_tile.current = true;
        }
    }

    public EnabledTile get_target_tile(GameObject target)
    {
        RaycastHit hit;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            if (hit.collider.tag == "Ground" || hit.collider.tag == "EffectTile")
            {
                if (hit.collider.tag == "Ground")
                {
                    new_current_tile = hit.collider.GetComponent<EnabledTile>();
                }
                else new_current_tile = hit.collider.GetComponentInParent<EnabledTile>();
            }
        }
        return new_current_tile;
    }

    public void compute_adjacency_lists(float jump_height, EnabledTile target)
    {
        foreach (GameObject EnabledTile in tiles)
        {
            EnabledTile t = EnabledTile.GetComponent<EnabledTile>();
            t.find_neighbours(jump_height, target);
        }
    }

    public void find_selectable_tiles(int move)
    {
        compute_adjacency_lists(jump_height, null);
        get_current_tile(true);

        Queue<EnabledTile> process = new Queue<EnabledTile>();

        process.Enqueue(current_tile);
        current_tile.visited = true;

        while(process.Count > 0)
        {
            EnabledTile t = process.Dequeue();
            if (!t.passable)
            {   
                selectable_tiles.Add(t);
                t.selectable = true;
            }
            t.passable = false;
            if (t.distance < move)
            {
                foreach(EnabledTile EnabledTile in t.adjacency_list)
                {
                    if (!EnabledTile.visited)
                    {
                        EnabledTile.parent = t;
                        EnabledTile.visited = true;
                        EnabledTile.distance = 1 + t.distance;
                        process.Enqueue(EnabledTile);
                    }
                }
            }
        }
    }

    public void remove_selectable_tiles()
    {
        if (current_tile != null)
        {
            current_tile.current = false;
            current_tile = null;
        }

        foreach(EnabledTile EnabledTile in selectable_tiles)
        {
            EnabledTile.reset();
        }

        selectable_tiles.Clear();
    }

    public void move_to_tile(EnabledTile tile)
    {
        last_current_tile = current_tile;
        path.Clear();
        tile.target = true;
        moving = true;

        EnabledTile next = tile;
        while(next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    public void go_back(GameObject character)
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("FirstLayerButton");
        GameObject cursor = GameObject.Find("Cursor");

        current_tile = last_current_tile;
        foreach(GameObject SkillGemButton in buttons)
        {
            Destroy(SkillGemButton);
        }

        character.GetComponent<CharacterBehaviour>().skill_turn = false;
        character.GetComponent<CharacterBehaviour>().chosing_skill = false;
        var position_x = transform.position.x;
        transform.position = last_current_tile.transform.position + new Vector3(0, 0.75f, 0);
        cursor.GetComponent<CursorBehaviour>().goto_character(gameObject);
        cursor.GetComponent<CursorBehaviour>().on = true;
    }

    public void walk()
    {
        if (path.Count > 0)
        {
            EnabledTile t = path.Peek();
            Vector3 target = t.transform.position;

            target.y += half_height + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.08)
            {
                bool jump = target.y >= transform.position.y + 0.001 || target.y <= transform.position.y - 0.001;

                if (jump)
                {
                    jumping(target);
                }
                else
                {
                    calculate_heading(target);
                    set_horizontal_velocity();
                }
                

                transform.forward = heading;
                switch(transform.rotation.eulerAngles.y)
                {
                    case 270: GetComponent<CharacterBehaviour>().looking_direction = 1; break;
                    case 0: GetComponent<CharacterBehaviour>().looking_direction = 2; break;
                    case 90: GetComponent<CharacterBehaviour>().looking_direction = 3; break;
                    case 180: GetComponent<CharacterBehaviour>().looking_direction = 4; break;
                }
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                transform.position = target;
                path.Pop();
                falling_down = false;
                jumping_up = false;
                moving_edge = false;
            }
        }
        else
        {
            GameObject cursor = GameObject.Find("Cursor");
            cursor.GetComponent<CursorBehaviour>().clicked_tile = null;
            remove_selectable_tiles();
            moving = false;
            gameObject.GetComponent<CharacterBehaviour>().skill_turn = true;
            get_current_tile(false);
        }
    }

    void calculate_heading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    void set_horizontal_velocity()
    {
        velocity = heading * move_speed;
    }

    void jumping(Vector3 target)
    {
        if (falling_down)
        {
            fall_downard(target);
        }
        else if (jumping_up)
        {
            jump_upward(target);
        }
        else if (moving_edge)
        {
            move_to_edge();
        }
        else
        {
            prepare_jump(target);
        }
    }

    void prepare_jump(Vector3 target)
    {
        float target_y = target.y;
        
        target.y = transform.position.y;
        calculate_heading(target);

        if (transform.position.y > target_y)
        {
            falling_down = false;
            jumping_up = false;
            moving_edge = true;

            jump_target = transform.position + (target - transform.position) / 2.0f;

        }
        else
        {
            falling_down = false;
            jumping_up = true;
            moving_edge = false;

            velocity = heading * move_speed / 3.0f;

            float difference = target_y - transform.position.y;
            velocity.y = jump_velocity * (0.5f + difference / 2.0f);
        }
    }

    void fall_downard(Vector3 target)
    {
        velocity += (Physics.gravity/2 + Physics.gravity)* Time.deltaTime;
        
        if (transform.position.y <= target.y)
        {
            falling_down = false;
            jumping_up = false;
            moving_edge = false;

            Vector3 p = transform.position;
            p.y = target.y;
            transform.position = p;

            velocity = new Vector3();
        }
    }
    
    void jump_upward(Vector3 target)
    {
        velocity += (Physics.gravity/2 + Physics.gravity) * Time.deltaTime;

        if (transform.position.y > target.y)
        {
            jumping_up = false;
            falling_down = true;

        }
    }

    void move_to_edge()
    {
        if (Vector3.Distance(transform.position, jump_target) >= 0.05f)
        {
            set_horizontal_velocity();
        }
        else
        {
            moving_edge = false;
            falling_down = true;

            velocity /= 3.0f;
            velocity.y = 1.5f;
        }
    }

    //NPC Movement
    protected EnabledTile find_lowest_f(List<EnabledTile> list)
    {
        EnabledTile lowest = list[0];

        foreach(EnabledTile t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }

        list.Remove(lowest);

        return lowest;
    }
    protected EnabledTile find_end_tile(EnabledTile t)
    {
        Stack<EnabledTile> temp_path = new Stack<EnabledTile>();

        EnabledTile next = t.parent;
        while(next != null)
        {
            temp_path.Push(next);
            next = next.parent;
        }

        if (temp_path.Count <= move)
        {
            return t.parent;
        }

        EnabledTile end_tile = null;
        for (int i = 0; i <= move; i++)
        {
            end_tile = temp_path.Pop();
        }

        return end_tile;
    }
    protected void find_path(EnabledTile target)
    {
        compute_adjacency_lists(jump_height, target);
        get_current_tile(true);

        List<EnabledTile> open_list = new List<EnabledTile>();
        List<EnabledTile> closed_list = new List<EnabledTile>();

        open_list.Add(current_tile);
        current_tile.h = Vector3.Distance(current_tile.transform.position, target.transform.position);
        current_tile.f = current_tile.h;

        while(open_list.Count > 0)
        {
            EnabledTile t = find_lowest_f(open_list);

            closed_list.Add(t);

            if (t == target)
            {
                actual_target_tile = find_end_tile(t);  
                move_to_tile(actual_target_tile);
                return;
            }

            foreach(EnabledTile tile in t.adjacency_list)
            {
                if (closed_list.Contains(tile))
                {

                }
                else if (open_list.Contains(tile))
                {
                    float temp_g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

                    if (temp_g < tile.g)
                    {
                        tile.parent = t;

                        tile.g = temp_g;
                        tile.f = tile.g + tile.h;
                    }
                }
                else
                {
                    tile.parent = t;

                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.f = tile.g + tile.h;

                    open_list.Add(tile);
                }
            }
        }
    } 
}
