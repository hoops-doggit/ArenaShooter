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

	public Transform GetSpawnpoint(int index)
	{
       return spawnpoints[index].transform;
	}


    public int GetValidSpawnPointIndex()
    {
        bool canSpawn = true;
        int spawnIndex = Random.Range(0, spawnpoints.Length); 

        for(int i = spawnIndex; i< spawnpoints.Length; i++)
        {
            Collider[] colliders = Physics.OverlapSphere(spawnpoints[i].transform.position, 5f);

            if (colliders.Length != 0)
            {
                foreach (Collider c in colliders)
                {
                    if (c.gameObject.tag == "Player")
                    {
                        canSpawn = false;
                    }
                }
            }
        }

        if (!canSpawn)
        {
            Debug.Log("can't spawn");
            return -1;
        }
        else
        {
            Debug.Log("CanSpawn");
            return spawnIndex;
        }
    }
}
