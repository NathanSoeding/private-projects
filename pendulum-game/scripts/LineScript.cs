using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineScript : MonoBehaviour
{
    LineRenderer line;
    PlayerScript pScr;
    public Material ropeMat;
    public float lineWidth;
    // Start is called before the first frame update
    void Start()
    {
        pScr = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        DestroyLines();

        for (int i = 0; i < pScr.ropeDots.Count; i++)
        {
            line = new GameObject("Lines").AddComponent<LineRenderer>();
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.useWorldSpace = true;
            line.numCapVertices = 5;
            line.material = ropeMat;
            line.tag = "Lines";

            line.positionCount = pScr.ropeDots[i].Count;

            for (int j = 0; j < pScr.ropeDots[i].Count; j++)
            {
                line.SetPosition(j, pScr.ropeDots[i][j].transform.position);
            }
        }
    }

    void DestroyLines()
    {
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Lines");
        foreach (GameObject l in lines)
        {
            Destroy(l);
        }
    }
}
