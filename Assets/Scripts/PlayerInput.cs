using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInput : MonoBehaviour
{
    private Camera m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
    }

    LayerMask raycastMask = ~(1 << 2);

    const int range = 4;
    private void handleGameInteraction()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance.GrabIt.GrabbedObject != null)
            {
                GameManager.Instance.GrabIt.ReleaseGrabbed();
            }
            else
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hitInfo, range, raycastMask))
                {
                    Debug.Log("Hit " + hitInfo.collider.gameObject);
                    Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        Debug.Log("Interact");
                        interactable.Interact(hitInfo);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Q))
        {
            if (GameManager.Instance.GrabIt.GrabbedObject != null)
            {
                GameManager.Instance.GrabIt.YeetGrabbed();
            }
            else
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hitInfo, range, raycastMask))
                {
                    Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        Debug.Log("InteractSecondary");
                        interactable.InteractSecondary(hitInfo);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hitInfo, range, raycastMask))
            {
                Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    Debug.Log("InteractTertiary");
                    interactable.InteractTertiary(hitInfo);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleGameInteraction();
    }
}