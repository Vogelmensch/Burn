using UnityEngine;

public class Stone : Burnable
{
    [Header("Stone")]
    public GameObject youWonCanvas;

    protected override void Start()
    {
        ignitionTemperature = 100;
        maxTemperature = 300;
        hitPoints = 450;
        spreadRadius = 0;

        base.Start();
    }

    protected override void Explode()
    {
        youWonCanvas.SetActive(true);

        base.Explode();
    }
}
