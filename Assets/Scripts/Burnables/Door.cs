using UnityEngine;

public class Door : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 400;
        maxTemperature = 450;
        hitPoints = 200;

        base.Start();
    }
}
