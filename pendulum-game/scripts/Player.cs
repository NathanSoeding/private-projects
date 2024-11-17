using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 pos;
    public Vector2 vel;
    public Vector2 acc;
    public float airRes;
    bool grappled = false;
    float grabDist;
    float pastHeight;
    Vector2 grabPoint;
    bool plus = true;
    bool boost = false;
    GameObject[] clouds;

    public Vector2 objectVel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        // Excluding hooks
        bool below = grabPoint.y < pos.y;
        bool behind = (vel.x > 0 && pos.x > grabPoint.x) || (vel.x < 0 && pos.x < grabPoint.x);
        if (Input.GetMouseButtonDown(0))
            grappled = true;
        if (Input.GetMouseButtonUp(0))
        {
            grappled = false;
            vel += objectVel;
        }
        if (Input.GetKeyDown("space"))
            boost = true;
        if (Input.GetKeyUp("space"))
            boost = false;
    }

    private void FixedUpdate()
    {
        Vector2 playerHook = grabPoint - pos;
        if (grappled && Vector2.Angle(vel, playerHook) > 89.9f)
        {
            // Test moovable objects
            grabPoint += objectVel * 0.02f;
            pos += objectVel * 0.02f;


            // Calculation for next position on Cirlce
            float ang = Vector2.Angle(Vector2.left, pos - grabPoint);
            float angChange = ((vel.magnitude / 50) / (2 * Mathf.PI * grabDist)) * 360;
            // Swinging backwards
            if (vel.x < 0)
                angChange = -angChange;
            float newAng = ang + angChange;
            float angRad = (newAng * Mathf.PI) / 180;
            Vector2 angToVector = new Vector2(-Mathf.Cos(angRad), -Mathf.Sin(angRad));
            angToVector = angToVector.normalized * grabDist;

            pos = grabPoint + angToVector;

            if (newAng < 4 || newAng > 176)
            {
                grappled = false;
                vel += objectVel;
            } 

            // Calculation for new velocity
            Vector2 playerToP = grabPoint - pos;
            float mPer = playerToP.y / playerToP.x;
            float m = -1 / mPer;
            Vector2 newVel = new Vector2(1, m);
            float velAng = Vector2.Angle(vel, newVel);
            bool behind = (vel.x > 0 && pos.x > grabPoint.x) || (vel.x < 0 && pos.x < grabPoint.x);
            if (velAng < 170 && velAng > 10 && behind)
                plus = !plus;

            float dHeight = pos.y - pastHeight - (objectVel.y/50);
            float velM = vel.magnitude;
            velM = Mathf.Sqrt(Mathf.Abs(2 * acc.y * dHeight + (velM * velM)));

            // Direction of velocity
            if (vel.y > 0 && velM < 0.2f)
                plus = !plus;  
            if (!plus)   
                velM = -velM;
            
            velM *= Mathf.Abs(Mathf.Cos((velAng * Mathf.PI) / 180));
            vel = new Vector2(1, m).normalized * velM;
        }
        else
        {
            // s = v * t
            pos += vel / 50;
            // v = a * t
            vel += acc / 50;

            // Calulation for grab point
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerToP;
            if (!grappled)
            {
                GameObject[] clouds = GameObject.FindGameObjectsWithTag("cloud");
                float closestDist = Mathf.Infinity;
                GameObject closest = clouds[0];
                foreach (GameObject cloud in clouds)
                {
                    Vector3 cloudPos = cloud.transform.position;
                    Vector2 mouseToCloud = new Vector2(cloudPos.x - mousePos.x, cloudPos.y - mousePos.y);
                    if (mouseToCloud.magnitude < closestDist)
                    {
                        closestDist = mouseToCloud.magnitude;
                        closest = cloud;
                    }
                }
                grabPoint = new Vector2(closest.transform.position.x, closest.transform.position.y);
                playerToP = new Vector2(mousePos.x, mousePos.y) - pos;
                
            }
            else
            {
                playerToP = grabPoint - pos;
            }

            grabDist = playerToP.magnitude;
        }
        pastHeight = pos.y;


        // Air resistance
        float v = vel.magnitude;
        v -= airRes * 0.02f * (v * v);
        // Boost
        if (boost && grappled)
            v += 0.2f;

        vel = vel.normalized * v;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 velNor = vel.normalized;
        Gizmos.DrawSphere(new Vector3(pos.x + velNor.x, pos.y + velNor.y, 0), 0.1f);
        Gizmos.DrawSphere(new Vector3(grabPoint.x, grabPoint.y, 0), 0.1f);
    }
}
