namespace Phoenix.SaveLoad
{
    public abstract class SaveableData: ISaveable
    {
        protected SaveableData(Keys key)
        {
            this.Key = key;
        }
        public Keys Key { get; protected set;}
    }
}