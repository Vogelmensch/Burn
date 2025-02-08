using UnityEngine;

public class Stone : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 500;
        maxTemperature = 600;
        hitPoints = 50;

        base.Start();
    }
}
