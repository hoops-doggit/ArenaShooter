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
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip hitRegSound;
    [SerializeField] GameObject hitRegMarker;
    [SerializeField] AmmoCount ammoCountUI;
    [SerializeField] GameObject reloadingText;

    [Header("Customization")]
    [SerializeField] GameObject bulletFlash;
    [SerializeField] float timeBetweenBullets = 0.1f;
    [SerializeField] Transform reloadAnimTarget;
    [SerializeField] Transform reloadAnimGoal;
    [SerializeField] float reloadMagDuration = 3f;
    private Vector3 startPos;
    private float nextBulletTimer = 0;

    [Header("Recoil Settings")]
    [SerializeField] RecoilController recoilControl;
    [SerializeField] Vector3 recoilAmount = new Vector3(3,3,3);

    PhotonView PV;
    bool canShoot = true;
    bool shooting = false;
    int roundsRemaining;
    bool reloading;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        startPos = reloadAnimTarget.localPosition;

        if (hitRegMarker.activeSelf)
        {
            hitRegMarker.SetActive(false);
        }

        roundsRemaining = ((GunInfo)itemInfo).magSize;
    }

    public override void Use()
	{
        shooting = true;
	}

    public override void StopUsing()
    {
        StopCoroutine(HitRegCooldown());
        HitReg(false);
        shooting = false;
        Debug.Log("up");
    }

    public void UpdateRoundsRemaining()
    {
        ammoCountUI.UpdateText(roundsRemaining, ((GunInfo)itemInfo).magSize);
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

        if(nextBulletTimer < timeBetweenBullets)
        {
            Vector3 goalPos = reloadAnimGoal.localPosition;
            reloadAnimTarget.localPosition = Vector3.Lerp(goalPos, startPos, nextBulletTimer/timeBetweenBullets);
            nextBulletTimer += Time.deltaTime;
        }
    }

    void Shoot()
	{
        roundsRemaining--;



        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
		ray.origin = cam.transform.position;
        //GameObject bulletImpact = Instantiate(bulletImpactPrefab, ray.origin, Quaternion.identity);
        //bulletImpact.name = "origin";

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 500, layerMask, QueryTriggerInteraction.Ignore))
        {
            if(hit.collider.gameObject.GetComponent<IDamageable>() != null)
            {
			    hit.collider.gameObject.GetComponent<IDamageable>().TakeDamage(((GunInfo)itemInfo).damage, player.PMViewID());
                audio.PlaySound(hitRegSound, 1, 1, 3);
                HitReg(true);
            }
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
		}

        audio.PlaySound(shootSound, 1, 1.5f, 0.5f);

        BulletFlash();

        UpdateRoundsRemaining();

        canShoot = false;
        nextBulletTimer = 0;

        if(roundsRemaining > 0)
        {
            StartCoroutine(ShootCooldown());
        }
        else
        {
            StartCoroutine(ReloadMag());
        }
	}

    IEnumerator ReloadMag()
    {

        reloadingText.SetActive(true);
        yield return new WaitForSeconds(reloadMagDuration);
        roundsRemaining = ((GunInfo)itemInfo).magSize;
        UpdateRoundsRemaining();
        reloadingText.SetActive(false);
        canShoot = true;
    }

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(timeBetweenBullets);
        canShoot = true;
    }

    private void HitReg(bool state)
    {
        hitRegMarker.SetActive(state);
        if (state)
        {
            StartCoroutine(HitRegCooldown());
        }
    }

    IEnumerator HitRegCooldown()
    {
        yield return new WaitForSeconds(0.15f);
        hitRegMarker.SetActive(false);
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            GameObject bulletImpact = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpact, 7f);
            bulletImpact.transform.SetParent(colliders[0].transform);

            if (!PV.IsMine)
            {
                player.BulletFlash(player.GetComponent<PlayerController>().GetItemIndex());
                audio.PlaySound(shootSound, 1, 1.5f, 2);
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
