using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumiereBlanc_Rouge : MonoBehaviour
{
    Renderer ren;
    // Start is called before the first frame update
    void Start()
    {
        ren = GetComponent<Renderer>();
        ren.material.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
