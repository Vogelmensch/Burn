using UnityEngine;

public class Candle : Burnable
{
    protected override void Start()
    {
        hitPoints = float.MaxValue;
        ignitionTemperature = 0;
        isOnFire = true;
        base.Start();
    }
}
