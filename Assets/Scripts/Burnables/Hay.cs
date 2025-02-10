using UnityEngine;

public class Hay : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 30;
        maxTemperature = 150;
        heatTransferCoefficient = 60;
        spreadRadius = 2;
        hitPoints = 100;

        base.Start();
    }
}
