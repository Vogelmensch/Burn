using UnityEngine;

public class Ground : Burnable
{
    protected override void Start()
    {
        base.Start();

        hitPoints = float.MaxValue;
        temperature = float.MaxValue;
        maxTemperature = float.MaxValue;
        temperatureDecreaseAtFireballThrow = 0f;
    }
}
