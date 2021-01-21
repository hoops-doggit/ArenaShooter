using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public int location;
    // 
    public void OnTriggerEnter(Collider other)
    {
        Vector3 position;
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (location == 1)
            {
                position = new Vector3(18.2f, 21.4f, 17.75f);
                
            }
            else if (location == 2)
            {
                position = new Vector3(18f, 28f, -33.42f);
            }
            else if (location == 3)
            {
                position = new Vector3(52.75f, -1f, -49.5f);
            }
            else
            {
                position = new Vector3(8.57f, 11.55f, -51f);
            }
            player.transform.position = position;
        }
    }
}
