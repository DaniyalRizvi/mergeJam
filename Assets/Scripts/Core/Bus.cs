using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Bus : MonoBehaviour
{
    public int capacity
    {
        get => _capacity;
        set
        {
            _capacity = value;
            if (AssignedSlot != null)
                capacityText.SetText(value.ToString());
        }
    }

    private int _capacity;
    
    internal int currentSize
    {
        get => _currentSize;
        set
        {
            _currentSize = value;
            if (AssignedSlot != null)
                currentSizeText.SetText(value.ToString());
        }
    }
    private int _currentSize;
    
    
    public Colors busColor;
    public bool isMergable = true;
    public Transform gateTransform;
    internal Rigidbody Rb;
    internal Slot AssignedSlot;
    public TMP_Text capacityText;
    public TMP_Text currentSizeText;

    public void Init()
    {
        currentSize = capacity;
        Rb = GetComponent<Rigidbody>();
        UpdateVisual();
    }

    void UpdateVisual()
    {
        GetComponent<Renderer>().material.color = busColor.GetColor();
    }

    public bool CanMergeWith(Bus otherBus)
    {
        return otherBus.busColor == busColor && otherBus.capacity == capacity && AssignedSlot != null && otherBus.AssignedSlot != null;
    }

    public void MergeWith(Bus otherBus)
    {
        capacity *= 2;
        currentSize = capacity;
        UpdateVisual();
        Destroy(otherBus.gameObject);
    }

    public void AssignSlot(Slot clickedSlot)
    {
        if (AssignedSlot != null)
        {
            AssignedSlot.CurrentBus = null;
        }
        AssignedSlot = clickedSlot;
        
        capacityText.SetText(capacity.ToString());
        currentSizeText.SetText(currentSize.ToString());
    }
}
