using UnityEngine;

public class WoodPile : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 150;
        maxTemperature = 400;
        heatTransferCoefficient = 20;
        spreadRadius = 1.5f;
        hitPoints = 400;
        
        base.Start();
    }
}
