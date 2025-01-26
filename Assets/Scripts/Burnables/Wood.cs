using UnityEngine;

public class Wood : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 100;
        hitPoints = 400f;
        maxTemperature = 300;
        spreadRadius = 1.5f;
        
        base.Start();        
    }
}
