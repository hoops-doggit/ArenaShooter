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
    [SerializeField] AudioManager audio;
    [SerializeField] RecoilController recoil;
    [SerializeField] Vector3 recoilAmount;
    [SerializeField] float cooldownTime = 0.5f;

    PhotonView PV;

    bool gunReady = true;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
	{
        if (gunReady)
        {
		    Shoot();
        }
	}

    public override void StopUsing()
    {
        //
    }

    void Shoot()
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
		ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
		{
			hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage, player.PMViewID());
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
		}

        if (PV.IsMine)
        {
            player.BulletFlash(player.GetComponent<PlayerController>().GetItemIndex());
            audio.PlaySound(audio.playerFirePistol[0], 1.2f, 1.5f);
            gunReady = false;
            StartCoroutine(ShootCooldown());
        }
    }

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        gunReady = true;
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
        }
        if (!PV.IsMine)
        {
            player.BulletFlash(player.GetComponent<PlayerController>().GetItemIndex());
            //do audio here
        }
    }

    public void BulletFlash()
    {
        Vector3 rotation = bulletFlash.transform.eulerAngles;
        rotation.z += Random.Range(-360, 360);
        bulletFlash.transform.rotation = Quaternion.Euler(rotation);
        bulletFlash.SetActive(true);
        StartCoroutine(TurnOffBulletFlash());
    }

    IEnumerator TurnOffBulletFlash()
    {
        yield return new WaitForSeconds(0.1f);
        bulletFlash.SetActive(false);
    }

}
