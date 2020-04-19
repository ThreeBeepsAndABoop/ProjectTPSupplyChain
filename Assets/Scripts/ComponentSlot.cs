using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSlot : MonoBehaviour
{
    public MachineComponentType AcceptedType;
    public bool StartFilled;

    [HideInInspector]
    public MachineController Machine;

    private Transform _componentTransform;
    private MachineComponent _heldComponent;

    // Start is called before the first frame update
    void Start()
    {
        Machine = GetComponentInParent<MachineController>();
        _componentTransform = transform.Find("ComponentLocation").transform;

        if(StartFilled)
        {
            GameObject prefab = null;
            switch (AcceptedType)
            {
                case MachineComponentType.Battery:
                    prefab = GameManager.Instance.BatteryPrefab;
                    break;
                case MachineComponentType.Compressor:
                    prefab = GameManager.Instance.CompressorPrefab;
                    break;
                case MachineComponentType.Computer:
                    prefab = GameManager.Instance.ComputerPrefab;
                    break;
                case MachineComponentType.Motor:
                    prefab = GameManager.Instance.MotorPrefab;
                    break;
                case MachineComponentType.Coolant:
                    prefab = GameManager.Instance.CoolantPrefab;
                    break;
                default:
                    break;
            }

            if (prefab != null)
            {
                GameObject go = Instantiate(prefab);
                AcceptComponent(go.GetComponent<MachineComponent>());
            }
        }
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
                GameManager.Instance.PlayerInventory.Drop(grabbedItem);

                AcceptComponent(machineComponent);
            }
        }
    }

    private void AcceptComponent(MachineComponent machineComponent)
    {
        _heldComponent = machineComponent;
        var rbs = machineComponent.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        machineComponent.transform.parent = _componentTransform;
        machineComponent.transform.localPosition = Vector3.zero;
        machineComponent.transform.localRotation = Quaternion.identity;

        _heldComponent.Slot = this;
        Machine.AddComponent(_heldComponent);
    }

    public void ReleaseComponent()
    {
        if (_heldComponent && !GameManager.Instance.PlayerInventory.IsFull())
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
