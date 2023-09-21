using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnabledTile : MonoBehaviour
{
    public GameObject cursor;
    public GameObject character_highlighted;
    public GameObject effect_tile;
    public bool effect_created;
    public bool walkable = true;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;
    public bool visited = false;
    public bool passable = false;
    public EnabledTile parent = null;
    public int distance = 0;
    public List<EnabledTile> adjacency_list = new List<EnabledTile>();
    // For NPC Movement & Attack
    public float f = 0;
    public float g = 0;
    public float h = 0;
    public bool attackable;
    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.Find("Cursor");
        cursor.GetComponent<CursorBehaviour>().EnabledTiles.Add(gameObject);
        effect_tile = Resources.Load<GameObject>("Prefabs/MapManager/EffectTile");
    }
    void Update()
    {
        create_effect();
    }

    public void reset()
    {
        adjacency_list.Clear();

        current = false;
        target = false;
        selectable = false;

        visited = false;
        parent = null;
        distance = 0;
    }

    public void find_neighbours(float jump_height, EnabledTile target)
    {
        reset();

        check_tile(Vector3.forward, jump_height, target);
        check_tile(-Vector3.forward, jump_height, target);
        check_tile(Vector3.right, jump_height, target);
        check_tile(-Vector3.right, jump_height, target);
    }

    public void check_tile(Vector3 direction, float jump_height, EnabledTile target)
    {
        Vector3 half_extents = new Vector3(0.25f, jump_height / 2f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, half_extents);

        foreach(Collider item in colliders)
        {
            EnabledTile tile = item.GetComponent<EnabledTile>();
            if (tile != null && tile.walkable)
            {
                RaycastHit hit;
                if (Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                {
                    switch(hit.collider.tag)
                    {
                        case "Enemy": tile.attackable = true; break;
                        case "Ground": break;
                        case "Self": break;
                        case "Team": tile.passable = true; adjacency_list.Add(tile); break;
                        case "Untagged": adjacency_list.Add(tile); break;
                        case null: adjacency_list.Add(tile); break;
                    }
                    if (tile == target)
                    {
                        adjacency_list.Add(tile);
                    }
                }
                else adjacency_list.Add(tile);
            }
        }
    }

    public void create_effect()
    {
        if (!effect_created)
        {
            if (current)
            {
                GameObject effect = Instantiate(effect_tile) as GameObject;
                effect.transform.SetParent(transform, false);
                effect.transform.position += new Vector3 (0, 0.3f, 0);
                
                effect_created = true;
            }
            else if (target)
            {
                GetComponent<Renderer>().material.color = Color.cyan;
                effect_created = true;
            }
            else if (selectable)
            {
                GameObject effect = Instantiate(effect_tile) as GameObject;
                effect.transform.SetParent(transform, false);
                effect.transform.position += new Vector3 (0, 0.3f, 0);
                
                effect_created = true;
            }
            else 
            {
                
            }
        }
        else if (!current && !target && !selectable)
        {
            if (transform.childCount > 0)
            {
                Transform child = transform.GetChild(0);
                Destroy(child.gameObject);
            }
            effect_created = false;
        }
    }
}
