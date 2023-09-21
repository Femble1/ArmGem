using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemPortrait : MonoBehaviour
{
    public string gem_type;
    // Start is called before the first frame update
    void Start()
    {
        switch(this.gameObject.name)
        {
            case "WeaponGem": gem_type = "Weapon"; break;
            case "ArmorGem": gem_type = "Armor"; break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
