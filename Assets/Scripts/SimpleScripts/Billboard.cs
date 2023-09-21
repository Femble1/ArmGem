using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("IsoCam").GetComponent<Transform>();
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam);
    }
}
