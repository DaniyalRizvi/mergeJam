using System.Collections.Generic;
using System.Linq;

public class Rocket : IPowerUp
{
    public void Execute(object data = null)
    {
        if(data == null)
            return;
        var sceneData = data as RocketData;
        var level = sceneData?.Level;
        var colors = sceneData?.Colors;
        if (level != null)
        {
            var busses = level.gameObject.GetComponentsInChildren<Bus>().ToList();
            busses.Shuffle();
            foreach (var bus in busses)
            {
                if (colors.Contains(bus.busColor))
                {
                    level.DestroyBus(bus);
                    break;
                }
            }
        }
    }

    public bool ExecuteWithReturn(object data = null)
    {
        if(data == null)
            return false;
        var sceneData = data as RocketData;
        var level = sceneData?.Level;
        var colors = sceneData?.Colors;
        if (level != null)
        {
            var busses = level.gameObject.GetComponentsInChildren<Bus>().ToList();
            busses.Shuffle();
            return busses.Any(bus => colors.Contains(bus.busColor));
        }
        return false;
    }
}

public class RocketData
{
    public readonly Level Level;
    public readonly List<Colors> Colors;

    public RocketData(Level level, List<Colors> colors)
    {
        Level = level;
        Colors = colors;
    }
}