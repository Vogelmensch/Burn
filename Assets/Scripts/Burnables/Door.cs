using UnityEngine;

public class Door : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 400;
        maxTemperature = 450;
        spreadRadius = 0;
        hitPoints = 200;
        temperatureDecreaseAtRainHit = 0;

        base.Start();
    }
}
