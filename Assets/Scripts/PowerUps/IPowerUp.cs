public interface IPowerUp
{
    public void Execute(object data = null);

    public bool ExecuteWithReturn(object data = null);
}