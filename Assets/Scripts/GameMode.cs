using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameMode : MonoBehaviourPun
{
    public enum Mode { Deathmatch }
    public Mode gameMode = Mode.Deathmatch;
    public static GameMode Instance;


    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
