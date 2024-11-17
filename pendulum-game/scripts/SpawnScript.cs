using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public GameObject cloud;
    public GameObject boost;
    public GameObject spike;
    public GameObject bouncy;
    public GameObject lowGrav;
    PlayerScript pScr;
    public int boostsCount = 0;
    public int spikesCount = 0;
    public int bouncyCount = 0;
    public int gravCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        pScr = GameObject.Find("Player").GetComponent<PlayerScript>();
        //Clouds();
        //for (int i = 0; i < 50; i++)
        //    Boost();
        //for (int i = 0; i < 150; i++)
        //    Spike();
    }

    void Clouds()
    {
        GameObject thisCloud = Instantiate(cloud, new Vector3(Random.Range(-10, 60), Random.Range(0, -40), -1), transform.rotation);
        moveObject cloudScr = thisCloud.GetComponent<moveObject>();
        if (Random.Range(0, 40) == 0)
            cloudScr.moveDirMax = new Vector2(Random.Range(-5, 5), Random.Range(-3, 3));
    }

    void Boost()
    {
        GameObject thisBoost = Instantiate(boost, new Vector3((int)Random.Range(pScr.pos.x - 20, pScr.pos.x + 21), (int)Random.Range(pScr.pos.y - 10, pScr.pos.y + 11), -1), transform.rotation);
        moveObject boostScr = thisBoost.GetComponent<moveObject>();
        if (Random.Range(0, 4) == 0)
            boostScr.moveDirMax = new Vector2(Random.Range(-5, 6), 0);
        else if (Random.Range(0, 4) == 0)
            boostScr.moveDirMax = new Vector2(0, Random.Range(-3, 4));

        // If spawned in sight of player its destroyed
        if (!(Mathf.Abs(thisBoost.transform.position.x - pScr.transform.position.x) < 13 && Mathf.Abs(thisBoost.transform.position.y - pScr.transform.position.y) < 7))
            boostsCount++;
        else
            Destroy(thisBoost);
    }

    void Spike()
    {
        GameObject thisSpike = Instantiate(spike, new Vector3((int)Random.Range(pScr.pos.x - 20, pScr.pos.x + 21), (int)Random.Range(pScr.pos.y - 10, pScr.pos.y + 11), -1), transform.rotation);
        moveObject spikeScr = thisSpike.GetComponent<moveObject>();
        if (Random.Range(0, 4) == 0)
            spikeScr.moveDirMax = new Vector2(Random.Range(-5, 6), 0);
        else if(Random.Range(0, 4) == 0)
            spikeScr.moveDirMax = new Vector2(0, Random.Range(-3, 4));

        // If spawned in sight of player its destroyed
        if (!(Mathf.Abs(thisSpike.transform.position.x - pScr.transform.position.x) < 13 && Mathf.Abs(thisSpike.transform.position.y - pScr.transform.position.y) < 7))
            spikesCount++;
        else
            Destroy(thisSpike);
    }

    void Bouncy()
    {
        GameObject thisBouncy = Instantiate(bouncy, new Vector3((int)Random.Range(pScr.pos.x - 20, pScr.pos.x + 21), (int)Random.Range(pScr.pos.y - 10, pScr.pos.y + 11), -1), transform.rotation);

        // If spawned in sight of player its destroyed
        if (!(Mathf.Abs(thisBouncy.transform.position.x - pScr.transform.position.x) < 14 && Mathf.Abs(thisBouncy.transform.position.y - pScr.transform.position.y) < 8))
            bouncyCount++;
        else
            Destroy(thisBouncy);
    }

    void LowGrav()
    {
        GameObject thisGrav = Instantiate(lowGrav, new Vector3((int)Random.Range(pScr.pos.x - 22, pScr.pos.x + 23), (int)Random.Range(pScr.pos.y - 12, pScr.pos.y + 13), -1), transform.rotation);

        // If spawned in sight of player its destroyed
        if (!(Mathf.Abs(thisGrav.transform.position.x - pScr.transform.position.x) < 17 && Mathf.Abs(thisGrav.transform.position.y - pScr.transform.position.y) < 11))
            gravCount++;
        else
            Destroy(thisGrav);
    }

    // Update is called once per frame
    void Update()
    {
        while (boostsCount < 5)
            Boost();
        while (spikesCount < 20)
            Spike();
        while (bouncyCount < 2)
            Bouncy();
        while (gravCount < 2)
            LowGrav();
    }
}
