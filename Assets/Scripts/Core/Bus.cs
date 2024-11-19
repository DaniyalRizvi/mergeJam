using UnityEngine;
using UnityEngine.Serialization;

public class Bus : MonoBehaviour
{
    [FormerlySerializedAs("currentSize")] public int capacity;
    public Colors busColor;
    public bool isMergable = true;
    internal int CurrentSize;
    private Slot _assignedSlot;
    internal Rigidbody Rb;
    public Transform gateTransform;

    public void Start()
    {
        CurrentSize = capacity;
        Rb = GetComponent<Rigidbody>();
        UpdateVisual();
    }

    void UpdateVisual()
    {
        GetComponent<Renderer>().material.color = busColor.GetColor();
    }

    public bool CanMergeWith(Bus otherBus)
    {
        return otherBus.busColor == busColor && otherBus.capacity == capacity && _assignedSlot != null && otherBus._assignedSlot != null;
    }

    public void MergeWith(Bus otherBus)
    {
        capacity *= 2;
        CurrentSize = capacity;
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
