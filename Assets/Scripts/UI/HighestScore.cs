using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
public class HighestScore : MonoBehaviourPunCallbacks
{

    [SerializeField] PlayerController controller;

    TextMeshPro highestScore;

    PlayerManager PM;

    private void Awake()
    {
        PM = controller.GetPlayerManager();
    }
    private void Start()
    {
        highestScore = GetComponent<TextMeshPro>();

        highestScore.text = PM.GetHighestScore().ToString();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("killer"))
        {
            StartCoroutine(WaitThenUpdateHighestScore());
        }
    }


    IEnumerator WaitThenUpdateHighestScore()
    {
        yield return new WaitForSeconds(0.2f);
        highestScore.text = controller.GetPlayerManager().GetHighestScore().ToString();
    }
}