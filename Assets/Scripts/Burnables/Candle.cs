using UnityEngine;

public class Candle : Burnable
{
    protected override void Start()
    {
        hitPoints = 10000f;
        ignitionTemperature = 0;
        isOnFire = true;
        base.Start();
    }
}
