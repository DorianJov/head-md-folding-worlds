using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class FlockUnit : MonoBehaviour
{
	[SerializeField] private float FOVAngle;
	[SerializeField] private float smoothDamp;
	[SerializeField] private LayerMask obstacleMask;
	[SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles;
	
	[Header("Audio sources")]
	[SerializeField] private AudioSource criCrevetteAudio;
	[SerializeField] private AudioSource crevetteSwimAudio;
	[SerializeField] private AudioSource[] CatchCrevetteAudioSources;
	
	private List<FlockUnit> cohesionNeighbours = new List<FlockUnit>();
	private List<FlockUnit> avoidanceNeighbours = new List<FlockUnit>();
	private List<FlockUnit> aligementNeighbours = new List<FlockUnit>();
	private Flock assignedFlock;
	private Vector3 currentVelocity;
	private Vector3 currentObstacleAvoidanceVector;
	private float speed;

    private bool checking = true;
    private bool isAlive = true;

	public bool playIdleSound = true;

	public Material crevetteMaterialFollowing;
	public Material crevetteMaterialVagabond;
	public Material crevetteMaterialTouching;


    Vector3 goalPos = Vector3.zero;

	public bool amIFollowingPlayer = false;
    

	public Transform myTransform { get; set; }

	private void Awake()
	{
		myTransform = transform;
	}
	
	void Start()
	{
		//Material material = new Material(Shader.Find("Crevette"));
		//material.SetColor("Color_C6F9B478", Color.blue);
		//this.gameObject.GetComponent<Renderer>().material = material;
		goalPos = Flock.assignedBasicPos;
	}

	public void AssignFlock(Flock flock)
	{
		assignedFlock = flock;
	}

	public void InitializeSpeed(float speed)
	{
		this.speed = speed*-1;
	}
	
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monstre") && isAlive) {
            isAlive = false;
            // In the List<>
            assignedFlock.allUnits.Remove(this);
            // in the heirachy
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false; // desactiver le visuel le temps de mourir

            //Debug.Log("Make a noise!!!!!");
            criCrevetteAudio.Play();
			
            Destroy(gameObject, criCrevetteAudio.clip.length); // le temps de audio
        }
        
        if (other.CompareTag("FollowMe") && !amIFollowingPlayer) {
				
			// Debug.Log("FollowMe!!!!!");
			CatchCrevetteAudioSources[Random.Range(0, CatchCrevetteAudioSources.Length)].Play();
			crevetteSwimAudio.Stop();	// seules les vagabondes bruissent
			
			amIFollowingPlayer = true;
			var glowDuration = 0.15f;
			StartCoroutine(MakeMeGlowCoroutine(glowDuration));
			gameObject.tag ="Shrimps";
        }
	}
    
	IEnumerator MakeMeGlowCoroutine(float duration)
	{
		var childRenderer = gameObject.GetComponentInChildren<Renderer>();
		childRenderer.material = crevetteMaterialTouching;
		yield return new WaitForSeconds(duration);
		childRenderer.material = crevetteMaterialFollowing;
	}
	
    
	 
	public void MoveUnit()
	{   
		if(assignedFlock.allUnits.Count == 2 && !amIFollowingPlayer){
			goalPos = Flock.ExperienceStartPoint;
		}
         
        ///if the shrimps follows the users hand
        if(amIFollowingPlayer){
             goalPos = Flock.goalPos;
        }

		FindNeighbours();
		CalculateSpeed();

		var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
		var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
		var aligementVector = CalculateAligementVector() * assignedFlock.aligementWeight;

		//if(amIFollowingPlayer){}
		//boundsVector = CalculateBoundsVector() * 10;
		var boundsForceWhenFollowing = assignedFlock.boundsWeight;

		if(amIFollowingPlayer){
			boundsForceWhenFollowing = 10;
		}
		var boundsVector = CalculateBoundsVector() * boundsForceWhenFollowing;
		 
        
        //I NEEED PERFORMANCES
		//var obstacleVector = CalculateObstacleVector() * assignedFlock.obstacleWeight;

        //var moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector + obstacleVector + goalPos;

		var moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector;
		moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);

		float distance = Vector3.Distance (transform.position, goalPos);

		Vector3 targetForward = moveVector.normalized;
		if(amIFollowingPlayer || assignedFlock.allUnits.Count == 2 && !amIFollowingPlayer ){
			moveVector = targetForward * (speed * (distance * assignedFlock.distanceAdditionalSpeed));
		}else{
			//Debug.Log("allunits: " + assignedFlock.allUnits.Count);
			moveVector = targetForward * speed;
		}
		
		if (moveVector == Vector3.zero)
			moveVector = transform.forward;

		myTransform.forward = targetForward;
		myTransform.position += moveVector * Time.deltaTime;
	}
	
	private void FindNeighbours()
	{
		cohesionNeighbours.Clear();
		avoidanceNeighbours.Clear();
		aligementNeighbours.Clear();
		var allUnits = assignedFlock.allUnits;
        for (int i = 0; i < allUnits.Count; i++)
		//for (int i = 0; i < allUnits.Length; i++)
		{
			var currentUnit = allUnits[i];
			if (currentUnit != this)
			{
				float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentUnit.myTransform.position - myTransform.position);
				if(currentNeighbourDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
				{
					cohesionNeighbours.Add(currentUnit);
				}
				if (currentNeighbourDistanceSqr <= assignedFlock.avoidanceDistance * assignedFlock.avoidanceDistance)
				{
					avoidanceNeighbours.Add(currentUnit);
				}
				if (currentNeighbourDistanceSqr <= assignedFlock.aligementDistance * assignedFlock.aligementDistance)
				{
					aligementNeighbours.Add(currentUnit);
				}
			}
		}
	}

	private void CalculateSpeed()
	{
	    //float distance = Vector3.Distance (this.gameObject.transform.position, goalPos);
		
		//distance = distance;
		if (cohesionNeighbours.Count == 0)
			return;
		speed = 0;
		for (int i = 0; i < cohesionNeighbours.Count; i++)
		{
			speed += cohesionNeighbours[i].speed;
		}

		speed /= cohesionNeighbours.Count;// + distance ;
		
		//speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
		///speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
		speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
		//Debug.Log(speed);
	}

	private Vector3 CalculateCohesionVector()
	{
		var cohesionVector = Vector3.zero;
		if (cohesionNeighbours.Count == 0)
			return Vector3.zero;
		int neighboursInFOV = 0;
		for (int i = 0; i < cohesionNeighbours.Count; i++)
		{
			if (IsInFOV(cohesionNeighbours[i].myTransform.position))
			{
				neighboursInFOV++;
				cohesionVector += cohesionNeighbours[i].myTransform.position;
			}
		}

		cohesionVector /= neighboursInFOV;
		cohesionVector -= myTransform.position;
		cohesionVector = cohesionVector.normalized;
		return cohesionVector;
	}

	private Vector3 CalculateAligementVector()
	{
		var aligementVector = myTransform.forward;
		if (aligementNeighbours.Count == 0)
			return myTransform.forward;
		int neighboursInFOV = 0;
		for (int i = 0; i < aligementNeighbours.Count; i++)
		{
			if (IsInFOV(aligementNeighbours[i].myTransform.position))
			{
				neighboursInFOV++;
				aligementVector += aligementNeighbours[i].myTransform.forward;
			}
		}

		aligementVector /= neighboursInFOV;
		aligementVector = aligementVector.normalized;
		return aligementVector;
	}

	private Vector3 CalculateAvoidanceVector()
	{
		var avoidanceVector = Vector3.zero;
		if (aligementNeighbours.Count == 0)
			return Vector3.zero;
		int neighboursInFOV = 0;
		for (int i = 0; i < avoidanceNeighbours.Count; i++)
		{
			if (IsInFOV(avoidanceNeighbours[i].myTransform.position))
			{
				neighboursInFOV++;
				avoidanceVector += (myTransform.position - avoidanceNeighbours[i].myTransform.position);
			}
		}

		avoidanceVector /= neighboursInFOV;
		avoidanceVector = avoidanceVector.normalized;
		return avoidanceVector;
	}
	
	private Vector3 CalculateBoundsVector()
	{

            /*
          if(touched){
             goalPos = Flock.goalPos;
        }else{
             goalPos = Flock.assignedBasicPos;
        }
        /*
       // Vector3 goalPos = Flock.goalPos;

         /*
          if(!Touched){
            Vector3 goalPos = Flock.goalPos;
        }else{
            Vector3 goalPos = Flock.assignedBasicPos;
        }

       

        /*

        /*
		var offsetToCenter = assignedFlock.transform.position - myTransform.position;
		bool isNearCenter = (offsetToCenter.magnitude >= assignedFlock.boundsDistance * 0.9f);
		return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
        */
        var offsetToCenter = assignedFlock.transform.position + goalPos - myTransform.position;
		bool isNearCenter = (offsetToCenter.magnitude >= assignedFlock.boundsDistance * 0.9f);
		return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
	}

	private Vector3 CalculateObstacleVector()
	{
		var obstacleVector = Vector3.zero;
		RaycastHit hit;
		if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance, obstacleMask))
		{
			obstacleVector = FindBestDirectionToAvoidObstacle();
		}
		else
		{
			currentObstacleAvoidanceVector = Vector3.zero;
		}
		return obstacleVector;
	}

	private Vector3 FindBestDirectionToAvoidObstacle()
	{
		if(currentObstacleAvoidanceVector != Vector3.zero)
		{
			RaycastHit hit;
			if(!Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance, obstacleMask))
			{
				return currentObstacleAvoidanceVector;
			}
		}
		float maxDistance = int.MinValue;
		var selectedDirection = Vector3.zero;
		for (int i = 0; i < directionsToCheckWhenAvoidingObstacles.Length; i++)
		{

			RaycastHit hit;
			var currentDirection = myTransform.TransformDirection(directionsToCheckWhenAvoidingObstacles[i].normalized);
			if(Physics.Raycast(myTransform.position, currentDirection, out hit, assignedFlock.obstacleDistance, obstacleMask))
			{

				float currentDistance = (hit.point - myTransform.position).sqrMagnitude;
				if(currentDistance > maxDistance)
				{
					maxDistance = currentDistance;
					selectedDirection = currentDirection;
				}
			}
			else
			{
				selectedDirection = currentDirection;
				currentObstacleAvoidanceVector = currentDirection.normalized;
				return selectedDirection.normalized;
			}
		}
		return selectedDirection.normalized;
	}

	private bool IsInFOV(Vector3 position)
	{
		return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
	}
}