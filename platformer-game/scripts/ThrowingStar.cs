using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingStar : MonoBehaviour
{
    GameObject player;
    Player playerScript;
    BoxCollider2D playerCollider;
    Animator anim;
    SpriteRenderer sr;

    public int roomNr;
    public Vector2 travelVector;
    public Vector2 pos1;
    public Vector2 pos2;
    public float travelTime;
    public float pauseTime;
    public float travelTimer;
    public float pauseTimer;
    public bool pointsSwapped;

    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        playerCollider = player.GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        pos1 = transform.position;
        pos2 = pos1 + travelVector;
    }

    void Awake()
    {
        travelTimer = travelTime;
        pauseTimer = pauseTime;
    }

    // Makes object moove along line between 2 points with pause time on points
    void FixedUpdate()
    {
        if (travelTimer > 0 && pauseTimer >= 0)
        {
            travelTimer -= Time.deltaTime;
            float relativeTime = travelTimer / travelTime;
            Vector2 newPos = pos1 + (pos2 - pos1) * Smooth(relativeTime);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            anim.SetBool("Spin", true);
            // Animation gets faster the faster the star2
            anim.speed = .2f + .8f * (-.5f * Mathf.Cos(2 * relativeTime * Mathf.PI) + .5f);
        }
        else if (travelTimer < 0 && pauseTimer > 0)
        {
            pauseTimer -= Time.deltaTime;
            anim.SetBool("Spin", false);
        }
        else if (travelTimer < 0 && pauseTimer <= 0)
        {
            travelTimer = travelTime;
            pauseTimer = pauseTime;
            Vector2 temp = pos1;
            pos1 = pos2;
            pos2 = temp;
            pointsSwapped = !pointsSwapped;
            //anim.SetTrigger("TrSpin");
            //sr.flipX = !sr.flipX;
        }
    }

    // Smoothly from (0, 1) to (1, 0)
    float Smooth(float x)
    {
        return .5f * Mathf.Cos(x * Mathf.PI) + .5f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == playerCollider)
        {
            playerScript.Death();
        }
    }
}
