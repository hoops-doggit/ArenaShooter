using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.SceneManagement;
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
    public int IDofHighestScore;

    TextMeshPro middleTextBox;

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
            Cursor.lockState = CursorLockMode.Locked;
        }        
    }

	void CreateController()
	{
        int spawnIndex = SpawnManager.Instance.GetValidSpawnPointIndex();

        if(spawnIndex != -1)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint(spawnIndex);
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", PlayerControllerName), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        }
        else
        {
            Invoke("CreateController", 0.001f);
        }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

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

            for(int i = 0; i < scores.Count; i++)
            {
                if(scores[i] == highestScore)
                {
                    IDofHighestScore = players[i];
                }
            }

            CheckForWinner();

        }
    }

    public void SetMiddleTextBox(TextMeshPro textObj)
    {
        middleTextBox = textObj;

    }

    

    void CheckForWinner()
    {
        if(highestScore >= ModeDeathMatch.Instance.GetScoreLimit())
        {
            if (PV.IsMine)
            {
                middleTextBox.text = "The winner is " + IDofHighestScore.ToString();
                middleTextBox.gameObject.SetActive(true);
                Invoke("StartNextMap", 6);
            }
        }
    }

    void StartNextMap()
    {
        Scene thisScene = SceneManager.GetActiveScene();

        int arenaToLoad = thisScene.buildIndex;

        arenaToLoad++;

        if(arenaToLoad > SceneManager.sceneCountInBuildSettings -1)
        {
            arenaToLoad = 1;
        }

        PhotonNetwork.LoadLevel(arenaToLoad);
        PhotonNetwork.ConnectUsingSettings();
    }

    public int GetHighestScore()
    {
        return highestScore;
    }



}