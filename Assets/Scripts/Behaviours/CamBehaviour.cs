using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBehaviour : MonoBehaviour
{
    public GameObject target;
    public float smoothing = 0.125f;
    public Vector3 offset;

    void Update()
    {
        check_for_prop();
    }
    void LateUpdate()
    {
        Vector3 desired_position = target.transform.position + offset;
        Vector3 smoothed_position = Vector3.Lerp(transform.position, desired_position, smoothing);
        transform.position = smoothed_position;
        
    }

    public void change_target(GameObject new_object)
    {
        target = new_object;
    }

    void check_for_prop()
    {
        Debug.DrawRay(transform.position, FindObjectOfType<CursorBehaviour>().transform.position - transform.position, Color.blue);
        RaycastHit[] hits = Physics.RaycastAll(transform.position, FindObjectOfType<CursorBehaviour>().transform.position - transform.position, 50f);
        foreach(RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.tag == "Prop")
            {
                hit.collider.GetComponent<PropBehaviour>().fade = true;
            }
        }
        
    }
}
