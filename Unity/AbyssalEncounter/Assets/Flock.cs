using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Flock : MonoBehaviour
{



    [Header("Spawn Setup")]
    [SerializeField] private FlockUnit flockUnitPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 spawnBounds;
    [SerializeField] private Transform spawnOrigin;

    [Header("Feedback")]
    [SerializeField] private GlowingCircuitEffect circuitEffect;

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

    public Vector3 goalPos = Vector3.zero;
    public Vector3 assignedBasicPos = Vector3.zero;

    public Vector3 EndingPos = Vector3.zero;

    public Vector3 ExperienceStartPoint = Vector3.zero;


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
    public GameObject HugeShrimp;
    GameObject GlobalVolumePostEffect;

    public GameObject HugeShrimpRedLight;

    GameObject Fog;

    GameObject AllElements;

    GameObject AllMovableElements;

    GameObject BlackScreen;

    GameObject Title;



    private void Start()
    {
        //Ending scene stuff
        //HugeShrimp = GameObject.Find("HugeShrimpFinal");

        //Giant RedLight
        //HugeShrimpRedLight = GameObject.Find("HugeShrimpRedLight");

        // GlobalVolumePostEffect = GameObject.Find("Global Volume");
        Fog = GameObject.Find("Fog/Particles");

        //Elements to deactivate once is the end.
        AllElements = GameObject.Find("AllElements");

        //Elements to move
        AllMovableElements = GameObject.Find("AllMovableElements");

        //Blackscreen.
        BlackScreen = GameObject.Find("BlackScreen");

        //Title
        Title = GameObject.Find("Title");




        //Not used stuff
        //GameObject g = GameObject.Find("BooleanManager");
        //InsideBooleanManager = g.GetComponent<booleanManager>();

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
        if (endingSceneIsPlaying)
        {
            EndingSceneUpdate();
        }
        //EndingScene();
        //InsideBooleanManager.Atleast20ShrimpsFollowsPlayer

        //Spawn 2 shrimps
        if (allUnits.Count == 0)
        {
            GenerateUnits();
        }
        //Wait for player to touch the 2 first shrimps before generating others
        if (!doneChecking)
        {
            for (int i = 0; i < allUnits.Count; i++)
            {
                if (allUnits[i].amIFollowingPlayer)
                {
                    //Debug.Log(allUnits[i].amIFollowingPlayer);
                    doneChecking = true;
                    break;
                }
            }
        }

        //Calculate how many shrimps the player have
        howManyAreFollowing = 0;
        for (int i = 0; i < allUnits.Count; i++)
        {
            if (allUnits[i].amIFollowingPlayer)
            {
                howManyAreFollowing++;
            }
        }
        circuitEffect.SetFishCount(howManyAreFollowing == 0 ? -1 : howManyAreFollowing);
        //Debug.Log("nb of shrimps following = " + howManyAreFollowing);




        //Player has at least 2 shrimps. the others can spawn until 20 !
        // Stop spawning when ending scene is playing
        if (doneChecking && !endingSceneIsPlaying)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                if (allUnits.Count <= 22)
                {
                    GenerateUnits();
                    timer = 0;
                }

                if (howManyAreFollowing >= 20)
                {
                    GenerateUnits();
                    timer = 0;
                }
            }
        }


        goalPos = ObjectToFollow.transform.position;

        ExperienceStartPoint = StartingPoint.transform.position;

        if (endingSceneIsPlaying)
        {
            EndingPos = HugeShrimpRedLight.transform.position;

        }
        /*for (int i = 0; i < allUnits.Length; i++)
        {
            allUnits[i].MoveUnit();
        }*/
        // for (int i = 0; i < allUnits.Length; i++)
        for (int i = 0; i < allUnits.Count; i++)
        {
            allUnits[i].MoveUnit();
        }

        // go here only once.
        if (howManyAreFollowing >= 75 && !endingSceneIsPlaying)
        {
            endingSceneIsPlaying = true;
            // do the ending setup here.
            EndingSceneSetup();
        }

    }

    int count;
    private void GenerateUnits()
    {


        if (allUnits.Count >= flockSize)
        {
            Debug.Log("NO MORE SHRIMPS");
            return;
        }

        count++;
        //Debug.Log("Counter value:" + count);
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

        // Override of spawn position by Raphael
        randomVector = GetRandomVectorPosition();
        spawnPosition = spawnOrigin != null ? spawnOrigin.position + randomVector : transform.position + randomVector;

        var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

        if (doneChecking)
        {
            assignedBasicPos = spawnPosition;
        }
        else
        {
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

            if (count % 3 == 0)
            {
                tempVariable.playIdleSound = true;
            }
            else
            {
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

    /// <summary>
    /// Gets a random position vector
    /// with a full range random rotation in Y
    /// a defined range rotation in X
    /// and a random length between 30 and 60cm (~arm length)
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomVectorPosition()
    {
        var angleY = Random.Range(0f, 360f);
        var angleX = Random.Range(-45f, 45f);
        var length = Random.Range(0.45f, 0.7f);

        var vector = Vector3.forward;
        var rot = Quaternion.Euler(angleX, angleY, 0f);
        var randomPosition = rot * vector;  // rotates the direction
        return randomPosition * length;
    }

    float duration = 0.1f;
    Color color0 = Color.red;
    Color color1 = Color.blue;
    private float timeCount = 0.0f;

    private float coloranimvar = 0.0f;
    public bool endingSceneIsPlaying = false;
    public bool titleScreenIsOn;

    float endingSceneTimeBeforeTitle = 15;

    float Vibrationspeed = 10.0f;
    float Vibrationintensity = 0.3f;

    private void EndingSceneSetup()
    {
        //Set the object to be "activated"/Visible
        HugeShrimp.SetActive(true);

    }

    private void EndingSceneUpdate()
    {
        /*
        // HowmanyShrimpAreFollowing => 75
        ///////////////////////////////// Play Ending sounds: Scream of a giant shrimp (once) and Huge vibrating bass 

        //GetSoundObject
        GameObject FinalSound = GameObject.Find("FinalSound");

        //Get Audios
        AudioSource[] FinalSounds = FinalSound.GetComponents<AudioSource>();

        //Play Huge scream
        FinalSounds[1].Play();
        //Play Vibration Huge Bass
        FinalSounds[2].Play();
        */

        //Make everything move:perlin noise
        AllMovableElements.transform.position = Vibrationintensity * new Vector3(
            Mathf.PerlinNoise(Vibrationspeed * Time.time, 1),
            Mathf.PerlinNoise(Vibrationspeed * Time.time, 2),
            Mathf.PerlinNoise(Vibrationspeed * Time.time, 3));
        /////////////////////////////////// Turn Everything Red
        //couln't have acces to global volume parameters, idk how to do.
        //Get the Global volume object which has post effects and the fog object
        var FogLight = Fog.GetComponentInChildren<Light>();

        FogLight.color = Color.Lerp(color1, color0, timeCount);
        var deltaTime = Time.deltaTime;
        if (timeCount <= 1)
        {
            timeCount += deltaTime / 2;
        }
        else
        {
            FogLight.intensity += deltaTime * 10;
            //FogLight.spotAngle -= timing;
        }

        ///////////////////////////////// Set Visible the Huge shrimps and make it move slowly upwards


        //Move it upwards
        HugeShrimp.transform.position += transform.up * deltaTime;

        //////////////////////////////// After 6 seconds
        /////////////////////////////// Make it all dark with the title only "AbyssalEncounter" visible.
        endingSceneTimeBeforeTitle -= deltaTime;
        if (endingSceneTimeBeforeTitle < 0 && !titleScreenIsOn)
        {
            // we get here only once
            titleScreenIsOn = true;
            StartCoroutine(FadeOutHugeCrevetteAudio());
            StartCoroutine(FadeInTitle());
            //find gameobject Allelements and setActive = false;
            //find gameobject BlackScreen and meshrenderer = true;
            // find gameObject Title and setActive = true;
            //GameObject FinalText = GameObject.Find("Title");

            // AllElements.SetActive(false);

            //Blackscreen.
            // BlackScreen.GetComponent<MeshRenderer>().enabled = true;

            //Title
            // Title.GetComponent<MeshRenderer>().enabled = true;
        }

        IEnumerator FadeOutHugeCrevetteAudio()
        {
            var time = 1f;
            var duration = 7f;
            var audio = HugeShrimp.GetComponent<AudioSource>();
            while (time >= 0)
            {
                audio.volume = time;
                time -= Time.deltaTime / duration;
                yield return null;
            }
            audio.volume = 0;
        }

        IEnumerator FadeInTitle()
        {
            var time = 0f;
            var duration = 5f;

            var tmp = Title.GetComponent<TextMeshPro>();
            var startColor = Color.clear;
            var endColor = Color.white;
            tmp.color = startColor;

            Title.GetComponent<MeshRenderer>().enabled = true;

            while (time < 1f)
            {
                time += Time.deltaTime / duration;
                tmp.color = Color.Lerp(startColor, endColor, time);
                yield return null;
            }
            tmp.color = endColor;
        }
    }

}