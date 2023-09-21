using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTile : MonoBehaviour
{
    Vector3 initial_position;
    float max_offset;
    private float offset_value = 0.045f;
    private float movespeed = 0.004f;
    // Start is called before the first frame update
    void Start()
    {
        initial_position = transform.position;
        max_offset = initial_position.y - offset_value;
    }

    // Update is called once per frame
    void Update()
    {
        simple_movement();
    }

    void simple_movement()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.up, Color.green);
        if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 0.4f))
        {
            if (hit.collider.tag == "Cursor")
            {
                transform.position -= new Vector3 (0, movespeed, 0);
                if (transform.position.y <= max_offset) 
                {
                    transform.position = initial_position - new Vector3(0, offset_value, 0);
                }
            }
        }
        else
        {
            transform.position += new Vector3 (0, movespeed, 0);
            if (transform.position.y >= initial_position.y) transform.position = initial_position;
        }
    }
}
