using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitHP : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void update_health_bar(float hp_percentage)
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector3(-94.7f * (1 - hp_percentage), 0, 0);
    }
}
