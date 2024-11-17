using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public int currentRoom;
    static float unloadDelay = 2;
    static float unloadTimer;
    GameObject[] moving;
    // Stores the objects from last room
    List<GameObject> unloadList = new List<GameObject>();
    // Stores the objects from current room to be reloaded
    List<GameObject> reloadList = new List<GameObject>();

    void Start()
    {
        moving = GameObject.FindGameObjectsWithTag("Moving");
        Cursor.visible = false;
    }

    void Update()
    {
        DisableAfterDelay();
    }

    public void LoadMovingObjects()
    {
        unloadList.Clear();
        reloadList.Clear();

        bool reload = false;
        foreach (GameObject obj in moving)
        {
            int objRoomNr = 0;
            if (obj.name == "Throwing star")
                objRoomNr = obj.GetComponent<ThrowingStar>().roomNr;
            else if (obj.name == "Crystal")
                objRoomNr = obj.GetComponent<Crystal>().roomNr;
            else if (obj.name == "Energy Field")
                objRoomNr = obj.GetComponent<EnergyField>().roomNr;

            if (objRoomNr != currentRoom)
            {
                unloadList.Add(obj);
            }
            else
            {
                unloadList.Remove(obj);
                reloadList.Add(obj);
                // Only reloads room if moving objects get (de)activated
                bool before = obj.activeSelf;
                obj.SetActive(true);
                if (before != obj.activeSelf)
                    reload = true;
            }
        }

        if (reload)
        {
            Reload(reloadList.ToArray());
            unloadTimer = unloadDelay;
        }
    }

    // Resets cycle of all objects given
    public void Reload(GameObject[] objList)
    {
        foreach (GameObject obj in objList)
        {
            if (obj.name == "Throwing star")
            {
                ThrowingStar objScript = obj.GetComponent<ThrowingStar>();
                objScript.travelTimer = objScript.travelTime;
                objScript.pauseTimer = objScript.pauseTime;
                // The points get swapped for the way back. It needs to be stored if they are swapped or not to reset the cycle
                if (objScript.pointsSwapped)
                {
                    Vector2 temp = objScript.pos1;
                    objScript.pos1 = objScript.pos2;
                    objScript.pos2 = temp;
                    objScript.pointsSwapped = false;
                }
            }
            else if (obj.name == "Crystal")
            {
                obj.GetComponent<Crystal>().RespawnCrystal();
            }
        }
    }

    void DisableAfterDelay()
    {
        if (unloadTimer > 0)
            unloadTimer -= Time.deltaTime;
        else
        {
            while (unloadList.Count > 0)
            {
                GameObject temp = unloadList[0];
                unloadList.RemoveAt(0);
                temp.SetActive(false);
            }
        }
    }
}
