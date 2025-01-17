using UnityEngine;

public class Barrel : Burnable
{
    public GameObject explosionPrefab;
    public bool isOil; // If it's not oil, then it's water
    public double chanceToBeOil = 1; // Chance that barrel will be oil
    public float barrelExplosionRadius = 5.0f;

    protected override void Start()
    {
        isOil = DetermineType();
        base.Start();
    }

    protected override void Update()
    {
        UpdateHelper(true);
    }

    protected override void Explode(bool isWaterBarrel)
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, barrelExplosionRadius);
        foreach (Collider col in nearbyObjects)
        {
            if (col.transform != transform && col.transform.parent != transform)
            {
                Burnable burnable = col.GetComponent<Burnable>();
                if (burnable != null && !burnable.isOnFire && isOil)
                {
                    burnable.Ignite();
                }
            }
        }
        if (isOil) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        base.Explode(!isOil);
    }

    // Determine if the barrel contains oil or water 
    private bool DetermineType()
    {
        float rand = Random.Range(0.0f,1.0f);
        return rand < chanceToBeOil;
    }
}
