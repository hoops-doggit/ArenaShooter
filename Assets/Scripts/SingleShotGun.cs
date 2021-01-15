using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SingleShotGun : Gun
{
	[SerializeField] Camera cam;
    [SerializeField] PlayerController player;
    [SerializeField] GameObject bulletFlash;

    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
	{
		Shoot();
	}

	void Shoot()
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
		ray.origin = cam.transform.position;
        //GameObject bulletImpact = Instantiate(bulletImpactPrefab, ray.origin, Quaternion.identity);
        //bulletImpact.name = "origin";

        if (Physics.Raycast(ray, out RaycastHit hit))
		{
			hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage, player.PMViewID());
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
		}
	}

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            GameObject bulletImpact = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpact, 10f);
            bulletImpact.transform.SetParent(colliders[0].transform);
            player.BulletFlash(player.GetComponent<PlayerController>().GetItemIndex());
        }
    }

    public void BulletFlash()
    {
        bulletFlash.SetActive(true);
        StartCoroutine(TurnOffBulletFlash());
    }

    IEnumerator TurnOffBulletFlash()
    {
        yield return new WaitForSeconds(0.1f);
        bulletFlash.SetActive(false);
    }

}
