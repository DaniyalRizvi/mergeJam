using System.Collections.Generic;

namespace Phoenix.SaveLoad
{
    public interface IDataService
    {
        void Save<T>(T data, bool overwrite = true) where T : ISaveable;
        T Load<T>(Keys key) where T : ISaveable;
        void Delete(Keys key);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}