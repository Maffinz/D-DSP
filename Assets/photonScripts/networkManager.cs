using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class networkManager : MonoBehaviourPunCallbacks
{
    // UI
    private TMP_Text NetworkInfo;
    [SerializeField] private GameObject NetworkInformation;

    private static string _GameVersion = "Alpha 1.0";

    public static string GameVersion
    {
        get { return _GameVersion; }
        set { _GameVersion = value; }
    }

    private void Start()
    {
        PhotonNetwork.GameVersion = _GameVersion;
        PhotonNetwork.ConnectUsingSettings();

        // Activate NetworkInfo
        NetworkInformation.SetActive(true);

        // Set NetworkInfo Text
        NetworkInfo = NetworkInformation.GetComponentInChildren<TMP_Text>();
        NetworkInfo.text = "Connecting....";
        

        Debug.Log("Connecting To Server...");
    }

    public override void OnConnectedToMaster()
    {
        NetworkInfo.text = "Connected";
        NetworkInformation.SetActive(false);

        Debug.Log("Connected to Server");
    }
}
