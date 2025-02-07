using UnityEngine;

public class Crate : Burnable
{
    protected override void Start()
    {
        hitPoints = 200;
        base.Start();
    }
}
