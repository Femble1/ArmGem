using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsingLight : MonoBehaviour
{
    Light lighting;
    public float pulsing_speed = 0.05f;
    public float minimun_light = 3f;
    public float maximum_light = 7f;
    bool decrease = true;
    // Start is called before the first frame update
    void Start()
    {
        lighting = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        light_pulse();
    }

    void light_pulse()
    {
        if (lighting.intensity >= maximum_light)
        {
            decrease = true;
        }
        if (lighting.intensity <= minimun_light)
        {
            decrease = false;
        }

        switch (decrease)
        {
            case true: lighting.intensity -= pulsing_speed; break;
            case false: lighting.intensity += pulsing_speed; break;
        }
    }
}
