using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFish : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Vector3 minPosition;
    public Vector3 maxPosition;
    public Vector3 origin = Vector3.zero;
    public float radius = 10;

    ///FIND MY BOOLEAN MANAGER

    

    void Start()
        
    {
		
		// accesses the bool named "isOnFire" and changed it's value.
		//InsideBooleanManager.Atleast20ShrimpsFollowsPlayer = false;
        //InsideBooleanManager.numberOfShrimpsFollowingPlayer
        /*
        Vector3 randomPosition = new Vector3(
            Random.Range(minPosition.x, maxPosition.x),
            Random.Range(minPosition.y, maxPosition.y),
            Random.Range(minPosition.z, maxPosition.z)
        );
        Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
        */

    }

    public float intervale = 10;
    float timer;
    //public bool usersHas20Shrimps = false;
    void Update()
    {

        
        
        

      
         timer += Time.deltaTime;
        if(timer>= intervale)
        {
            SpawnFishes();
            timer -= intervale;

        }
      

      

    }

    void SpawnFishes()
    {
        Vector3 randomPosition = new Vector3(
          Random.Range(minPosition.x, maxPosition.x),
          Random.Range(minPosition.y, maxPosition.y),
          Random.Range(minPosition.z, maxPosition.z)
      );
        Instantiate(objectToSpawn, randomPosition, Quaternion.identity);

    }


}
