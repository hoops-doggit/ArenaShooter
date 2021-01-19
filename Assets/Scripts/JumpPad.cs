using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    List<PlayerController> players = new List<PlayerController>();
    public float upForce = 15;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            players.Add(other.GetComponent<PlayerController>());            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            players.Remove(other.GetComponent<PlayerController>());
        }
    }

    private void Update()
    {
        if(players.Count > 0)
        {
            foreach(PlayerController p in players)
            {
                Vector3 jumpPadForce = Vector3.up * upForce;

                p.AddForce(jumpPadForce);
            }
        }
    }
}


