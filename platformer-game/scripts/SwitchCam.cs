using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchCam : MonoBehaviour
{
    VirtualCam vCamScript;
    Player player;
    Manager manage;

    public int roomNr;
    public GameObject vCamNext;
    public GameObject vCamLast;
    public Vector2 roomRespawn;

    void Start()
    {
        vCamScript = GameObject.Find("Main Camera").GetComponent<VirtualCam>();
        player = GameObject.Find("Player").GetComponent<Player>();
        manage = GameObject.Find("Manager").GetComponent<Manager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            manage.currentRoom = roomNr;
            ChangeCam();
            manage.LoadMovingObjects();
        }
    }

    void ChangeCam()
    {
        vCamNext.SetActive(true);
        vCamLast.SetActive(false);
        vCamScript.transposer = vCamNext.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        player.respawnPoint = roomRespawn;
    }
}
