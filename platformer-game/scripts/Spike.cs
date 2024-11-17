using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    CompositeCollider2D col;
    GameObject player;
    BoxCollider2D playerCollider;
    Player playerScript;
    public bool isEnergyField;

    void Start()
    {
        player = GameObject.Find("Player");
        playerCollider = player.GetComponent<BoxCollider2D>();
        playerScript = player.GetComponent<Player>();
        col = playerScript.col;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Used to check if players hitbox is inside ground or wall
        Vector3 pointLeftCheck = new Vector3(playerScript.transform.position.x - .215f, playerScript.transform.position.y - .65f, playerScript.transform.position.z);
        Vector3 pointRightCheck = new Vector3(playerScript.transform.position.x + .215f, playerScript.transform.position.y - .65f, playerScript.transform.position.z);

        if (collision == playerCollider && !(Physics2D.OverlapPoint(pointLeftCheck) == col || Physics2D.OverlapPoint(pointRightCheck) == col) && (!isEnergyField || !playerScript.dashing))
        {
            playerScript.Death();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Used to check if players hitbox is inside ground or wall
        Vector3 pointLeftCheck = new Vector3(playerScript.transform.position.x - .215f, playerScript.transform.position.y - .65f, playerScript.transform.position.z);
        Vector3 pointRightCheck = new Vector3(playerScript.transform.position.x + .215f, playerScript.transform.position.y - .65f, playerScript.transform.position.z);

        if (collision == playerCollider && !(Physics2D.OverlapPoint(pointLeftCheck) == col || Physics2D.OverlapPoint(pointRightCheck) == col) && (!isEnergyField || !playerScript.dashing))
        {
            playerScript.Death();
        }
    }
}
