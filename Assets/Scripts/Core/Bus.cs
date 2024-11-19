using UnityEngine;

public class Bus : MonoBehaviour
{
    public int currentSize;
    public Colors busColor;
    public bool isMergable = true;
    internal int Capacity;
    private Slot _assignedSlot;
    internal Rigidbody Rb;
    public Transform gateTransform;

    public void Start()
    {
        Capacity = currentSize;
        Rb = GetComponent<Rigidbody>();
        UpdateVisual();
    }

    void UpdateVisual()
    {
        GetComponent<Renderer>().material.color = busColor.GetColor();
    }

    public bool CanMergeWith(Bus otherBus)
    {
        return otherBus.busColor == busColor && otherBus.currentSize == currentSize && _assignedSlot != null && otherBus._assignedSlot != null;
    }

    public void MergeWith(Bus otherBus)
    {
        currentSize *= 2;
        Capacity = currentSize;
        UpdateVisual();
        Destroy(otherBus.gameObject);
    }

    public void AssignSlot(Slot clickedSlot)
    {
        if (_assignedSlot != null)
        {
            _assignedSlot.CurrentBus = null;
        }
        _assignedSlot = clickedSlot;
    }
}
