using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFieldScript : MonoBehaviour
{
    PlayerScript pScr;
    SpawnScript spawnScr;
    bool lowGrav = false;

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
        if (dist < 5)
        {
            lowGrav = true;
            pScr.g = -2;
        }

        if (lowGrav && dist > 5)
        {
            lowGrav = false;
            pScr.g = -10;
        }

        if (dist > 18)
        {
            spawnScr.gravCount -= 1;
            Destroy(gameObject);
        }
    }
}
