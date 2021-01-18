using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class mainMenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text GameVersionText;
    [SerializeField] private GameObject _MainMenu;


    private void Start()
    {
        GameVersionText.text = networkManager.GameVersion; // Set GameVersion Text to Game Version

        Debug.Log(networkManager.GameVersion);
    }

    public override void OnConnectedToMaster()
    {
        _MainMenu.SetActive(true);
    }

    public static void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
        Debug.Log("Changing Level");
    }
}
