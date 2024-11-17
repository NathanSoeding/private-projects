using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    PlayerScript player;
    Vector3 vel = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPos = new Vector3(player.pos.x + (player.vel.x / 10), player.pos.y + (player.vel.y / 10), -10);
        Vector3 connect = targetPos - transform.position;
        //transform.position += connect;

        //transform.position = new Vector3(player.pos.x, player.pos.y, -10);
        transform.position = Vector3.SmoothDamp(transform.position, player.transform.position, ref vel, .05f);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
