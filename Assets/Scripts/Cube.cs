using UnityEngine;

public class Cube : Burnable
{
    protected override void Explode(bool isWaterBarrel)
    {
        Extinguish();
        GeneralizedCubeDivider.allBurnables.Remove(this);
        Destroy(gameObject);
    }
}
