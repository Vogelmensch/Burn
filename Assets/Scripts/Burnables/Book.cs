using UnityEngine;

public class Book : Burnable
{
    protected override void Start()
    {
        fireName = "silentFirePrefab";

        ignitionTemperature = 60;
        maxTemperature = 120;
        heatTransferCoefficient = 5;
        
        base.Start();
    }
}
