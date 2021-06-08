using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
	public static SpawnManager Instance;

	Spawnpoint[] spawnpoints;
    Spawnpoint tmp;

	void Awake()
	{
		Instance = this;
		spawnpoints = GetComponentsInChildren<Spawnpoint>();
	}

	public Transform GetSpawnpoint(int index)
	{
       return spawnpoints[index].transform;
	}

    public void ShuffleSpawnpoints()
    {
        for (int i = 0; i < spawnpoints.Length - 1; i++)
        {
            int rnd = Random.Range(i, spawnpoints.Length);
            tmp = spawnpoints[rnd];
            spawnpoints[rnd] = spawnpoints[i];
            spawnpoints[i] = tmp;
        }
    }


    public int GetValidSpawnPointIndex()
    {
        bool canSpawn = true;
        int spawnIndex = 0;

        ShuffleSpawnpoints();

        for(int i = 0; i< spawnpoints.Length; i++)
        {
            Collider[] colliders = Physics.OverlapSphere(spawnpoints[i].transform.position, 5f);
            spawnIndex = i;

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
