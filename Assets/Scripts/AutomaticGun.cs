using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AutomaticGun : Gun
{
    [Header("Boring Stuff")]
	[SerializeField] Camera cam;
    [SerializeField] PlayerController player;
    [SerializeField] AudioManager audio;

    [Header("Customization")]
    [SerializeField] GameObject bulletFlash;
    [SerializeField] float timeBetweenBullets = 0.1f;
    [SerializeField] Transform reloadAnimTarget;
    [SerializeField] Transform reloadAnimGoal;
    private Vector3 startPos;
    private float reloadAnimTimer = 0;

    [Header("Recoil Settings")]
    [SerializeField] RecoilController recoilControl;
    [SerializeField] Vector3 recoilAmount = new Vector3(3,3,3);

    PhotonView PV;
    bool canShoot = true;
    bool shooting = false;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        startPos = reloadAnimTarget.localPosition;
    }

    public override void Use()
	{
        shooting = true;
	}

    public override void StopUsing()
    {
        shooting = false;
        Debug.Log("up");
    }

    private void Update()
    {
        if (shooting)
        {
            if (canShoot)
            {
                Shoot();
            }
        }

        if(reloadAnimTimer < timeBetweenBullets)
        {
            Vector3 goalPos = reloadAnimGoal.localPosition;
            reloadAnimTarget.localPosition = Vector3.Lerp(goalPos, startPos, reloadAnimTimer/timeBetweenBullets);
            reloadAnimTimer += Time.deltaTime;
        }
    }

    void Shoot()
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
		ray.origin = cam.transform.position;
        //GameObject bulletImpact = Instantiate(bulletImpactPrefab, ray.origin, Quaternion.identity);
        //bulletImpact.name = "origin";

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 500, layerMask, QueryTriggerInteraction.Ignore))
        {
			hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage, player.PMViewID());
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
		}

        audio.PlaySound(audio.playerFireAR[0], 1, 1.5f, 1);

        BulletFlash();

        canShoot = false;
        reloadAnimTimer = 0;

        StartCoroutine(ShootCooldown());
	}

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(timeBetweenBullets);
        canShoot = true;
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

            if (!PV.IsMine)
            {
                player.BulletFlash(player.GetComponent<PlayerController>().GetItemIndex());
                audio.PlaySound(audio.playerFireAR[0], 1, 1.5f, 2);
            }
        }
    }

    public void BulletFlash()
    {
        Vector3 rotation = bulletFlash.transform.localEulerAngles;
        rotation.z += Random.Range(-360, 360);
        bulletFlash.transform.localEulerAngles = rotation;
        bulletFlash.SetActive(true);
        StartCoroutine(TurnOffBulletFlash());
    }

    IEnumerator TurnOffBulletFlash()
    {
        yield return new WaitForSeconds(0.1f);
        bulletFlash.SetActive(false);
    }

}
