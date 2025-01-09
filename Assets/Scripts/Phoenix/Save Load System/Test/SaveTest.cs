using Phoenix.Singleton;
using UnityEngine;

namespace Phoenix.SaveLoad
{
    public class SaveTest : PersistentSingleton<SaveTest>
    {
        public ISaveable data;
        
        private IDataService dataHandler;

        protected override void Awake()
        {
            base.Awake();
            dataHandler = new DataService(new Serializer());

            data = new CoinsData(Keys.Resources)
            {
                Coin = 10,
                Gems = 5
            };
            dataHandler.Save<CoinsData>(data as CoinsData);
            
            data = new PlayerData(Keys.Player)
            {
                Health = 10,
                Level = 1
            };
          
            dataHandler.Save<PlayerData>(data as PlayerData);
           
            data = new LevelData(Keys.Game)
            {
                Level = "1"
            };
          
            dataHandler.Save<LevelData>(data as LevelData);
            
            data = new PlayerData(Keys.AI)
            {
                Health = 30,
                Level = 2
            };
          
            dataHandler.Save<PlayerData>(data as PlayerData);
            
            data = new InventoryData(Keys.Inventory)
            {
                Tools = 1
            };
          
            dataHandler.Save<InventoryData>(data as InventoryData);
            
        }

        public void Save()
        {
            data = new CoinsData(Keys.Resources)
            {
                Coin = 20,
                Gems = 10
            };
            dataHandler.Save<CoinsData>(data as CoinsData);
            
            data = new PlayerData(Keys.Player)
            {
                Health = 20,
                Level = 2
            };
            dataHandler.Save<PlayerData>(data as PlayerData);
            
            data = new PlayerData(Keys.AI)
            {
                Health = 30,
                Level = 2
            };
            dataHandler.Save<PlayerData>(data as PlayerData);
            
            data = new LevelData(Keys.Game)
            {
                Level = "2"
            };
          
            dataHandler.Save<LevelData>(data as LevelData);
            
            data = new InventoryData(Keys.Inventory)
            {
                Tools = 3
            };
          
            dataHandler.Save<InventoryData>(data as InventoryData);
        }

        public void Load()
        {
            Debug.Log(dataHandler.Load<PlayerData>(Keys.Player).Health);
            Debug.Log(dataHandler.Load<LevelData>(Keys.Game).Level);
            Debug.Log(dataHandler.Load<PlayerData>(Keys.AI).Health);
            Debug.Log(dataHandler.Load<CoinsData>(Keys.Resources).Coin);
            Debug.Log(dataHandler.Load<InventoryData>(Keys.Inventory).Tools);
        }
        
    }
}