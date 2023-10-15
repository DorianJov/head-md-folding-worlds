////////PAS Utilisé
///////////PAS Utilisé
///////////PAS Utilisé
///////////PAS Utilisé
///////////PAS Utilisé
///////////PAS Utilisé
///////////PAS Utilisé
///////////PAS Utilisé
///////////PAS Utilisé
///////////PAS Utilisé
///////////PAS Utilisé
///////////PAS Utilisé

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalFlock : MonoBehaviour
{

    public GameObject fishPrefab;

    public GameObject ObjectToFollow;

    public static Vector3 goalPos = Vector3.zero;

    
    public static int tankSize = 2;
    static int numFish = 100;

    public static GameObject[] allFish = new GameObject[numFish];
   // public Vector3 goalPos = ObjectToFollow.transform.position;

    // Start is called before the first frame update
    void Start()
    {

        for(int i = 0; i < numFish;i++){

            Vector3 pos = new Vector3(Random.Range(-tankSize,tankSize),
                                      Random.Range(0,tankSize),
                                      Random.Range(-tankSize,tankSize));

            allFish[i] = (GameObject) Instantiate(fishPrefab, pos, Quaternion.identity);
                                      
        }
        
    }

    // Update is called once per frame
    void Update()
    {
         
          goalPos = ObjectToFollow.transform.position;

         /*goalPos = new Vector3 (Random.Range(-tankSize,tankSize),
                                Random.Range(-tankSize,tankSize),
                                Random.Range(-tankSize,tankSize));
        */
    }
}
