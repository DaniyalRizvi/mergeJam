using System;
using System.Collections;
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
    public bool slotAssigned=false;

    public void Init()
    {
        currentSize = capacity;
        Rb = GetComponent<Rigidbody>();
        UpdateVisual();  
        VehicleRenderModelsOnInitilization.UpdateVisual(busColor.GetColor());
        VehicleRenderModelsOnInitilization.ActiveVehicle(capacity);
        
    }

    public int MaxBuCapacity()
    {
        return VehicleRenderModelsOnInitilization.VehicleMaxCapacity();
    }

    public Slot GetAssignedSlot()
    {
        return AssignedSlot;
    }

    public void UpdateVisual()
    {
        VehicleRenderModels.UpdateVisual(busColor.GetColor()); 
    }

    public void AssignSlot(Slot clickedSlot)
    {
        if (clickedSlot.isLocked)
        {
            Debug.Log("Slot is locked. Cannot assign a bus.");
            return;
        }

        SoundManager.Instance.AddingVehiclesToSlotsSFX();
        
        if (AssignedSlot != null)
            AssignedSlot.CurrentBus = null;
        AssignedSlot = clickedSlot;

        gameObject.GetComponent<BoxCollider>().isTrigger = true;

        //StartCoroutine(SetVehicle());
    }

    public void ClearSlot()
    {
        if (AssignedSlot == null)
            return;
        AssignedSlot = null;
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
        capacityText.SetText("");
        currentSizeText.SetText("");
    }

    public IEnumerator SetVehicle()
    {
        yield return new WaitForSeconds(3f);

        VehicleRenderModelsOnInitilization.DisableAllData();
        VehicleRenderModels.ActiveVehicle(capacity);

        //yield return new WaitForSeconds(0.5f);

        
    }

    
    public BusSaveData ToBusData(int assignedSlotIndex)
    {
        return new BusSaveData
        {
            capacity = this.capacity,
            currentSize = this.currentSize,
            busColor = this.busColor,
            position = transform.position,
            slotIndex = assignedSlotIndex
        };
    }

    public void FromBusData(BusSaveData data, Slot assignedSlot)
    {
        this.capacity = data.capacity;
        this.currentSize = data.currentSize;
        this.busColor = data.busColor;
        transform.position = data.position;
        AssignSlot(assignedSlot);
        UpdateVisual();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            VehicleRenderModelsOnInitilization.DisableAllData();
            VehicleRenderModels.ActiveVehicle(capacity);
            VehicleRenderModels.ActiveVehicle(capacity);
            capacityText.SetText(capacity.ToString());
            currentSizeText.SetText(currentSize.ToString());
        }
    }
}
