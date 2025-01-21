using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix.SaveLoad
{


    public class LevelDataManager : SaveableData
    {
        public int currentLevel {get; set;}
        
        

        public LevelDataManager(Keys key) : base(key)
        {
        }
    }

    public class Levels
    {

    }

    [Serializable]
    public class ParkingLot
    {
        public int parkingSlot;
        public Colors parkingLotColor;
        // public int
    }
}