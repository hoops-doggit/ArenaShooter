using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ModeDeathMatch : MonoBehaviourPun
{
    public static ModeDeathMatch Instance;
    public List<PlayerStats> players = new List<PlayerStats>();

    [SerializeField] int scoreLimit = 25;



    PhotonView PV;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;        

        PV = GetComponent<PhotonView>();
    }

    public void AddPlayer(string myName)
    {
        Debug.Log(myName);
        PV.RPC("RPC_AddPlayer", RpcTarget.All, myName);
    }

    [PunRPC]
    void RPC_AddPlayer(string myName)
    {
        PlayerStats me = (PlayerStats)gameObject.AddComponent(typeof(PlayerStats));
        me.playerName = myName;
        players.Add(me);
    }


    public void UpdateScore(int killersViewID)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, killersViewID);
    }

    [PunRPC]
    void RPC_UpdateScore(int killersViewID)
    {
        PhotonView doerOfDamage = PhotonView.Find(killersViewID);
        string damageDoer = doerOfDamage.Owner.NickName;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerName == damageDoer)
            {
                players[i].score++;
            }
        }
    }

    public int GetScoreLimit()
    {
        return scoreLimit;
    }

    


    
}
