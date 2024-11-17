using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Vector2 pos;
    Vector2 lastPos;
    public Vector2 vel;
    public Vector2 acc;
    public float g;
    public bool grappled = false;
    public Vector2 grabPoint;
    Vector2 grabPointChange;
    public GameObject grabObj;
    public bool objAssigned = false;
    public GameObject ropeDot;
    public List<List<GameObject>> ropeDots = new List<List<GameObject>>();          // Saves all ropes and for every rope all points it consists of
    public float dotsDist;
    public float releaseVelMultiplier;
    public bool boost = false;          // Used to boost velocity in grappled state
    public float boostAmount;

    // Start is called before the first frame update
    void Start()
    {
        ropeDots.Add(new List<GameObject>());
        ropeDots.Add(new List<GameObject>());
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    void Inputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grappled = true;
            SpawnRope();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!(ropeDots[0].Count == 0))
                Ungrapple();
            vel *= releaseVelMultiplier;
        }
    }

    public void Ungrapple()
    {
        grappled = false;
        GameObject point;
        if (ropeDots[0].Count > 0)
        {
            point = ropeDots[0][0];
            point.GetComponent<RopeDotScript>().posLocked = false;
        }
        ropeDots.Insert(0, new List<GameObject>());
    }

    private void FixedUpdate()
    {
        calcGrabPoint();
        UpdateRope();  
        // Updates velocity while grappled
        if (grappled)
        {
            pos = ropeDots[0][ropeDots[0].Count - 1].GetComponent<RopeDotScript>().pos;
            vel = (pos - lastPos) * 50;
        }
        else
        {
            vel += acc / 50;
            pos += vel / 50;
        }
        // Gravity froce
        acc = new Vector2(0, g); 

        lastPos = pos;
    }

    void calcGrabPoint()
    {
        if (!grappled)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            grabPoint = new Vector2(mousePos.x, mousePos.y);
        }

        if (objAssigned)
        {
            moveObject cloudScr = grabObj.GetComponent<moveObject>();
            grabPointChange = cloudScr.moveDir;
        }
           
        if (grappled)
        {
            grabPoint += grabPointChange / 50;
        }
    }

    // Spawns rope connecting the player and grab point
    void SpawnRope()
    {
        Vector2 connect = pos - grabPoint;
        float grabDist = connect.magnitude;
        int dotNumber = (int)(grabDist / dotsDist + 1);
        for (int i = 0; i < dotNumber; i++)
        {
            GameObject currentDot = Instantiate(ropeDot, transform.position, transform.rotation);
            RopeDotScript dotScr = currentDot.GetComponent<RopeDotScript>();

            dotScr.pos = grabPoint + connect.normalized * i * dotsDist;
            // Gives the points velocity in the same direction of the player but lower the closer they are to grab point
            dotScr.lastPos = dotScr.pos - (vel / 50) * (i / (float)dotNumber);

            if (i == 0)
            {
                dotScr.posLocked = true;
            }
            ropeDots[0].Add(currentDot);
        }
    }

    void UpdateRope()
    {
        // Loops through every rope
        for (int i = 0; i < ropeDots.Count; i++)
        {
            // Applies gravity and calcs new position for every point
            foreach (GameObject point in ropeDots[i])
            {
                RopeDotScript pScr = point.GetComponent<RopeDotScript>();

                Vector2 v = pScr.pos - pScr.lastPos;
                if (boost && i == 0)
                {
                    float oldVel = v.magnitude;
                    v = v.normalized * (oldVel + boostAmount / 50);
                }
                v.y += g / 2500;
                v *= 1.003f;

                pScr.lastPos = pScr.pos;

                pScr.pos += v;
                point.transform.position = new Vector3(pScr.pos.x, pScr.pos.y, transform.position.z);  
            }
            boost = false;

            // The rope the player is attached to is simulated better
            int iterations = 50;
            if (i == 0 && grappled)
                iterations = 150;

            // Pushes every point of the rope outwards to keep the length every frame
            for (int j = 0; j < iterations; j++)
            {
                for (int n = 0; n < ropeDots[i].Count - 1; n++)
                {
                    GameObject dotA = ropeDots[i][n];
                    GameObject dotB = ropeDots[i][n + 1];
                    RopeDotScript a = dotA.GetComponent<RopeDotScript>();
                    RopeDotScript b = dotB.GetComponent<RopeDotScript>();

                    Vector2 connect = new Vector2(b.pos.x - a.pos.x, b.pos.y - a.pos.y);
                    float dist = connect.magnitude;
                    float error = dist - dotsDist;
                    Vector2 changeAmount = connect.normalized * error * 0.5f;

                    if (a.posLocked || a.tag == "first")
                        b.pos -= 2 * changeAmount;
                    else if (b.posLocked || b.tag == "first")
                        a.pos += 2 * changeAmount;
                    // Increases the weight of the point the player is attached to while grappled
                    else if (i == ropeDots.Count && n == ropeDots[i].Count - 2 && grappled)
                    {
                        a.pos += 1.95f * changeAmount;
                        b.pos -= 0.05f * changeAmount;
                    }
                    else
                    {
                        a.pos += changeAmount;
                        b.pos -= changeAmount;
                    }
                }
            }
        }
        // Runs other calculations for ropes
        DeleteRopes();
        SplitRopes();
    }

    // Delete ropes depending on the number of rope dots that exist
    void DeleteRopes()
    {
        int countDots = 0;
        foreach (List<GameObject> rope in ropeDots)
            for (int i = 0; i < rope.Count; i++)
                countDots++;

        if (countDots > 100)
        {
            ropeDots.RemoveAt(ropeDots.Count - 1);
        }
    }

    // Loops through ropeDots and spikes to check if the rope needs to be cut
    void SplitRopes()
    {
        foreach (GameObject dot in ropeDots[0])
        {
            if (!ropeDots[1].Contains(dot))
            {
                GameObject[] spikes = GameObject.FindGameObjectsWithTag("spike");
                foreach (GameObject spike in spikes)
                {
                    Vector2 connect = new Vector2(spike.transform.position.x - dot.transform.position.x, spike.transform.position.y - dot.transform.position.y);
                    float dist = connect.magnitude;
                    if (dist < .2f)
                    {
                        Ungrapple();
                        // Splits the rope in 2 parts
                        ropeDots.Insert(1, new List<GameObject>());
                        int index = ropeDots[2].IndexOf(dot);
                        int iterations = ropeDots[2].Count - index;
                        for (int i = 0; i < iterations; i++)
                            ropeDots[1].Add(ropeDots[2][index + i]);
                    }
                }
            }
        }
        foreach (GameObject dot in ropeDots[1])
        {
            ropeDots[2].Remove(dot);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 velNor = vel.normalized;
        //Gizmos.DrawSphere(new Vector3(pos.x + velNor.x, pos.y + velNor.y, 0), 0.1f);
        //Gizmos.DrawSphere(new Vector3(grabPoint.x, grabPoint.y, 0), 0.1f);

        Gizmos.color = Color.black;
        foreach (List<GameObject> rope in ropeDots)
        {
            foreach (GameObject dot in rope)
            {
                //Gizmos.DrawSphere(new Vector3(dot.transform.position.x, dot.transform.position.y, -1), 0.1f);
            }
        }
    }
}
