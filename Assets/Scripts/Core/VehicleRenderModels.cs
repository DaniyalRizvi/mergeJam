using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleRenderModels : MonoBehaviour
{
    [SerializeField] private List<VehicleModelData> vehicleModels = new List<VehicleModelData>();


    public int VehicleMaxCapacity()
    {
        return vehicleModels[vehicleModels.Count-1].capacity;
    }

    public void DisableAllData()
    {
        foreach (var model in vehicleModels)
        {
            model.model.SetActive(false);
        }
    }
    public void UpdateVisual(Color color)
    {
        foreach (var item in vehicleModels)
        {
            var mat= item.model.GetComponent<Renderer>().materials;
            mat[0].color = color;
            //foreach (var mat in mats) mat.color=color; //item.model.GetComponent<Renderer>()..material.color = color;
            item.model.GetComponent<Renderer>().SetMaterials(mat.ToList());
        }
    }
    public void ActiveVehicle(int Capacity)
    {

            foreach (var vehicle in vehicleModels)
            {
            //if (vehicle.color == color && vehicle.capacity == capacity)
                if (vehicle.capacity == Capacity) 
                    vehicle.model.SetActive(true);
                else 
                    vehicle.model.SetActive(false);

            }
    }
}
