using System.Linq;
using UnityEngine;

public class Fan : IPowerUp
{
    public void Execute(object data = null)
    {
        if (data is not FanData sceneData) return;
        var force = sceneData.Force;
        var buses = sceneData.Level.GetComponentsInChildren<Bus>(true).ToList();
        var busesInPile = buses.Where(bus => bus.AssignedSlot == null).ToList();
        foreach (var bus in busesInPile)
        {
            Vector3 randomDirection = Vector3.up + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            randomDirection.Normalize(); 
            float randomForce = force * Random.Range(0.9f, 1.1f);
            bus.GetComponent<Rigidbody>().AddForce(randomDirection * randomForce, ForceMode.VelocityChange);
        }
    }

    public bool ExecuteWithReturn(object data = null)
    {
        return false;
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