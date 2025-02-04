using UnityEngine;

public class Cube : Burnable
{
    protected override void Explode()
    {
        Extinguish();
        GeneralizedCubeDivider.allBurnables.Remove(this);
        Destroy(gameObject);
    }
}
