using UnityEngine;

public class Stone : Burnable
{
    [Header("Stone")]
    public GameObject youWonCanvas;

    protected override void Start()
    {
        ignitionTemperature = 200;
        maxTemperature = 300;
        hitPoints = 450;
        spreadRadius = 0;
        temperatureDecreaseAtRainHit = 50;

        base.Start();
    }

    protected override void Explode()
    {
        youWonCanvas.SetActive(true);

        base.Explode();
    }
}
