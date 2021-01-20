using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
public class MyScore : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerController controller;
    [SerializeField] PhotonView PV;

    TextMeshPro myScore;

    PlayerManager PM;

    private void Awake()
    {
        PM = controller.GetPlayerManager();
    }
    private void Start()
    {
        myScore = GetComponent<TextMeshPro>();

        myScore.text = PM.GetKills().ToString();
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            myScore.text = PM.GetKills().ToString();
        }
    }

}