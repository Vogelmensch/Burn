using UnityEngine;

public class Stone : Burnable
{
    [Header("Stone")]
    public GameObject youWonCanvas;

    protected override void Start()
    {
        ignitionTemperature = 300;
        maxTemperature = 310;
        hitPoints = 300;
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
