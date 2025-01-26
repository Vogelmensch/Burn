using UnityEngine;

public class Crate : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 70;
        hitPoints = 150f;
        
        base.Start();
    }
}
