using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FFTAnimator : MonoBehaviour
{
    CharacterBehaviour origin_body;
    Animator animator;
    Color tmp;
    Material material;
    public bool appear;
    int last_looked_direction;
    bool get_last_looked_direction;
    // Start is called before the first frame update
    void Start()
    {
        origin_body = GetComponentInParent<CharacterBehaviour>();
        animator = GetComponent<Animator>();
        material = GetComponent<SpriteRenderer>().material;
        appear = true;
    }

    // Update is called once per frame
    void Update()
    {
        change_sprite_direction();
        alpha_control();
    }

    void change_sprite_direction()
    {
        if (!origin_body.performing_attack)
        {
            animator.SetInteger("Direction", origin_body.looking_direction); 
        }
    }
    void alpha_control()
    {
        if (tmp.a < 1)
        {
            material = Resources.Load<Material>("Materials/SpriteTransparent");
        }
        else
        {
            material = Resources.Load<Material>("Materials/SpriteOpaque");
        }
        if (appear)
        {
            tmp = GetComponent<SpriteRenderer>().color;
            tmp.a += 0.03f;
            if (tmp.a >= 1) tmp.a = 1;
            GetComponent<SpriteRenderer>().color = tmp;

            if (get_last_looked_direction)
            {
                switch(animator.GetInteger("Direction"))
                {
                    case 1: animator.SetInteger("Direction", 4); break;
                    case 2: animator.SetInteger("Direction", 3); break; 
                    case 3: break;
                    case 4: break;
                }
                get_last_looked_direction = false;
            }
        }
        else
        {
            tmp = GetComponent<SpriteRenderer>().color;
            tmp.a -= 0.04f;
            if (tmp.a <= 0) tmp.a = 0;
            GetComponent<SpriteRenderer>().color = tmp;

            if (!get_last_looked_direction)
            {
                last_looked_direction = origin_body.looking_direction;
                switch(animator.GetInteger("Direction"))
                {
                    case 1: break;
                    case 2: break; 
                    case 3: animator.SetInteger("Direction", 2); break;
                    case 4: animator.SetInteger("Direction", 1); break;
                }
                get_last_looked_direction = true;
            }
        }

        if (tmp.a < 0.04)
        {
            animator.enabled = false;
        }
        else animator.enabled = true;
    }
}
