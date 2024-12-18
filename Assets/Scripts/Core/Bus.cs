using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bus : MonoBehaviour
{
    public VehicleRenderModels VehicleRenderModelsOnInitilization;
    public VehicleRenderModels VehicleRenderModels;
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
        //VehicleRenderModels.ActiveVehicle(capacity);

        VehicleRenderModelsOnInitilization.UpdateVisual(busColor.GetColor());
        VehicleRenderModelsOnInitilization.ActiveVehicle(capacity);
        
    }

    public void UpdateVisual()
    {
        VehicleRenderModels.UpdateVisual(busColor.GetColor());
        //GetComponent<Renderer>().material.color = busColor.GetColor();
        //VehicleRender.GetComponent<Renderer>().material.color = busColor.GetColor();
    }
    
    public void AssignSlot(Slot clickedSlot)
    {
        if (clickedSlot.isLocked)
        {
            Debug.Log("Slot is locked. Cannot assign a bus.");
            return;
        }

        if (AssignedSlot != null)
            AssignedSlot.CurrentBus = null;
        AssignedSlot = clickedSlot;

        capacityText.SetText(capacity.ToString());
        currentSizeText.SetText(currentSize.ToString());

         
        VehicleRenderModelsOnInitilization.DisableAllData();
        VehicleRenderModels.ActiveVehicle(capacity);
    }
}
