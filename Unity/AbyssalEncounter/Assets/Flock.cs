using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    

    [Header("Spawn Setup")]
    [SerializeField] private FlockUnit flockUnitPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 spawnBounds;

    [Header("Speed Setup")]
    [Range(0, 2)]
    [SerializeField] private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0, 2)]
    [SerializeField] private float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }

    public float distanceAdditionalSpeed = 0.1f;

    [Header("Target")]
    public GameObject ObjectToFollow;

    [Header("StartingPoint")]
    public GameObject StartingPoint;

    public static Vector3 goalPos = Vector3.zero;
    public static Vector3 assignedBasicPos = Vector3.zero;

    public static Vector3 ExperienceStartPoint = Vector3.zero;
    

    [Header("Detection Distances")]

    [Range(0, 200)]
    [SerializeField] private float _cohesionDistance;
    public float cohesionDistance { get { return _cohesionDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceDistance;
    public float avoidanceDistance { get { return _avoidanceDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _aligementDistance;
    public float aligementDistance { get { return _aligementDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _obstacleDistance;
    public float obstacleDistance { get { return _obstacleDistance; } }

    [Range(0, 100)]
    [SerializeField] private float _boundsDistance;
    public float boundsDistance { get { return _boundsDistance; } }


    [Header("Behaviour Weights")]

    [Range(0, 10)]
    [SerializeField] private float _cohesionWeight;
    public float cohesionWeight { get { return _cohesionWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceWeight;
    public float avoidanceWeight { get { return _avoidanceWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _aligementWeight;
    public float aligementWeight { get { return _aligementWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _boundsWeight;
    public float boundsWeight { get { return _boundsWeight; } }

    [Range(0, 100)]
    [SerializeField] private float _obstacleWeight;
    public float obstacleWeight { get { return _obstacleWeight; } }

    //public FlockUnit[] allUnits { get; set; }
    

    public List<FlockUnit> allUnits;

    
    
    public booleanManager InsideBooleanManager;

    private void Start()
    {

        GameObject g = GameObject.Find("BooleanManager");
		InsideBooleanManager = g.GetComponent<booleanManager> ();

        allUnits = new List<FlockUnit>();
        //allUnits = new FlockUnit[flockSize];
        GenerateUnits();

        // Finds the object the script "IGotBools" is attached to and assigns it to the gameobject called g.
		//GameObject g = GameObject.FindGameObjectWithTag (BoolKeeper);
		//assigns the script component "IGotBools" to the public variable of type "IGotBools" names boolBoy.
		//InsideSpawnFishScript = g.GetComponent<BooleanHolder> ();

		// accesses the bool named "isOnFire" and changed it's value.
		///InsideSpawnFishScript.isOnFire = false;
    }

    public int howManyAreFollowing = 0;

    public float interval = 3;
    float timer;
    //int count = -1;
    bool doneChecking = false;
    

    
    private void Update()
    {   


        //InsideBooleanManager.Atleast20ShrimpsFollowsPlayer

        //Spawn 2 shrimps
        if(allUnits.Count == 0){
            GenerateUnits();
        }
        //Wait for player to touch the 2 first shrimps before generating others
        if (!doneChecking) {
            for(int i=0; i < allUnits.Count; i++) {
                if (allUnits[i].amIFollowingPlayer) {
                    //Debug.Log(allUnits[i].amIFollowingPlayer);
                    doneChecking = true;
                    break;
                }
            }
        }

        //Calculate how many shrimps the player have
        howManyAreFollowing = 0;
        for(int i=0; i < allUnits.Count; i++) {
            if (allUnits[i].amIFollowingPlayer) {
                 howManyAreFollowing++;
            }
        }
        //Debug.Log("nb of shrimps following = " + howManyAreFollowing);

        
    
    
        //Player has at least 2 shrimps. the others can spawn until 20 !
        if(doneChecking){
            timer += Time.deltaTime;
                if (timer >= interval)
                {   
                    if(allUnits.Count<=22){
                    GenerateUnits();           
                    timer -= interval;
                    }

                    if(howManyAreFollowing>=20){
                    GenerateUnits();           
                    timer -= interval;
                    }
                }
       }

        
        goalPos = ObjectToFollow.transform.position;
        
        ExperienceStartPoint = StartingPoint.transform.position;
        /*for (int i = 0; i < allUnits.Length; i++)
        {
            allUnits[i].MoveUnit();
        }*/
        // for (int i = 0; i < allUnits.Length; i++)
        for (int i = 0; i < allUnits.Count; i++)
        {
            allUnits[i].MoveUnit();
        }
    }

    int count;
    private void GenerateUnits()
    {


        if (allUnits.Count >= flockSize) {
            Debug.Log("NO MORE SHRIMPS");
            return;
        }

        count++;
        Debug.Log("Counter value:" + count);
        /*allUnits = new FlockUnit[flockSize];
        for (int i = 0; i < flockSize; i++)
        {
            var randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            allUnits[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation);
            allUnits[i].AssignFlock(this);
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }*/
        
        
        //count = count + 2;
        
        
        //spawn here 
       //allUnits = new FlockUnit[flockSize];
      
        var randomVector = UnityEngine.Random.insideUnitSphere;
        //randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y + 0.2f * spawnBounds.y, randomVector.z * spawnBounds.z);
        randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y + 0.35f * spawnBounds.y, randomVector.z * spawnBounds.z);
        var spawnPosition = transform.position + randomVector;
        var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

        if(doneChecking){
        assignedBasicPos = spawnPosition;
        }else{
        assignedBasicPos = StartingPoint.transform.position;
        spawnPosition = assignedBasicPos;
        }

        for (int i = 0; i < 2; i++)
      {
        FlockUnit tempVariable = Instantiate(flockUnitPrefab, spawnPosition, rotation);
        tempVariable.AssignFlock(this);
        tempVariable.InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));

        /*if(i==1){
            tempVariable.playIdleSound = false;
        }else{
            tempVariable.playIdleSound = true;
        }*/

         if(count%3 == 0){
            tempVariable.playIdleSound = true;
        }else{
            tempVariable.playIdleSound = false;
        }

        allUnits.Add(tempVariable);
      }
           //assignedBasicPos = StartingPoint.transform.position;

        Debug.Log(allUnits.Count);
        
          //spawn here 
        //allUnits = new FlockUnit[flockSize];
        /*for (int i = 0; i < 3; i++)
        { count++;

            var randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            allUnits[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation);
            allUnits[i].AssignFlock(this);
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }
          // count++;
        

        */

    }
}