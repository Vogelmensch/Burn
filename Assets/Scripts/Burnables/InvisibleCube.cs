using UnityEngine;

public class InvisibleCube : Burnable

{
    public delegate void CubeDestroyedHandler(GameObject cube);
    public event CubeDestroyedHandler OnDestroyed;

    protected override void Start()
    {
        hitPoints = 100;
        base.Start();
    }
    private void DestroyCube()
    {
        if (OnDestroyed != null)
        {
            OnDestroyed.Invoke(gameObject);
        }
        Destroy(gameObject);
    }
}
