using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class MyScore : BouncyUi
{
    [SerializeField] PlayerController controller;
    [SerializeField] PhotonView PV;
    [SerializeField] float bounceDuration;
    [SerializeField] float bounceAmount;
    private int kills;
    private int previousKills;


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
            previousKills = kills;
            kills = PM.GetKills();
            if(previousKills < kills)
            {
                myScore.text = kills.ToString();
                BounceUI(bounceDuration, bounceAmount);
            }            
        }        
    }

}