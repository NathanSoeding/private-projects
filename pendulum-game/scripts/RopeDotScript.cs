using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeDotScript : MonoBehaviour
{
    public Vector2 pos;
    public Vector2 lastPos;
    PlayerScript p;
    public float snapDist;
    public bool posLocked = false;
    public bool followPlayer = false;
    // Start is called before the first frame update
    void Start()
    {
        p = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {       
        if (posLocked)
        {
            pos = p.grabPoint;
        }
        else if (followPlayer)
        {
            pos = p.pos;
        }
    }
}
