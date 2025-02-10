using UnityEngine;

public class Roof : Burnable
{
    protected override void Start()
    {
        temperatureDecreaseAtRainHit = 0;
        temperatureDecreaseAtFireballThrow = 0;

        ignitionTemperature = 60;
        maxTemperature = 400;
        heatTransferCoefficient = 2;
        spreadRadius *= 2;
        hitPoints = 150;

        base.Start();
    }
}
