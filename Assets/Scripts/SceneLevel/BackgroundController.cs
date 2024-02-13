using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public float velocidad;

    public GameObject ObjetoDebajo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (transform.position.z <= -13)
        {
            transform.position = ObjetoDebajo.transform.position + new Vector3(0, 0, 13);
        }
        
        transform.position += new Vector3(0, 0, velocidad * Time.deltaTime);
    }
}
