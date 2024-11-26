using System.Linq;
using UnityEngine;

public class Fan : IPowerUp
{
    public void UsePowerUp(object data = null)
    {
        if (data is not FanData sceneData) return;
        var force = sceneData.Force;
        var buses = sceneData.Level.GetComponentsInChildren<Bus>(true).ToList();
        var busesInPile = buses.Where(bus => bus.AssignedSlot == null).ToList();
        foreach (var bus in busesInPile)
        {
            bus.GetComponent<Rigidbody>().AddForce(Vector3.up * force, ForceMode.VelocityChange);
        }
    }
}

public class FanData
{
    public readonly Level Level;
    public readonly float Force;

    public FanData(Level level, float force)
    {
        Level = level;
        Force = force;
    }
}