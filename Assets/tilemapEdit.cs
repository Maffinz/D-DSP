using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Photon.Pun;

public class tilemapEdit : MonoBehaviourPun
{
    public Tilemap tilemap; // Default Tilemap
    public List<TileBase> tilePack; // (Testing) Default Tile
    public GameObject tileBox;
    bool EditingMode = false;
    private GameObject tileSlotprefab;
    private Tile SelectedTile;

    public GameObject TileBoxSelector;

    // Testing
    public static Vector3Int CellPosition { get; set; }
    public static int TileIndex { get; set; }
    public static bool Changed { get; set; }

    public PhotonView photonView;

    void Start()
    {
        tileSlotprefab = Resources.Load("Prefabs/TileSlot") as GameObject;
        FillTileParent();
        TileBoxSelector.SetActive(false);

        // Set Default Selected Tile
        SelectedTile = (Tile)tilePack[0];

        //PV.RPC("RPC_DrawTile", RpcTarget.All);
        NetworkMapEdit.Tilemap_ = tilemap;

        Changed = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) EditingMode = true;
        if (Input.GetKeyDown(KeyCode.Escape)) EditingMode = false;

        // Editing Mode
        if(EditingMode)
        {
            if (Input.GetKeyDown(KeyCode.Tab)) TileBoxSelector.SetActive(!tileBox.activeInHierarchy);

            if (!tileBox.activeInHierarchy)
            {
                if (Input.GetMouseButtonDown(0)) // Insert Tile
                {
                    if(photonView.IsMine)
                    {
                        NetworkMapEdit.OnClickEditTile(GetCellPosition(GetClickPosition()), tilePack.IndexOf(SelectedTile));
                    }
                    
                  //DrawTile();
                }
            }
        }

        if(Changed)
        {
            Netowork_ChangeTile();
            Changed = false;
        }
    }

    Vector3 GetClickPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    Vector3Int GetCellPosition(Vector3 Click_Position)
    {
        return tilemap.WorldToCell(Click_Position);
    }

    public void onClick_UpdateTile()
    { 
        Vector3 click_position = GetClickPosition();
        CellPosition = tilemap.WorldToCell(click_position);
        tilemap.SetTile(CellPosition, SelectedTile);
    }

    void FillTileParent()
    {
        // This Functions Is Assuming UI Hierarchy looks like
        //       TileSlot
        //          TileButton
        //              TileImage

        foreach(Tile tile in tilePack)
        {
            GameObject newTileSlot = GameObject.Instantiate(tileSlotprefab, transform.position, transform.rotation);
            newTileSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = tile.sprite;
            newTileSlot.transform.GetChild(0).GetChild(0).GetComponent<TileSelect>().tile = tile;

            newTileSlot.transform.SetParent(tileBox.transform);
            newTileSlot.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => GetTileButtonClicked(newTileSlot.transform.GetChild(0).gameObject));
        }
    }

    private void GetTileButtonClicked(GameObject btn = null)
    {
        SelectedTile = btn.transform.GetChild(0).GetComponent<TileSelect>().tile;
    }

    public void Netowork_ChangeTile()
    {
        tilemap.SetTile(CellPosition, tilePack[TileIndex]);
    }

/*    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("TileMapEdit...Stream: " + stream);
        if (stream.IsWriting)
        {
            stream.SendNext(CellPosition);
            stream.SendNext(SelectedTile);
        }
        else if (stream.IsReading)
        {
            Vector3Int cp = (Vector3Int)stream.ReceiveNext();
            Tile st = (Tile)stream.ReceiveNext();

            tilemap.SetTile(cp, st);
        }
    }*/
}
