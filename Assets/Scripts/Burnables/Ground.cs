using UnityEngine;

public class Ground : Burnable
{
    protected override void Start()
    {
        base.Start();

        damageCoefficient = 0f;
        temperature = 150;
        maxTemperature = 200;
        temperatureDecreaseAtFireballThrow = 0f;
    }
}
