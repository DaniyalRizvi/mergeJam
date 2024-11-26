using System.Collections.Generic;

public class Rocket : IPowerUp
{
    public void UsePowerUp(object data = null)
    {
        if(data == null)
            return;
        var sceneData = data as RocketData;
        var level = sceneData?.Level;
        var colors = sceneData?.Colors;
        if (level != null)
        {
            var busses = level.gameObject.GetComponentsInChildren<Bus>();
            foreach (var bus in busses)
            {
                if (colors.Contains(bus.busColor))
                    level.DestroyBus(bus);
            }
        }
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