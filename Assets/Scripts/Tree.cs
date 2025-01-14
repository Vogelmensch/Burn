using UnityEngine;

public class Tree : Burnable
{
    protected override Vector3 GetTopCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        return renderer.bounds.center - Vector3.up * renderer.bounds.extents.y;
    }
}
