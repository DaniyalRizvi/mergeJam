using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VehicleModelData
{
    public GameObject model;
    public int capacity;
    public Colors Color;
} 
public class VehicleDataManager : MonoBehaviour
{
    [Header("Vehicle Models")]
    public List<VehicleModelData> vehicleModels = new List<VehicleModelData>();

    //public GameObject GetVehicleModel(Color color, int capacity)
    public GameObject GetVehicleModel(int capacity)
    {
        foreach (var vehicle in vehicleModels)
        {
            //if (vehicle.color == color && vehicle.capacity == capacity)
            if (vehicle.capacity == capacity)
            {
                return vehicle.model;
            }
        }

        Debug.LogWarning("No vehicle model found with the specified color and capacity.");
        return null;
    }
}
