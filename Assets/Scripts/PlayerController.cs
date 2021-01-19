using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
	[SerializeField] GameObject cameraHolder;

	[SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

	[SerializeField] Item[] items;

	int itemIndex;
	int previousItemIndex = -1;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	Rigidbody rb;

	PhotonView PV;

	const float maxHealth = 100f;
	float currentHealth = maxHealth;

	PlayerManager playerManager;
    int playerManagerViewID;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();

		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        playerManagerViewID = playerManager.PVViewID;
	}

	void Start()
	{
		if(PV.IsMine)
		{
			EquipItem(0);
		}
		else
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
		}
	}

	void Update()
	{
		if(!PV.IsMine)
			return;

		Look();
		Move();
		Jump();
        Escape();

		for(int i = 0; i < items.Length; i++)
		{
			if(Input.GetKeyDown((i + 1).ToString()))
			{
				EquipItem(i);
				break;
			}
		}

		if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
		{
			if(itemIndex >= items.Length - 1)
			{
				EquipItem(0);
			}
			else
			{
				EquipItem(itemIndex + 1);
			}
		}
		else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
		{
			if(itemIndex <= 0)
			{
				EquipItem(items.Length - 1);
			}
			else
			{
				EquipItem(itemIndex - 1);
			}
		}

		if(Input.GetMouseButtonDown(0))
		{
			items[itemIndex].Use();
		}

        if (Input.GetMouseButtonUp(0))
        {
            items[itemIndex].StopUsing();
        }

        if (transform.position.y < -10f) // Die if you fall out of the world
		{
			Die();
		}
	}

	void Look()
	{
		transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

		verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
	}

	void Move()
	{
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

		moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
	}

	void Jump()
	{
		if(Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			rb.AddForce(transform.up * jumpForce);
		}
	}

    void Escape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RoomManager.Instance.DestorySelf();
            StartCoroutine(LoadMainMenu());
        }
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
        PhotonNetwork.Disconnect();
    }

	void EquipItem(int _index)
	{
		if(_index == previousItemIndex)
			return;

		itemIndex = _index;

		items[itemIndex].itemGameObject.SetActive(true);

		if(previousItemIndex != -1)
		{
			items[previousItemIndex].itemGameObject.SetActive(false);
		}

		previousItemIndex = itemIndex;

		if(PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if(!PV.IsMine && targetPlayer == PV.Owner)
		{
            if(changedProps.ContainsKey("itemIndex"))
            {
			    EquipItem((int)changedProps["itemIndex"]);
            }
            if (changedProps.ContainsKey("firedGun"))
            {
                //BulletFlash((int)changedProps["firedGun"]);
            }
		}
	}

	public void SetGroundedState(bool _grounded)
	{
		grounded = _grounded;
	}

	void FixedUpdate()
	{
		if(!PV.IsMine)
			return;

		rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	public void TakeDamage(float damage, int photonViewID)
	{
		PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, photonViewID);
	}

	[PunRPC]
	void RPC_TakeDamage(float damage, int photonViewID)
	{
		if(!PV.IsMine)
			return;

		currentHealth -= damage;

		if(currentHealth <= 0)
        { 
            Die(photonViewID);
        }
    }

    void Die(int killerViewID)
	{
		playerManager.Die(killerViewID);
	}

    void Die()
    {
        playerManager.Die();
    }

    public int GetItemIndex()
    {
        return itemIndex;
    }

    public int PMViewID()
    {
        return playerManagerViewID;
    }

    public PlayerManager GetPlayerManager()
    {
        return playerManager;
    }

    public PhotonView GetPhotonView()
    {
        return PV;
    }

    public void BulletFlash(int _itemIndex)
    {
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("firedGun", _itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);            
        }

        if ((SingleShotGun)items[_itemIndex])
        {
            SingleShotGun g = (SingleShotGun)items[_itemIndex];
            g.BulletFlash();
        }
        else if ((AutomaticGun)items[_itemIndex])
        {
            AutomaticGun g = (AutomaticGun)items[_itemIndex];
            g.BulletFlash();
        }        
    }

}