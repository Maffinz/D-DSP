using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomCreateJoin : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField CreateRoomInput;
    [SerializeField] private TMP_InputField JoinRoomInput;
    [SerializeField] private GameObject CreateRoomObject;
    [SerializeField] private GameObject JoinRoomObject;
    [SerializeField] private GameObject StartGameObject;

    [SerializeField] GameObject CreateGame;
    [SerializeField] GameObject JoinGame;
    [SerializeField] GameObject MainMenuInfo;
    [SerializeField] TMP_Text MainMenuInfoText;

    private void Awake()
    {
        ChangeInteractionValue(CreateRoomObject);
        ChangeInteractionValue(JoinRoomObject);
        ChangeInteractionValue(StartGameObject);

        // Set Start Game to not active
        StartGameObject.SetActive(false);
    }

    private void Update()
    {
        // Check If Create Room Input is Nothing and if so Unable to Click button.
        if (CreateRoomInput.text != "")
        {
            CreateRoomObject.GetComponent<Button>().interactable = true;
        } else
        {
            CreateRoomObject.GetComponent<Button>().interactable = false;

        }
        if (JoinRoomInput.text != "")
        {
            JoinRoomObject.GetComponent<Button>().interactable = true;
        } else
        {
            JoinRoomObject.GetComponent<Button>().interactable = false;
        }
    }

    // OnCkickEvents
    public void OnClickCreateRoom()
    {
        PhotonNetwork.CreateRoom(CreateRoomInput.text);
    }

    public void OnClickJoinRoom()
    {
        PhotonNetwork.JoinRoom(JoinRoomInput.text);
    }

    public void OnClickQuitGame()
    {
        Application.Quit();
    }

    public void OnClickStartGame()
    {
        mainMenuManager.StartGame();
    }

    // PUN CALLBACKS
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");

        // Hide Start Game, Join and Create
        JoinGame.SetActive(false);
        CreateGame.SetActive(false);

        if (!PhotonNetwork.IsMasterClient)
        {
            MainMenuInfo.SetActive(true);
            MainMenuInfoText.text = "Waiting for host";
        } 
    }
    public override void OnCreatedRoom()
    {
        StartGameObject.GetComponent<Button>().interactable = true;
        Debug.Log("Created Room");

        // Once Room has been Created there is no Need to Search for a game, Search Lobby
        //JoinRoomObject.SetActive(false);
        // CreateRoomObject.SetActive(false);

        StartGameObject.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Error. Unable to connect to Room" + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Error. Unable to Create Room " + message);

        // Try Again Search or Create
        JoinRoomObject.SetActive(true);
        CreateRoomObject.SetActive(true);
    }


    // Helper Methods
    private void ChangeInteractionValue(GameObject gameobject)
    {
        bool interactable = gameobject.GetComponent<Button>().interactable;
        gameobject.GetComponent<Button>().interactable = !interactable;
    }
}
