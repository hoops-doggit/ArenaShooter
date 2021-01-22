using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public int location;
    public void OnTriggerEnter(Collider other)
    {
        Vector3 position;
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (location == 0)
            {
                position = new Vector3(18.3f, 28f, -3f);   
            }
            else if (location == 1)
            {
                position = new Vector3(25f, 11.5f, 10.5f);
            }
            else if (location == 2)
            {
                position = new Vector3(24.8f, 11.5f, -47.8f);
            }
            else if (location == 3)
            {
                position = new Vector3(-17.6f, -1f, -17.3f);
            }
            else if (location == 4)
            {
                position = new Vector3(17.6f, 28f, -33.3f);
            }
            else if (location == 5)
            {
                position = new Vector3(47f, -1f, -17.3f);
            }
            else
            {
                position = new Vector3(18f, 93f, -17f);
            }
            player.transform.position = position;
        }
    }
}
