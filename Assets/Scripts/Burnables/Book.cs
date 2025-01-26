using UnityEngine;

public class Book : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 30;
        hitPoints = 100;
        
        base.Start();
    }
}
