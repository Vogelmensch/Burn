using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    public elevator platformA;
    public elevator platformB;
      public float getWeightDifference()
    {
        float weightDifference = platformA.weight - platformB.weight;
        return weightDifference;
    }
    public void Update()
    {   
        if(platformA.weight == platformB.weight){
            balance();
        } else {
            float weightDifference = getWeightDifference();
            platformA.invokeForce(weightDifference);
            platformB.invokeForce(-weightDifference);
        }
    }
    public void balance(){
        int diffA = platformA.transform.position.y - platformA.startPos.y > 0 ? 1 : -1;
        int diffB = platformB.transform.position.y - platformB.startPos.y > 0 ? 1 : -1;
        platformA.invokeForce(diffA);
        platformB.invokeForce(diffB);
    }
}
