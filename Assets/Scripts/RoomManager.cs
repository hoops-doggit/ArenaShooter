using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public static RoomManager Instance;
    [SerializeField] List<string> playerManagers;

	void Awake()
	{
		if(Instance)
		{
            Destroy(gameObject);
            return;            
		}

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnEnable()
	{
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if(scene.buildIndex > 0) // We're in the game scene
		{
			PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", playerManagers[scene.buildIndex -1]), Vector3.zero, Quaternion.identity);
		}
	}

    public void DestorySelf()
    {
        Destroy(gameObject);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}