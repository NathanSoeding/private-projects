using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyRebinds : MonoBehaviour
{
    Player playerScript;
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {

    }

    private void OnGUI()
    {
        if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
        {
            //pressedKey = Event.current.keyCode;
        }
    }
}
