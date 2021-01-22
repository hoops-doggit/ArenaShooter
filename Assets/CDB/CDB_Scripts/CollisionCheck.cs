using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    /* Checks if player's next position is in wall,
    If so: prevent next move from happening */
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.SetWalled();
        }
    }
}
