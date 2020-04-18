using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    void OnTriggerEnter()
    {
        Destroy(transform.Find("Door").gameObject);
    }
}
