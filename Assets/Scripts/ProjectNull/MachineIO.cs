using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MachineIOType
{
    Input = 0,
    Output = 1,
    InputOutput = 2
}

public class MachineIO : MonoBehaviour
{
    public MachineIOType type;
    public Machine machine;

    [Range(0.1f, 10.0f)]
    public float processingTime = 1.0f;
    
    private BoxCollider voidRigidbody;

    private Vector3 axisOfTransit;
    private Vector3 axisOfOutput;
    private float distanceOfTransit = 2.0f;
    private float speed = 2.0f;
    private GameObject capturedObject;

    // Start is called before the first frame update
    void Start()
    {
        voidRigidbody = transform.Find("Void").GetComponent<BoxCollider>();
        axisOfTransit = -voidRigidbody.gameObject.transform.right;
        axisOfOutput = voidRigidbody.gameObject.transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (type == MachineIOType.Input || type == MachineIOType.InputOutput && machine.canAccept && colliders.Count > 0)
        {
            Debug.Log("Schloink");
            machine.canAccept = false;
            SchloinkObject(colliders[0].gameObject);
        }
    }

    public void YeetObject(GameObject obj)
    {
        if (type != MachineIOType.Input && type != MachineIOType.InputOutput)
        {
            return;
        }

        capturedObject = obj;
        capturedObject.GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(YeetAnimation(voidRigidbody.transform.position + axisOfOutput * (distanceOfTransit / 1.5f)));
    }

    public void SchloinkObject(GameObject obj)
    {
        if (type != MachineIOType.Input && type != MachineIOType.InputOutput)
        {
            return;
        }

        capturedObject = obj;
        capturedObject.GetComponent<Rigidbody>().isKinematic = true;
        if (capturedObject == GameManager.Instance.GrabIt.GrabbedObject)
        {
            GameManager.Instance.GrabIt.ReleaseGrabbed();
        }
        StartCoroutine(SchloinkAnimation(capturedObject.transform.position, capturedObject.transform.position + distanceOfTransit * axisOfTransit, distanceOfTransit / speed));
    }

    public void SchlorpObject(GameObject obj)
    {
        if(type != MachineIOType.Output && type != MachineIOType.InputOutput)
        {
            return;
        }
        capturedObject = obj;
        capturedObject.GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(SchlorpAnimation(voidRigidbody.transform.position - axisOfOutput * (distanceOfTransit / 1.5f), voidRigidbody.transform.position + axisOfOutput * (distanceOfTransit / 1.5f), distanceOfTransit / speed));
    }

    int yeetLayer;
    IEnumerator YeetAnimation(Vector3 originalPosition)
    {
        yeetLayer = capturedObject.layer;
        capturedObject.layer = 12;

        capturedObject.transform.position = originalPosition;
        Rigidbody rb = capturedObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(axisOfOutput * 25, ForceMode.Impulse);
        float waitDuration = 0.1f;
        if (waitDuration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + waitDuration;
            yield return null;
            while (Time.time < endTime)
            {
                yield return null;
            }
        }
        
        capturedObject.layer = yeetLayer;
        machine.ObjectWasYeeted(capturedObject);
        capturedObject = null;
    }

    IEnumerator SchloinkAnimation(Vector3 originalPosition, Vector3 finalPosition, float duration)
    {
        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            capturedObject.transform.position = originalPosition;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / duration;

                capturedObject.transform.position = Vector3.Lerp(originalPosition, finalPosition, progress);
                yield return null;
            }
        }

        if (capturedObject)
        {
            capturedObject.transform.position = finalPosition;
        }

        GameObject schloinked = capturedObject;
        capturedObject = null;
        machine.ObjectWasSchloinked(schloinked);
    }

    IEnumerator SchlorpAnimation(Vector3 originalPosition, Vector3 finalPosition, float duration)
    {
        GameManager.Instance.RequestPlayFactoryProcessingSound(machine, processingTime);

        float delayStartTime = Time.time;
        float delayEndTime = delayStartTime + processingTime;
        yield return null;
        while (Time.time < delayEndTime)
        {
            yield return null;
        }

        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            capturedObject.transform.position = originalPosition;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / duration;

                capturedObject.transform.position = Vector3.Lerp(originalPosition, finalPosition, progress);
                yield return null;
            }
        }

        if (capturedObject)
        {
            capturedObject.transform.position = finalPosition;
        }

        capturedObject.GetComponent<Rigidbody>().isKinematic = false;
        machine.ObjectWasSchlorped(capturedObject);
        capturedObject = null;
    }

    private List<Collider> colliders = new List<Collider>();
    public List<Collider> GetColliders() { return colliders; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Box>()) { return; }
        if (!colliders.Contains(other)) { colliders.Add(other); }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Box>()) { return; }
        colliders.Remove(other);
    }

}
