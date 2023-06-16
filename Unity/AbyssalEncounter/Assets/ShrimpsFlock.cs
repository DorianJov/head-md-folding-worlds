using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

public class ShrimpsFlock : MonoBehaviour
{
    public float speed = 0.000003f;
    public float rotationSpeed = 0.0003f;
    Vector3 averageHeading;
    Vector3 averagePosition;
    public float neighbourMaxDistance = 0.1f;

    bool turning = false;
    // Start is called before the first frame update
    void Start()
    {
             
        // add random to speed of shrimp ?
        //speed = Random.Range(0.5f,1);
    }

    // Update is called once per frame
    void Update()   
    {
        /*if(Vector3.Distance(transform.position, Vector3.zero) >= globalFlock.tankSize)
        {
            turning = true;
        }
        else
            turning = false;
        if(turning)
        {
            Vector3 direction = Vector3.zero- transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  rotationSpeed * Time.deltaTime);
            //speed = Random.Range(0.5f,1);
        }
        */
        

        //Vector3 goalPos = globalFlock.goalPos;

        //dont apply flocking every time ? why ? idk
        //if(Random.Range(0,5)<1)
                  ApplyRules();
             
       transform.Translate(0,0,Time.deltaTime * speed); 
    }

    void ApplyRules(){

        GameObject[] gos;
        gos = globalFlock.allFish;

        Vector3 vcenter = Vector3.zero;
        Vector3 vavoid = Vector3.zero;

        //SPEED
         //float gSpeed = 0.1f;
        float gSpeed = 0.05f;
        

        //Vector3 goalPos = globalFlock.ObjectToFollow;
        Vector3 goalPos = globalFlock.goalPos;

        float dist;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
                if(go != this.gameObject)
                {
                    dist = Vector3.Distance(go.transform.position,this.transform.position);
                    if(dist <= neighbourMaxDistance)
                    {
                        vcenter += go.transform.position;
                        groupSize++;
                        //DIST 1.0f
                        if(dist < 0.3f)
                        {
                            vavoid = vavoid + (this.transform.position - go.transform.position);
                        }   

                         ShrimpsFlock anotherFlock = go.GetComponent<ShrimpsFlock>();
                        gSpeed = gSpeed + anotherFlock.speed;
            
                    }
                }           
        }

        if(groupSize > 0)
        {
             vcenter = vcenter/groupSize + (goalPos - this.transform.position);
            speed = gSpeed/groupSize;

             //Vector3 direction = (vcenter + vavoid) - transform.position;   
            Vector3 direction = (vcenter + vavoid/2) - transform.position;
            if(direction != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                                          Quaternion.LookRotation(direction),
                                                          rotationSpeed * Time.deltaTime);

        }
    }
}
