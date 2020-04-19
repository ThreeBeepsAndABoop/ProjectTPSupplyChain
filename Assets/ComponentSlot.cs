using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSlot : MonoBehaviour
{
    public MachineComponentType AcceptedType;
    public MachineController Machine;

    private Transform _componentTransform;
    private MachineComponent _heldComponent;

    // Start is called before the first frame update
    void Start()
    {
        Machine = GetComponentInParent<MachineController>();
        _componentTransform = transform.Find("ComponentLocation").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if(_heldComponent)
        {
            ReleaseComponent();
        }
        else
        {
            AcceptHeldComponentFromPlayer();
        }
    }

    public void AcceptHeldComponentFromPlayer()
    {
        Grabbable grabbedItem = GameManager.Instance.PlayerInventory.GetSelectedItem();
        if (grabbedItem != null)
        {
            MachineComponent machineComponent = grabbedItem.GetComponent<MachineComponent>();
            if (machineComponent && machineComponent.Type == AcceptedType)
            {
                _heldComponent = machineComponent;
                GameManager.Instance.PlayerInventory.Drop(grabbedItem);

                var rbs = grabbedItem.GetComponentsInChildren<Rigidbody>();
                foreach (var rb in rbs)
                {
                    rb.isKinematic = true;
                    rb.detectCollisions = false;
                }

                grabbedItem.transform.parent = _componentTransform;
                grabbedItem.transform.localPosition = Vector3.zero;
                grabbedItem.transform.localRotation = Quaternion.identity;

                _heldComponent.Slot = this;
                Machine.AddComponent(_heldComponent);
            }
        }
    }

    public void ReleaseComponent()
    {
        if (_heldComponent)
        {
            var rbs = _heldComponent.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
            }

            _heldComponent.transform.parent = null;

            GameManager.Instance.PlayerInventory.PickUp(_heldComponent.GetComponent<Grabbable>());

            _heldComponent.Slot = null;
            Machine.RemoveComponent(_heldComponent);

            _heldComponent = null;
        }
    }
}
