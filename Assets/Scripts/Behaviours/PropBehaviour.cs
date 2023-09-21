using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropBehaviour : MonoBehaviour
{
    public bool fade;
    Color tmp;
    Material material;
    void Start()
    {
        tmp = GetComponent<SpriteRenderer>().color;
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (tmp.a < 0.99f)
        {
            GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Materials/Sprites/SpriteTransparent");
        }
        else
        {
            GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Materials/Sprites/SpriteOpaque");
        }
        fading();
    }

    void fading()
    {
        if (!fade)
        {
            tmp.a += 0.1f;
            if (tmp.a >= 1f) tmp.a = 1f; 
        }
        else
        {
            tmp.a -= 0.1f;
            if (tmp.a <= 0.1f) tmp.a = 0.1f;
            fade = false;
        }
        GetComponent<SpriteRenderer>().color = tmp;
    }
}
