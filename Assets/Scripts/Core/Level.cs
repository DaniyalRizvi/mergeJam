using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
{
    [Min(2.5f)]
    public float range;
    public List<BusType> busTypes;
    public List<ColorCount> colors;
    private List<Slot> _slots;
    private List<Passenger> _passengers;
    public void Init()
    {
        if (Mathf.Approximately(range, 0))
            range = 7.5f;
        InitLevel();
        InitBuses();
        GameManager.Instance.Init(_slots, _passengers, this);
    }

    private void InitBuses()
    {
        var spawnPoint = GetComponentInChildren<SpawnPoint>().transform;
        var prefab = Resources.Load<GameObject>("VehiclesComponent");
        busTypes.Shuffle();
        foreach (var busType in busTypes)
        {
            var bus = Instantiate(prefab, GetRandomSpawnPoint(spawnPoint), Quaternion.identity).GetComponent<Bus>();
            //GameObject p=GameManager.Instance.VehicleDataManager.GetVehicleModel(busType.capacity);
            //var bus = Instantiate(p, GetRandomSpawnPoint(spawnPoint), Quaternion.identity).GetComponent<Bus>();

            bus.transform.SetParent(spawnPoint, true);
            bus.busColor = busType.color;
            bus.capacity = busType.capacity;
            
            bus.Init();
            if (TutorialManager.Instance)
            {
                var outline = bus.gameObject.AddComponent<Outline>();
                outline.OutlineColor = Color.red;
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineWidth = 3f;
                TutorialManager.Instance.busOutlines.Add(outline);
                outline.enabled = false;
            }
        }
    }

    private Vector3 GetRandomSpawnPoint(Transform spawnPoint)
    {
        float theta = Random.Range(0f, 2f * Mathf.PI);
        float phi = Random.Range(0f, Mathf.PI / 2f);

        float x = Mathf.Sin(phi) * Mathf.Cos(theta);
        float z = Mathf.Sin(phi) * Mathf.Sin(theta);
        float y = Mathf.Cos(phi);

        Vector3 randomDirection = new Vector3(x, y, z) * range;
        return spawnPoint.position + new Vector3(0, range, 0) + randomDirection;
    }

    private void InitLevel()
    {
        _slots = gameObject.GetComponentsInChildren<Slot>().ToList();
        _passengers = gameObject.GetComponentsInChildren<Passenger>().ToList();
    }

    public void SetActiveState(bool state)
    {
        gameObject.SetActive(state);
    }

    private void OnDrawGizmosSelected()
    {
        var spawnPoint = GetComponentInChildren<SpawnPoint>().transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPoint + new Vector3(0, range, 0), range);
    }

    public void DestroyBus(Bus bus)
    {
        if (TutorialManager.Instance)
        {
            GameManager.Instance.RocketPowerUps(bus.transform);
        }
        Destroy(bus.gameObject);
    }
}


[Serializable]
public class BusType
{
    public int capacity;
    public Colors color;
}

[Serializable]
public class ColorCount
{
    public Colors color;
    public int count;
}
