using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject SpawnButton;

    private Vector2 location;

    public void SpawnPlayer()
    {
        float randomValue = Random.Range(-1f, 1f);
        location = new Vector2(this.transform.position.x * randomValue, this.transform.position.y * randomValue);
        PhotonNetwork.Instantiate(PlayerPrefab.name, location, Quaternion.identity, 0);
    }

    public void OnClick_Spawn()
    {
        SpawnPlayer();
        SpawnButton.SetActive(false);
        Debug.Log("Spawn Player");
    }
}
