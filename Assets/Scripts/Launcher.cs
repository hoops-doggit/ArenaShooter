using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
	public static Launcher Instance;

	[SerializeField] TMP_InputField roomNameInputField;
	[SerializeField] TMP_Text errorText;
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] Transform roomListContent;
	[SerializeField] GameObject roomListItemPrefab;
	[SerializeField] Transform playerListContent;
	[SerializeField] GameObject PlayerListItemPrefab;
	[SerializeField] GameObject startGameButton;
    List<RoomInfo> fullRoomList = new List<RoomInfo>();
    [SerializeField] List<string> names;
    public int arenaToLoad;
    void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		Debug.Log("Connecting to Master");
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master");
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnJoinedLobby()
	{
		MenuManager.Instance.OpenMenu("title");
		Debug.Log("Joined Lobby");
		//PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
		PhotonNetwork.NickName = GetRandomName();
	}

    private string GetRandomName()
    {
        return names[Random.Range(0, names.Count)];
    }

	public void CreateRoom()
	{
		if(string.IsNullOrEmpty(roomNameInputField.text))
		{
			return;
		}
		PhotonNetwork.CreateRoom(roomNameInputField.text);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnJoinedRoom()
	{
		MenuManager.Instance.OpenMenu("room");
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;

		Player[] players = PhotonNetwork.PlayerList;

		foreach(Transform child in playerListContent)
		{
			Destroy(child.gameObject);
		}

		for(int i = 0; i < players.Count(); i++)
		{
			Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
		}

		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "Room Creation Failed: " + message;
		Debug.LogError("Room Creation Failed: " + message);
		MenuManager.Instance.OpenMenu("error");
	}

	public void StartGame()
	{
		PhotonNetwork.LoadLevel(arenaToLoad);
        PhotonNetwork.ConnectUsingSettings();
        fullRoomList = new List<RoomInfo>();
    }

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loading");
	}

	public void JoinRoom(RoomInfo info)
	{
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnLeftRoom()
	{
		MenuManager.Instance.OpenMenu("title");
	}

    public override void OnRoomListUpdate(List<RoomInfo> roomList)//this parameter is not what you think it is :P
    {
        //the room list parameter does not contain every room made, which is why you can't draw it to the lobby list screen
        // So I (SFDarkHZ AKA Hedgineering <--- Hedgineering is my YT Channel, subscribe if you want ;]) created a custom
        // List<RoomInfo> fullRoomList, and dynamically updated that based on the callbacks to this method
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);

        }

        for (int i = 0; i <= roomList.Count - 1; i++)
        {
            //If a room is marked as "removed", we remove it from our fullRoomList
            if (roomList[i].RemovedFromList)
            {
                for (int a = 0; a < fullRoomList.Count; a++)
                {
                    //We have to check name equality because other elements of the RoomInfo could change and cause the 
                    //fullRoomInfo.Contains() to misrepresent its actual posession of the lobby in question
                    if (fullRoomList[a].Name.Equals(roomList[i].Name)) fullRoomList.RemoveAt(a);
                }
            }
            //If the room is NOT marked as "removed", we add it to our fullRoomList
            if (!fullRoomList.Contains(roomList[i])) fullRoomList.Add(roomList[i]);

            //And we also use this to make sure multiple instances of the same room are not placed into fullRoomList (trust me I needed this code block to make this work)
            for (int b = 0; b < fullRoomList.Count; b++)
            {
                if (fullRoomList[b].Name.Equals(roomList[i].Name)) fullRoomList[b] = roomList[i];
            }
        }

        if (!(fullRoomList.Count == 0)) //check to see if we need to draw onto the screen
        {
            for (int i = 0; i < fullRoomList.Count; i++)
            {
                //theoretically we shouldn't get any non-removed rooms in this list, but we do this just in case
                if (fullRoomList[i].RemovedFromList == false) //draw the non-removed rooms onto the screen
                {
                    Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(fullRoomList[i]);
                }
            }
        }
        

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
	}
}