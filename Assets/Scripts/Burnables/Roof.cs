using UnityEngine;

public class Roof : Burnable
{
    protected override void Start()
    {
        temperatureDecreaseAtRainHit = 0;

        ignitionTemperature = 300;
        maxTemperature = 400;
        heatTransferCoefficient = 2;
        spreadRadius = 2;
        hitPoints = 250;

        base.Start();
    }
}
