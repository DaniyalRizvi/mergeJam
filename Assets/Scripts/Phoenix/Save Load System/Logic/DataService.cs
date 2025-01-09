using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Phoenix.SaveLoad
{
    public class DataService : IDataService
    {
        private readonly ISerializer _serializer;
        private readonly string _dataPath;
        private readonly string _fileExtension;

        public DataService(ISerializer serializer)
        {
            _dataPath = Application.persistentDataPath;
            _fileExtension = "json";
            _serializer = serializer;

        }

        private string GetFilePath(string key)
        {
            return Path.Combine(_dataPath, string.Concat(key, ".", _fileExtension));
        }
        
        public void Save<T>(T data, bool overwrite = true) where T : ISaveable
        {
            string fileLocation = GetFilePath(data.Key.ToString());

            if (!overwrite && File.Exists(fileLocation))
                throw new IOException($"File'{data.Key.ToString()}.{_fileExtension}' already exists and cannot be overwritten.");
            
            File.WriteAllText(fileLocation, _serializer.Serialize(data));

        }

        public T Load<T>(Keys key) where T : ISaveable
        {
            string fileLocation = GetFilePath(key.ToString());

            if (!File.Exists(fileLocation))
                throw new IOException($"File'{key.ToString()}.{_fileExtension}' not found.");
            
            return _serializer.Deserialize<T>(File.ReadAllText(fileLocation));
        }

        public void Delete(Keys key)
        {
            string fileLocation = GetFilePath(key.ToString());

            if (!File.Exists(fileLocation))
                throw new IOException($"File'{key.ToString()}.{_fileExtension}' not found.");
            
            File.Delete(fileLocation);
        }

        public void DeleteAll()
        {
            foreach (string key in Directory.GetFiles(_dataPath)) 
                File.Delete(key);
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string key in Directory.EnumerateFiles(_dataPath))
                if (Path.GetExtension(key) == _fileExtension)
                    yield return Path.GetFileNameWithoutExtension(key);
        }
    }
}