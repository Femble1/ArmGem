using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpPortrait : MonoBehaviour
{
    public int ep_number;
    // Start is called before the first frame update
    void Start()
    {
        switch(this.gameObject.name)
        {
            case "Ep1": ep_number = 1; break;
            case "Ep2": ep_number = 2; break;
            case "Ep3": ep_number = 3; break;
            case "Ep4": ep_number = 4; break;
            case "Ep5": ep_number = 5; break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
