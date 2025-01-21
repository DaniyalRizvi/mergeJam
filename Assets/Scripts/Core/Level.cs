using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Min(2.5f)]
    public float range;
    public List<BusType> busTypes;
    public List<ColorCount> colors;
    private List<Slot> _slots;
    private List<Passenger> _passengers;
    public bool loadingData;

    public void Init()
    {
        if (Mathf.Approximately(range, 0))
            range = 7.5f;

        InitLevel();

        // Attempt to load saved state
        if (!LoadLevelStateFromJson())
        {
            // Initialize new level if no save exists
            InitBuses();
        }

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

        for (int i=0;i< _passengers.Count;i++)
        {
            _passengers[i].id = i;
        }
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

    // --- Save/Load State Methods ---

    public void SaveLevelStateToJson()
    {
        if (PlayerPrefs.GetInt("TutorialCompleted") == 0)
            return;
        
        var saveData = new LevelSaveData();

        // Save buses
        for (int i = 0; i < _slots.Count; i++)
        {
            var slot = _slots[i];
            if (slot.CurrentBus != null)
            {
                var bus = slot.CurrentBus;
                saveData.buses.Add(new BusSaveData
                {
                    slotIndex = i,
                    busColor = bus.busColor,
                    capacity = bus.capacity,
                    currentSize = bus.currentSize
                });
            }
        }

        foreach (var color in colors)
        {
            saveData.colors.Add(new ColorCount()
            {
                count = color.count,
                color = color.color
            });
        }

        // Save passengers
        foreach (var passenger in _passengers)
        {
            saveData.passengers.Add(new PassengerSaveData
            {
                id = passenger.id,
                passengerColor = passenger.passengerColor,
                hasBoarded = passenger.hasBoarded
            });
        }

        // Save slots
        for (int i = 0; i < _slots.Count; i++)
        {
            var slot = _slots[i];
            saveData.slots.Add(new SlotSaveData
            {
                slotIndex = i,
                isLocked = slot.isLocked,
                isEmpty = slot.isEmpty
            });
        }

        string json = JsonUtility.ToJson(saveData, true);
        string filePath = Path.Combine(Application.persistentDataPath, "LevelSaveData.json");
        File.WriteAllText(filePath, json);

        Debug.Log("Level state saved to: " + filePath);
    }

    public bool LoadLevelStateFromJson()
    {
        if (PlayerPrefs.GetInt("TutorialCompleted") == 0)
            return false;
        
        string filePath = Path.Combine(Application.persistentDataPath, "LevelSaveData.json");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No save file found.");
            return false;
        }
        Debug.Log(filePath);
        loadingData = true;

        string json = File.ReadAllText(filePath);
        var saveData = JsonUtility.FromJson<LevelSaveData>(json);

        // Clear current state
        foreach (var slot in _slots)
        {
            slot.ClearSlot();
        }
        _passengers.Clear();

        // Restore slots
        foreach (var slotData in saveData.slots)
        {
            var slot = _slots[slotData.slotIndex];
            slot.isLocked = slotData.isLocked;
            slot.isEmpty = slotData.isEmpty;
        }

        // Restore buses
        foreach (var busData in saveData.buses)
        {
            var slot = _slots[busData.slotIndex];
            var newBus = InstantiateBus(busData.busColor, busData.capacity, busData.currentSize);
            newBus.AssignSlot(slot);
            slot.AssignBus(newBus);
        }

        // Restore passengers
        foreach (var passengerData in saveData.passengers)
        {
            var newPassenger = InstantiatePassenger(passengerData.passengerColor, passengerData.id);
            newPassenger.hasBoarded = passengerData.hasBoarded;
            _passengers.Add(newPassenger);
        }

        Debug.Log("Level state loaded from: " + filePath);
        
        return true;
    }

    private Passenger InstantiatePassenger(Colors color, int id)
    {
        var passengerPrefab = Resources.Load<Passenger>("Passenger"); // Replace with actual path
        var newPassenger = Instantiate(passengerPrefab);
        newPassenger.passengerColor = color;
        newPassenger.id = id;
        return newPassenger;
    }

    private Bus InstantiateBus(Colors busColor, int capacity, int currentSize)
    {
        var busPrefab = Resources.Load<Bus>("VehiclesComponent"); // Replace with actual path
        var newBus = Instantiate(busPrefab);
        newBus.busColor = busColor;
        newBus.capacity = capacity;
        newBus.currentSize = currentSize;
        newBus.UpdateVisual();
        return newBus;
    }
}


// public class Level : MonoBehaviour
// {
//     [Min(2.5f)]
//     public float range;
//     public List<BusType> busTypes;
//     public List<ColorCount> colors;
//     private List<Slot> _slots;
//     private List<Passenger> _passengers;
//     public void Init()
//     {
//         if (Mathf.Approximately(range, 0))
//             range = 7.5f;
//         InitLevel();
//         InitBuses();
//         GameManager.Instance.Init(_slots, _passengers, this);
//     }
//
//     private void InitBuses()
//     {
//         var spawnPoint = GetComponentInChildren<SpawnPoint>().transform;
//         var prefab = Resources.Load<GameObject>("VehiclesComponent");
//         busTypes.Shuffle();
//         foreach (var busType in busTypes)
//         {
//             var bus = Instantiate(prefab, GetRandomSpawnPoint(spawnPoint), Quaternion.identity).GetComponent<Bus>();
//             //GameObject p=GameManager.Instance.VehicleDataManager.GetVehicleModel(busType.capacity);
//             //var bus = Instantiate(p, GetRandomSpawnPoint(spawnPoint), Quaternion.identity).GetComponent<Bus>();
//
//             bus.transform.SetParent(spawnPoint, true);
//             bus.busColor = busType.color;
//             bus.capacity = busType.capacity;
//             
//             bus.Init();
//             if (TutorialManager.Instance)
//             {
//                 var outline = bus.gameObject.AddComponent<Outline>();
//                 outline.OutlineColor = Color.red;
//                 outline.OutlineMode = Outline.Mode.OutlineAll;
//                 outline.OutlineWidth = 3f;
//                 TutorialManager.Instance.busOutlines.Add(outline);
//                 outline.enabled = false;
//             }
//         }
//     }
//
//     private Vector3 GetRandomSpawnPoint(Transform spawnPoint)
//     {
//         float theta = Random.Range(0f, 2f * Mathf.PI);
//         float phi = Random.Range(0f, Mathf.PI / 2f);
//
//         float x = Mathf.Sin(phi) * Mathf.Cos(theta);
//         float z = Mathf.Sin(phi) * Mathf.Sin(theta);
//         float y = Mathf.Cos(phi);
//
//         Vector3 randomDirection = new Vector3(x, y, z) * range;
//         return spawnPoint.position + new Vector3(0, range, 0) + randomDirection;
//     }
//
//     private void InitLevel()
//     {
//         _slots = gameObject.GetComponentsInChildren<Slot>().ToList();
//         _passengers = gameObject.GetComponentsInChildren<Passenger>().ToList();
//     }
//
//     public void SetActiveState(bool state)
//     {
//         gameObject.SetActive(state);
//     }
//
//     private void OnDrawGizmosSelected()
//     {
//         var spawnPoint = GetComponentInChildren<SpawnPoint>().transform.position;
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(spawnPoint + new Vector3(0, range, 0), range);
//     }
//
//     public void DestroyBus(Bus bus)
//     {
//         if (TutorialManager.Instance)
//         {
//             GameManager.Instance.RocketPowerUps(bus.transform);
//         }
//         Destroy(bus.gameObject);
//     }
// }
//
//
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
