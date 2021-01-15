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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("killer"))
        {
            if((int)changedProps["killer"] == PM.PVViewID)
            {
                myScore.text = PM.GetKills().ToString();
            }            
        }
    }
}