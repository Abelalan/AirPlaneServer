using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject airplane;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (airplane == null)
        {
            airplane = GameObject.Find("Airplane");
        }
        if (airplane != null)
        {
            Vector3 pos = airplane.transform.position + airplane.transform.up * 10 + airplane.transform.forward * -25;
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 5);
            Quaternion q = Quaternion.LookRotation(airplane.transform.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5);
        }
    }
}
