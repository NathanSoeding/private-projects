using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveObject : MonoBehaviour
{
    public Vector2 moveDirMax;
    public Vector2 moveDir = Vector2.zero;
    bool dirChange = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (dirChange)
            moveDir -= moveDirMax / 100;
        else
            moveDir += moveDirMax / 100;

        if (moveDir.magnitude > moveDirMax.magnitude)
            dirChange = !dirChange;

        transform.position = new Vector3(transform.position.x + moveDir.x / 50, transform.position.y + moveDir.y / 50, transform.position.z);
    }
}
