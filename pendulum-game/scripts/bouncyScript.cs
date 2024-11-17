using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bouncyScript : MonoBehaviour
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
    void Update()
    {
        float dist = new Vector2(Mathf.Abs(pScr.transform.position.x - transform.position.x), Mathf.Abs(pScr.transform.position.y - transform.position.y)).magnitude;
        if (dist < 1.3f)
        {
            pScr.Ungrapple();
            Vector2 dir = pScr.pos - new Vector2(transform.position.x, transform.position.y);
            float oldVel = pScr.vel.magnitude;
            if (oldVel < 8)
                oldVel = 8;
            pScr.vel = dir.normalized * oldVel;
        }

        if (dist > 15)
        {
            spawnScr.bouncyCount -= 1;
            Destroy(gameObject);
        }
    }
}
