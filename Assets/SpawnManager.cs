using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	public static SpawnManager Instance;

	Spawnpoint[] spawnpoints;

	void Awake()
	{
		Instance = this;
		spawnpoints = GetComponentsInChildren<Spawnpoint>();
	}

	public Transform GetSpawnpoint()
	{
        //if(Physics.SphereCast())
		return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
	}

    private int GetSpawnPoint()
    {
        return Random.Range(0, spawnpoints.Length);
    }
}
