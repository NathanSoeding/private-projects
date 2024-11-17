using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostScript : MonoBehaviour
{
    PlayerScript pScr;
    SpawnScript spawnScr;

    // Start is called before the first frame update
    void Start()
    {
        pScr = GameObject.Find("Player").GetComponent<PlayerScript>();
        spawnScr = GameObject.Find("Spawner").GetComponent<SpawnScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float vertDist = Mathf.Abs(transform.position.y - pScr.pos.y);
        float horDist = Mathf.Abs(transform.position.x - pScr.pos.x);
        if (vertDist < .5f && horDist < .5f)
        {
            if (pScr.grappled)
            {
                pScr.boost = true;
            }
            else
            {
                float oldVel = pScr.vel.magnitude;
                pScr.vel = pScr.vel.normalized * (oldVel + pScr.boostAmount);
            }
            spawnScr.boostsCount -= 1;
            Destroy(gameObject);
        }

        if (vertDist > 10 || horDist > 20)
        {
            spawnScr.boostsCount -= 1;
            Destroy(gameObject);
        }
    }
}
