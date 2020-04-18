using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineWindow : MonoBehaviour
{
    public Machine machine;
    public GameObject displayedObject { get; private set; }

    [Range(0.1f, 2.5f)]
    public float enterTime = 1.0f;

    [Range(0.1f, 2.5f)]
    public float exitTime = 1.0f;

    private Transform startPosition;
    private Transform displayPosition;
    private Transform endPosition;

    public bool locked = false;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.Find("Start Position");
        displayPosition = transform.Find("Display Position");
        endPosition = transform.Find("End Position");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayObject(GameObject target)
    {
        Debug.Log("DisplayObject", target);
        if (locked)
        {
            return;
        }
        displayedObject = target;
        StartCoroutine(AnimateObject(startPosition, displayPosition, enterTime, true));
    }

    public void RemoveObjectFromDisplay()
    {
        if(locked || !displayedObject)
        {
            return;
        }
        StartCoroutine(AnimateObject(displayPosition, endPosition, exitTime, false));
    }

    IEnumerator AnimateObject(Transform originalPosition, Transform finalPosition, float duration, bool displaying)
    {
        locked = true;
        displayedObject.transform.rotation = displayPosition.rotation;
        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            displayedObject.transform.position = originalPosition.position;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / duration;

                displayedObject.transform.position = Vector3.Lerp(originalPosition.position, finalPosition.position, progress);
                yield return null;
            }
        }
        if(displaying)
        {
            machine.ObjectWasDisplayed(displayedObject);
        } else
        {
            machine.ObjectWasRemovedFromDisplay(displayedObject);
            displayedObject = null;
        }
        locked = false;
    }
}
