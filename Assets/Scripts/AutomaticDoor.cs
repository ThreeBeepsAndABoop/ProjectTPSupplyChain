﻿using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    private GameObject door;
    private MeshRenderer meshRenderer;
    private bool open;
    private float distanceLifted;

    void Awake()
    {
        door = transform.Find("Door").gameObject;
        meshRenderer = door.GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter()
    {
        open = true;
    }

    void OnTriggerExit()
    {
        open = false;
    }

    void Update()
    {
        if (open)
        {
            if (distanceLifted < meshRenderer.bounds.size.y)
            {
                door.transform.Translate(Vector3.up * Time.deltaTime * 1.5f);
                distanceLifted += Time.deltaTime * 1.5f;
            }
        } 
        else
        {
            if (distanceLifted > 0)
            {
                door.transform.Translate(Vector3.down * Time.deltaTime * 1.5f);
                distanceLifted -= Time.deltaTime * 1.5f;
            }
        }
    }
}
