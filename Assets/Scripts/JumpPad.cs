using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{

    List<PlayerController> players = new List<PlayerController>();

    [SerializeField] JumpPadStats jps;


    private void Awake()
    {
        if(jps == null)
        {
            jps = GetComponentInParent<JumpPadStats>();
        }

        if(jps == null)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

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

    private void FixedUpdate()
    {
        if(players.Count > 0)
        {
            foreach(PlayerController p in players)
            {
                if (jps != null)
                {
                    Vector3 jumpPadForce = transform.up * jps.upForce;

                    p.AddForce(jumpPadForce);
                }                
            }
        }
    }
}


