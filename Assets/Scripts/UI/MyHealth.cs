using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class MyHealth : MonoBehaviour
{

    [SerializeField] PhotonView PV;
    [SerializeField] TextMeshPro healthText;
    [SerializeField] PlayerController player;

    // Update is called once per frame
    void Update()
    {
        if (PV != null)
        {
            if (PV.IsMine)
            {
                if(healthText != null)
                {
                    if(player != null)
                    {
                        healthText.text = player.GetHealth().ToString();
                    }
                }
            }
        }
    }
}
