using UnityEngine;

public class Tree : Burnable
{
    protected override void Start()
    {
        ignitionTemperature = 300;
        hitPoints = 1000;
        maxTemperature = 500;
        spreadRadius = 3;
    }

    protected override void Update()
    {
        Debug.Log(hitPoints);
        base.Update();
    }

    protected override Vector3 GetTopCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        return renderer.bounds.center;
    }
}
