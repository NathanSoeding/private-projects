using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public float paralexStrength;

    Transform camPos;
    Vector3 camStartPos;
    Vector3 starPos;

    void Start()
    {
        camPos = GameObject.Find("Main Camera").GetComponent<Transform>();

        camStartPos = camPos.position;
        starPos = transform.position;
    }

    void Update()
    {
        transform.position = starPos - (camStartPos - camPos.position) * paralexStrength;
    }
}
