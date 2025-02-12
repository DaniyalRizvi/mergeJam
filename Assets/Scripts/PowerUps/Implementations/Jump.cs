using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Jump : IPowerUp
{
    public void Execute(object data = null)
    {
        if(data == null)
            return;
        var sceneData = data as JumpData;
        var level = sceneData?.Level;
        if (level != null)
        {
            var slots = level.gameObject.GetComponentsInChildren<Slot>().ToList();
            for (int i = slots.Count - 1; i >= 0; i--)
            {
                if (slots[i].CurrentBus != null)
                {
                    var bus = slots[i].CurrentBus;
                    slots[i].CurrentBus.ClearSlot();
                    slots[i].CurrentBus = null;
                    slots[i].vehiclePlaced=false;
                    LevelManager.Instance.ApplyJump(level, bus, slots[i].transform);
                    break;
                }
            }
        }
    }

    public bool ExecuteWithReturn(object data = null)
    {
        return false;
    }
}

public class JumpData
{
    public readonly Level Level;
    public readonly float Force;

    public JumpData(Level level, float force)
    {
        Level = level;
        Force = force;
    }
}
