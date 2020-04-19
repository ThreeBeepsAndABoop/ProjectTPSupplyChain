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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.Instance.PlayerInventory.SelectItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameManager.Instance.PlayerInventory.SelectItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameManager.Instance.PlayerInventory.SelectItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameManager.Instance.PlayerInventory.SelectItem(3);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Grabbable selectedItem = GameManager.Instance.PlayerInventory.GetSelectedItem();
            if (selectedItem != null)
            {
                GameManager.Instance.PlayerInventory.Drop(selectedItem);
            }
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hitInfo, range, raycastMask))
            {
                Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    Debug.Log("Interact");
                    interactable.Interact(hitInfo);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
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

        //if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.F))
        //{
        //    RaycastHit hitInfo;
        //    if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hitInfo, range, raycastMask))
        //    {
        //        Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
        //        if (interactable != null)
        //        {
        //            Debug.Log("InteractTertiary");
        //            interactable.InteractTertiary(hitInfo);
        //        }
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        handleGameInteraction();
    }
}