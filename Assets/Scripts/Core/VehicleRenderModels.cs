using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleRenderModels : MonoBehaviour
{
    [SerializeField] private List<VehicleModelData> vehicleModels = new List<VehicleModelData>();

    public void UpdateVisual(Color color)
    {
        foreach (var item in vehicleModels)
        {
            var mats= item.model.GetComponent<Renderer>().materials;
            foreach (var mat in mats) mat.color = color;
            //item.model.GetComponent<Renderer>()..material.color = color;
            item.model.GetComponent<Renderer>().SetMaterials(mats.ToList());
        }
    }
    public void ActiveVehicle(int Capacity)
    {
            foreach (var vehicle in vehicleModels)
            {
            //if (vehicle.color == color && vehicle.capacity == capacity)
            if (vehicle.capacity == Capacity)
            {
                vehicle.model.SetActive(true);
            }
            else
                vehicle.model.SetActive(false);

            }
    }
}
