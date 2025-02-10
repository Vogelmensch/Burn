using UnityEngine;

public class Barrel : Burnable
{
    public GameObject explosionPrefab;
    private float barrelExplosionRadius = 5.0f;
    private float explosionTemperatureIncrease = 150f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Explode()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, barrelExplosionRadius);
        foreach (Collider col in nearbyObjects)
        {
            if (col.transform != transform && col.transform.parent != transform)
            {
                Burnable burnable = col.GetComponent<Burnable>();
                if (burnable != null && !burnable.isOnFire && !IsWallInBetween(burnable))
                {
                    burnable.IncreaseTemperature(explosionTemperatureIncrease);
                }
            }
        }
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        base.Explode();
    }
}
