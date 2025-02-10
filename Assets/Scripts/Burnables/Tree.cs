using UnityEngine;

public class Tree : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 300;
        maxTemperature = 600;
        heatTransferCoefficient = 30;
        spreadRadius *= 3;
        hitPoints = 1500;
        temperatureDecreaseAtRainHit = 10;

        base.Start();
    }

    protected override Vector3 GetTopCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        return renderer.bounds.center - Vector3.up * renderer.bounds.extents.y * 0.4f;
    }
}
