using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisabledTile : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player.GetComponent<CursorBehaviour>().DisabledTiles.Add(transform);
        //GetComponent<Renderer>().enabled = false;
    }
}
