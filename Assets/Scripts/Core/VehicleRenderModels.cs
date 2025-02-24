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
    public void UpdateVisual(List<Material> materials)
    {
        Debug.Log("Materials count:"+materials.Count);
        for(int i=0;i<materials.Count;i++)
        {
            Debug.Log("Material: index:"+i+" name:");
            vehicleModels[i].model.GetComponent<Renderer>().material=materials[i];
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
