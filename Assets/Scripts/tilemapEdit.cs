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
    [SerializeField] private static GameObject m_Player;
    private static Camera m_Camera;

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
        Debug.Log("At TileMap");
        if (Input.GetKeyDown(KeyCode.E)) {
            EditingMode = true;
            Player.isEditing = true;
        } 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EditingMode = false;
            Player.isEditing = false;
        }

        // Editing Mode
        if(EditingMode && PhotonNetwork.IsMasterClient)
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
                    Debug.Log("Clicked on Tile: Editing");
                }
            }
            Debug.Log("Editing Mode");
        }

        if(Changed)
        {
            Netowork_ChangeTile();
            Changed = false;
        }
    }

    Vector3 GetClickPosition()
    {
        return m_Camera.ScreenToWorldPoint(Input.mousePosition);
    }

    Vector3Int GetCellPosition(Vector3 Click_Position)
    {
        Debug.Log("Cell Position: " + tilemap.WorldToCell(Click_Position));
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

    public static void SetMasterClientCamera()
    {
        // If Master Client Player Has already been set return
        if (m_Player != null) return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                if(player.GetPhotonView().IsMine)
                {
                    m_Player = player;
                    m_Camera = m_Player.transform.GetChild(0).GetComponent<Camera>();
                    return;
                }
            }
        }

        // Get Master Client Object
        //m_Player_ = PhotonNetwork.MasterClient;
        //m_Camera = m_Player_.transform.GetChild(0).GetComponent<Camera>();
        

        Debug.LogWarning("Spawned Player...");
        Debug.LogWarning("Camera: " + m_Camera);
    }

}
