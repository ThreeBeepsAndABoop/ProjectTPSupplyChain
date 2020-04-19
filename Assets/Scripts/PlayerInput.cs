using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInput : MonoBehaviour
{
    private Camera m_Camera;
    private TextMeshProUGUI m_crosshairText;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
        m_crosshairText = GameObject.Find("CROSSHAIR_TEXT").GetComponent<TextMeshProUGUI>();
    }

    LayerMask raycastMask = ~(1 << 2);

    private float m_ScrollWheelSensitivity = 0.1f;
    private float m_AccumlatedScrollWheelDelta;

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

        float scrollDelta = Input.mouseScrollDelta.y;
        if(scrollDelta == 0)
        {
            m_AccumlatedScrollWheelDelta = 0;
        } else
        {
            m_AccumlatedScrollWheelDelta += scrollDelta;
        }

        if(m_AccumlatedScrollWheelDelta > m_ScrollWheelSensitivity)
        {
            GameManager.Instance.PlayerInventory.SelectNextItem();
            m_AccumlatedScrollWheelDelta = 0;
        }
        else if (m_AccumlatedScrollWheelDelta < -m_ScrollWheelSensitivity)
        {
            GameManager.Instance.PlayerInventory.SelectPreviousItem();
            m_AccumlatedScrollWheelDelta = 0;
        }

        RaycastHit hoverTextHitInfo;
        m_crosshairText.text = "";
        if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hoverTextHitInfo, range, raycastMask))
        {
            Grabbable grabbable = hoverTextHitInfo.collider.gameObject.GetComponent<Grabbable>();
            if (grabbable != null)
            {
                m_crosshairText.text = grabbable.name;
            }
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
                    Debug.Log("Interact with " + hitInfo.rigidbody.gameObject);
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