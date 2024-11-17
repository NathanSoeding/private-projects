using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
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
        if (dist < .4f)
        {
            pScr.Ungrapple();
            Vector2 dir = pScr.pos - new Vector2(transform.position.x, transform.position.y);
            pScr.vel = dir.normalized * 4;
        }

        if (dist > 15)
        {
            spawnScr.spikesCount -= 1;
            Destroy(gameObject);
        }
    }
}
