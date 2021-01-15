using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerManager : MonoBehaviourPunCallbacks
{
	PhotonView PV;
    public int PVViewID;

	GameObject controller;

    [SerializeField] string PlayerControllerName;

    PlayerStats pStats;

    [SerializeField] List<int> players = new List<int>();
    [SerializeField] List<int> scores = new List<int>();

    public int highestScore = 0;

    public int kills = 0;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
        PVViewID = PV.ViewID;
	}

	void Start()
	{
		if(PV.IsMine)
		{
			CreateController();

            //AddPlayerStats(PVViewID);
        }        
    }

	void CreateController()
	{
		Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
		controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", PlayerControllerName), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
	}

	public void Die(int killerViewID) // for when you get killed by another player
	{
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("killer", killerViewID);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        //aPV.RPC("RPC_UpdateScores", RpcTarget.All, killerViewID);
        PhotonNetwork.Destroy(controller);
		CreateController();
	}

    public void Die() // for when you kill yourself. There's Currently no penalty for killing yourself
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

    //[PunRPC]
    //void RPC_UpdateScores(int killerViewID)
    //{
    //    for (int i = 0; i < players.Count; i++)
    //    {
    //        if (players[i].viewID == killerViewID)
    //        {
    //            players[i].score ++;

    //            if(PV.ViewID == killerViewID)
    //            {
    //                pStats.score++;
    //            }
    //        }
    //    }
    //}

    //void AddPlayerStats(int ViewID)
    //{
    //    Debug.Log("mine =" + PV.ViewID);
    //    Debug.Log("incoming" + ViewID);

    //    bool alreadyAdded = false;

    //    for (int i = 0; i < players.Count; i++)
    //    {
    //        if (players[i].viewID == ViewID)
    //        {
    //            alreadyAdded = true;
    //        }
    //    }

    //    if (!alreadyAdded)
    //    {
    //        if(ViewID == PVViewID)
    //        {
    //            pStats = GetComponent<PlayerStats>();

    //            pStats.playerName = PV.Owner.NickName;
    //            pStats.viewID = PV.ViewID;

    //            players.Add(pStats);

    //            //after adding self to list, tell everyone you're here
    //            Hashtable hash = new Hashtable();
    //            hash.Add("addPlayer", PVViewID);
    //            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    //        }

    //        else
    //        {
    //            PlayerStats newPS = (PlayerStats)gameObject.AddComponent(typeof(PlayerStats));

    //            PlayerStats otherPlayer = PhotonView.Find(ViewID).gameObject.GetComponent<PlayerStats>();

    //            newPS.playerName = otherPlayer.playerName;

    //            newPS.viewID = ViewID;

    //            players.Add(newPS);
                

    //            //after adding someone tell them to add you back, just in case you got here before them
    //            Hashtable hash = new Hashtable();
    //            hash.Add("addPlayer", PVViewID);
    //            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    //        }
    //    }        
    //}

    void SomeoneGotAKill(int killerVID)
    {
        if (killerVID == PV.ViewID)
        {
            kills++;
        }
    }

    public int GetKills()
    {
        return kills;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        if (changedProps.ContainsKey("killer"))
        {
            //this is for updating local data from others
            SomeoneGotAKill((int)changedProps["killer"]);
        }

        //if (changedProps.ContainsKey("addPlayer"))
        //{
        //    AddPlayerStats((int)changedProps["addPlayer"]);
        //}

        if (changedProps.ContainsKey("killer"))
        {
            int killer = (int)changedProps["killer"];

            bool scoreExists = false;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == killer)
                {
                    scoreExists = true;
                    scores[i]++;
                }
            }

            if (!scoreExists)
            {
                players.Add(killer);
                scores.Add(1);
            }

            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i] > highestScore)
                {
                    highestScore = scores[i];
                }
            }
        }

    }

    public int GetHighestScore()
    {
        return highestScore;
    }



}