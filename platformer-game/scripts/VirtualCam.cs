using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCam : MonoBehaviour
{
    public CinemachineFramingTransposer transposer;
    SpriteRenderer playerSR;
    Player playerScript;

    public float lookAhead;
    public float upwardsCamBias;
    public float fallLookAhead;
    public float fallLookAheadStartVel;
    float playerLastYPos;
    float yOffset;

    void Start()
    {
        playerSR = GameObject.Find("player sprite").GetComponent<SpriteRenderer>();
        playerScript = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        int layerMask = LayerMask.GetMask("Ground");
        float rayLen = Physics2D.Raycast(playerScript.transform.position, Vector2.down, Mathf.Infinity, layerMask).distance - 0.6600049f;
        if (playerScript.Grounded())
            playerLastYPos = playerScript.transform.position.y;


        if (playerScript.transform.position.y > playerLastYPos - .1f && playerScript.transform.position.y - playerLastYPos < upwardsCamBias)
            yOffset = (upwardsCamBias - (playerScript.transform.position.y - playerLastYPos)) *1.15f;
        else
        {
            yOffset = Mathf.Max(-rayLen, -upwardsCamBias) + upwardsCamBias;
            if (playerScript.rb.velocity.y < -fallLookAheadStartVel)
                yOffset += (playerScript.rb.velocity.y + fallLookAheadStartVel) * fallLookAhead;
        }
        
        if (playerScript.pointRight)
            transposer.m_TrackedObjectOffset = new Vector3(lookAhead, yOffset, 0);
        else
            transposer.m_TrackedObjectOffset = new Vector3(-lookAhead, yOffset, 0);               
    }
}
