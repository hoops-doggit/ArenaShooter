using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public List<PlayerController> players = new List<PlayerController>();
    Vector3 jumpPadForce = new Vector3(0, 15, 0);

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
                p.AddForce(jumpPadForce);
            }
        }
    }
}


