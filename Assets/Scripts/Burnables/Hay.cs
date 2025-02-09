using UnityEngine;

public class Hay : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 60;
        maxTemperature = 150;
        heatTransferCoefficient = 30;
        hitPoints = 100;

        base.Start();
    }
}
