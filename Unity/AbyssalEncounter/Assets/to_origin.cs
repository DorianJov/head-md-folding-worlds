using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class to_origin : MonoBehaviour
{

    Flock flock;

    bool stopMoving = false;
    bool lookTowardsTarget = false;
    bool lookbackwardsAndRun = false;

    public Animator anim;
    public GameObject TargetToAttack;
    float m_HorizontalMovement;
    GameObject enrironement;

    bool soundEnv = false;



    // Start is called before the first frame update
    void Start()
    {

        flock = GameObject.FindGameObjectWithTag("Flock").GetComponent<Flock>();
        enrironement = GameObject.Find("SonAmbiant");
        // transform.LookAt(Vector3.zero);
        //AudioSource[] sources = this.gameObject.GetComponents<AudioSource>();
        //sources[0].Play();

        anim = GetComponent<Animator>();
        Vector3 startPos = transform.position;
        Vector3 goalPos = new Vector3(0, 0, 0);
        //float duration = 15;
        float duration = Random.Range(10, 16);

        // Moves an object to the set position
        StartCoroutine(moveFromToCoroutine(startPos, goalPos, duration));
    }

    // Update is called once per frame
    void Update()

    {
        //Debug.Log("FromFISHc counter :  " + flock.howManyAreFollowing);
        //TargetToAttack = GameObject.Find("wescouz");
        //TargetToAttack = GameObject.Find("RedLight");
        TargetToAttack = GameObject.FindGameObjectWithTag("Shrimps");

        if (TargetToAttack == null)
        {
            TargetToAttack = GameObject.Find("wescouz");
        }


        //Debug.Log(TargetToAttack.transform.position);
        //transform.position = new Vector3(0, 0, 2) * Time.deltaTime;

        //Rotate to target
        if (lookTowardsTarget == true)
        {
            RotateTowards(TargetToAttack.transform.position);
        }

        //rotate backwards and leave
        if (lookbackwardsAndRun == true)
        {
            RotateTowardsBACK(TargetToAttack.transform.position);
            //RotateTowardsBACK(new Vector3(0, 0, 0));
        }

        if (soundEnv)
        {
            // SoundEnvironementReplay();
        }
    }

    IEnumerator moveFromToCoroutine(Vector3 start, Vector3 end, float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            Vector3 newPosition = Vector3.Lerp(start, end, (elapsedTime / time));

            // Calculate the direction vector and set the rotation
            Vector3 direction = (newPosition - transform.position).normalized;
            if (direction != Vector3.zero) // Check for zero direction vector
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = toRotation;
            }

            transform.position = newPosition;
            if (stopMoving == true)
            {
                yield break;
                //elapsedTime = 100;

            }
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the movement is completed by setting the final position to the target
        //Vector3 finalPosition = end;
        //finalPosition.y = transform.position.y;
        //transform.position = finalPosition;
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Limit")
        {
            // Do smth
            // Debug.Log("fish has passed!!!!!");

            if (flock.howManyAreFollowing <= 75)
            {
                PreparesToAttack();
            }
            else
            {

                RunAway();
            }




        }
    }

    void RunAway()
    {

        stopMoving = true;
        anim.SetFloat("preparesToAttckTransition", 1);
        AudioSource[] sources = this.gameObject.GetComponents<AudioSource>();
        sources[0].Stop();
        sources[1].Play();

        Invoke("StartRunaway", 4f);
    }

    void StartRunaway()
    {
        lookbackwardsAndRun = true;
    }


    void PreparesToAttack()
    {
        stopMoving = true;

        anim.SetFloat("preparesToAttckTransition", 1);

        AudioSource[] sources = this.gameObject.GetComponents<AudioSource>();
        sources[0].Stop();
        sources[1].Play();
        //soundEnv = true; 

        Invoke("StartAttack", 4f);
    }

    void StartAttack()
    {
        lookTowardsTarget = true;
    }


    float lerpPercent = 0f;
    float lerpSpeed = 1;
    bool hasLookedOnce = false;
    Quaternion targetRotation;
    bool playdasHasPlayed = false;

    float lerpPercent2 = 0f;

    public float smooth = 1f;

    private Vector3 targetAngles;

    public enum ChargeState
    {
        TETE_CHERCHEUSE,
        ATTACK,
        FUITE
    }

    private ChargeState chargeState;
    private Vector3 positionDebutAttack;

    void RotateTowards(Vector3 target)
    {
        //Debug.Log("Fish Follow:" + lookTowardsTarget);



        switch (chargeState)
        {
            case ChargeState.TETE_CHERCHEUSE:
                float distance = Vector3.Distance(transform.position, TargetToAttack.transform.position);
                targetRotation = Quaternion.LookRotation(TargetToAttack.transform.position - transform.position);
                if (distance <= 2)
                {
                    chargeState = ChargeState.ATTACK;
                    positionDebutAttack = transform.position;
                }
                break;
            case ChargeState.ATTACK:
                float distanceParcourue = Vector3.Distance(transform.position, positionDebutAttack);
                if (distanceParcourue >= 2.9f)
                {
                    chargeState = ChargeState.FUITE;
                    targetRotation = Quaternion.LookRotation(transform.right, Vector3.up);
                    Destroy(gameObject, 10f);
                }
                break;
            case ChargeState.FUITE:

                break;
            default:
                break;
        }


        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        transform.position += transform.forward * Time.deltaTime * 3;
        /*

                if (hasLookedOnce == false & distance>=2)
                {
                    targetRotation = Quaternion.LookRotation(TargetToAttack.transform.position - transform.position);


                      ///  hasLookedOnce = true;
                        //RunAway();
                        ///lookTowardsTarget = false;
                } else {
                    hasLookedOnce = true;
                        /*if (timeRemaining-4 > 0)
                    {
                    timeRemaining -= Time.deltaTime;
                    }else{
                        lookTowardsTarget = false;
                        hasLookedOnce = false;
                        lookbackwardsAndRun = true;
                    }

                    Debug.Log("FishDistance: " + distance);
                }


                */


        //lerpPercent = Mathf.MoveTowards(lerpPercent, 4f, Time.deltaTime * lerpSpeed);     
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpPercent);


        //Debug.Log(lerpPercent);

        if (lerpPercent >= 4f)
        {

            if (!playdasHasPlayed)
            {
                playdasHasPlayed = true;
            }

            //dash Speed
            transform.position += transform.forward * Time.deltaTime * 3;

            Destroy(this.gameObject, 20.0f);

        }
    }

    bool Lerped = false;
    Quaternion targetRotation2;
    void RotateTowardsBACK(Vector3 target)
    {

        Debug.Log("Fish Ce CASSE:");
        if (hasLookedOnce == false)
        {
            targetRotation2 = Quaternion.LookRotation(-transform.forward, Vector3.up);
            targetRotation = Quaternion.LookRotation(TargetToAttack.transform.position - transform.position);
            hasLookedOnce = true;

        }


        if (!Lerped)
        {
            lerpPercent = Mathf.MoveTowards(lerpPercent, 4f, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpPercent);

        }

        if (lerpPercent >= 4f)
        {
            Lerped = true;


            transform.position += transform.forward * Time.deltaTime * 2;



            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, 1f * Time.deltaTime);

            Destroy(this.gameObject, 20.0f);

        }

    }

    float timeRemaining = 5f;
    void SoundEnvironementReplay()
    {


        if (timeRemaining > 0)
        {
            AudioSource[] Envsources = enrironement.GetComponents<AudioSource>();
            Envsources[0].Stop();
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0;
            AudioSource[] Envsources = enrironement.GetComponents<AudioSource>();
            Envsources[0].Play();

        }


    }


}










