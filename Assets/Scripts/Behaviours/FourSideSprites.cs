using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourSideSprites : MonoBehaviour
{
    public int direction;
    public Sprite right_sprite;
    public Sprite down_sprite;
    public Sprite left_sprite;
    public Sprite up_sprite;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = 2;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = 3;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = 4;
        }
        update_direction();
    }

    void update_direction()
    {
        switch(direction)
        {
            case 1: GetComponent<SpriteRenderer>().sprite = right_sprite; break;
            case 2: GetComponent<SpriteRenderer>().sprite = down_sprite; break;
            case 3: GetComponent<SpriteRenderer>().sprite = left_sprite; break;
            case 4: GetComponent<SpriteRenderer>().sprite = up_sprite; break;
        }
    }
}
