using UnityEngine;

public class Hay : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 60;
        maxTemperature = 150;
        heatTransferCoefficient = 10;
        hitPoints = 50;

        base.Start();
    }
}
