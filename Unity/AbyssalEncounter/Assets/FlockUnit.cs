using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockUnit : MonoBehaviour
{
	[SerializeField] private float FOVAngle;
	[SerializeField] private float smoothDamp;
	[SerializeField] private LayerMask obstacleMask;
	[SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles;

	private List<FlockUnit> cohesionNeighbours = new List<FlockUnit>();
	private List<FlockUnit> avoidanceNeighbours = new List<FlockUnit>();
	private List<FlockUnit> aligementNeighbours = new List<FlockUnit>();
	private Flock assignedFlock;
	private Vector3 currentVelocity;
	private Vector3 currentObstacleAvoidanceVector;
	private float speed;

	private bool touched = false;
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

	public void AssignFlock(Flock flock)
	{
		assignedFlock = flock;
	}

	public void InitializeSpeed(float speed)
	{
		this.speed = speed * -1;
	}



	bool glowingShrimp = false;
	void OnTriggerEnter(Collider other)
	{

		if (other.tag == "Monstre" && isAlive)
		{
			isAlive = false;
			// In the List<>
			assignedFlock.allUnits.Remove(this);
			// in the heirachy
			this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false; // desactiver le visuel le temps de mourir

			//Debug.Log("Make a noise!!!!!");
			//this.gameObject.GetComponent<AudioSource>().Play();
			AudioSource[] sources = this.gameObject.GetComponents<AudioSource>();
			//if (sources.Length > 0) {
			//sources[(int)UnityEngine.Random.Range(0,sources.Length)].Play();
			sources[0].Play();
			//}

			Destroy(this.gameObject, 5.0f); // le temps de audio
		}


		if (other.tag == "FollowMe" && !amIFollowingPlayer)
		{

			// Debug.Log("FollowMe!!!!!");
			AudioSource[] sources = this.gameObject.GetComponents<AudioSource>();
			sources[(int)UnityEngine.Random.Range(2, sources.Length)].Play();

			touched = true;
			amIFollowingPlayer = true;
			glowingShrimp = true;
			this.gameObject.tag = "Shrimps";








		}

	}


	float timeRemaining = 0.15f;
	void makeMeGLOWfor()
	{

		if (timeRemaining > 0)
		{
			this.gameObject.GetComponentInChildren<Renderer>().material = crevetteMaterialTouching;
			timeRemaining -= Time.deltaTime;
		}
		else
		{
			timeRemaining = 0;
			//Debug.Log("Time has run out!");
			//Debug.Log("PROUUUUT");
			this.gameObject.GetComponentInChildren<Renderer>().material = crevetteMaterialFollowing;
			glowingShrimp = false;

		}


	}



	void start()
	{
		//Material material = new Material(Shader.Find("Crevette"));
		//material.SetColor("Color_C6F9B478", Color.blue);
		//this.gameObject.GetComponent<Renderer>().material = material;




		goalPos = Flock.assignedBasicPos;

	}

	bool hasRead = false;

	public void MoveUnit()
	{
		//read once
		if (!hasRead)
		{
			if (!playIdleSound)
			{
				AudioSource[] sources = this.gameObject.GetComponents<AudioSource>();
				sources[1].Stop();
			}
			//Debug.Log("I AM BORN");
			//Debug.Log("AM I Playing a sound ?" + playIdleSound);
			//Debug.Log("AM I Playing a sound ?" + playIdleSound)
			goalPos = Flock.assignedBasicPos;
			hasRead = true;
		}

		if (glowingShrimp)
		{
			makeMeGLOWfor();

		}
		if (assignedFlock.allUnits.Count == 2 & !touched)
		{
			goalPos = Flock.ExperienceStartPoint;
		}

		///if has touched makes the shrimps follows the users hand
		if (touched)
		{
			goalPos = Flock.goalPos;
		}

		if (touched & Flock.endingSceneIsPlaying)
		{
			goalPos = Flock.EndingPos;
		}
		/*
        //if shrimps got hit by the fish destroy them and play sound
        if(fishHit){
            //destroy()
        }*/

		FindNeighbours();
		CalculateSpeed();



		//Vector3 goalPos = Flock.goalPos;

		var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
		var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
		var aligementVector = CalculateAligementVector() * assignedFlock.aligementWeight;

		//if(amIFollowingPlayer){}
		//boundsVector = CalculateBoundsVector() * 10;
		var boundsForceWhenFollowing = assignedFlock.boundsWeight;

		if (amIFollowingPlayer)
		{
			boundsForceWhenFollowing = 10;
		}
		var boundsVector = CalculateBoundsVector() * boundsForceWhenFollowing;





		//I NEEED PERFORMANCES
		//var obstacleVector = CalculateObstacleVector() * assignedFlock.obstacleWeight;

		//var moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector + obstacleVector + goalPos;

		var moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector;
		moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);

		float distance = Vector3.Distance(transform.position, goalPos);


		if (!Flock.endingSceneIsPlaying)
		{


			if (amIFollowingPlayer || assignedFlock.allUnits.Count == 2 & !amIFollowingPlayer)
			{
				moveVector = moveVector.normalized * speed * (distance * assignedFlock.distanceAdditionalSpeed);
			}
			else
			{

				//Debug.Log("allunits: " + assignedFlock.allUnits.Count);
				moveVector = moveVector.normalized * speed;
			}

		}
		else
		{
			touched = true;
			moveVector = moveVector.normalized * speed * ((distance / 20) * assignedFlock.distanceAdditionalSpeed);
		}



		if (moveVector == Vector3.zero)
			moveVector = transform.forward;

		myTransform.forward = moveVector;
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
				if (currentNeighbourDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
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
		if (currentObstacleAvoidanceVector != Vector3.zero)
		{
			RaycastHit hit;
			if (!Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance, obstacleMask))
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
			if (Physics.Raycast(myTransform.position, currentDirection, out hit, assignedFlock.obstacleDistance, obstacleMask))
			{

				float currentDistance = (hit.point - myTransform.position).sqrMagnitude;
				if (currentDistance > maxDistance)
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