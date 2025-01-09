using System;

namespace Phoenix.SaveLoad
{
    [Serializable]
    public class PlayerData : SaveableData
    {
        public int Health { get; set; }
        public int Level { get; set; }
        public PlayerData(Keys key) : base(key) { }
        
    }
    [Serializable]
    public class LevelData : SaveableData
    {
        public string Level { get; set; }
        public LevelData(Keys key) : base(key) { }
        
    }
    [Serializable]
    public class CoinsData : SaveableData
    {
        public int Coin { get; set; }
        public int Gems { get; set; }
        public CoinsData(Keys key) : base(key) { }
        
    }
    [Serializable]
    public class InventoryData : SaveableData
    {
        public int Tools { get; set; }
        public InventoryData(Keys key) : base(key) { }
        
    }
}