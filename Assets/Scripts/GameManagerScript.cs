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
    public GameObject CharacterSelection;
    [SerializeField] GameObject SpwanCamera;
    public List<Sprite> Character;
    private int INDEX = 0;

    private Vector2 location;

    public void SpawnPlayer()
    {
        float randomValue = Random.Range(-1f, 1f);
        location = new Vector2(this.transform.position.x * randomValue, this.transform.position.y * randomValue);
        SpwanCamera.SetActive(false);

        PlayerPrefab.GetComponent<SpriteRenderer>().sprite = Character[INDEX];
        PhotonNetwork.Instantiate(PlayerPrefab.name, location, Quaternion.identity, 0);
    }

    public void OnClick_Spawn()
    {
        SpawnPlayer();
        SpawnButton.SetActive(false);
        CharacterSelection.SetActive(false);
        Debug.Log("Spawn Player");
        tilemapEdit.SetMasterClientCamera();
    }

    public void onClick_Player(int index)
    {
        INDEX = index;
    }
}
