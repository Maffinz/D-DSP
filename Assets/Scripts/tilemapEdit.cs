using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class tilemapEdit : MonoBehaviourPun
{
    public Tilemap[] m_TileMaps;
    //private static int m_TileMapIndex;
    public static Dictionary<Vector3Int, string> tileData = new Dictionary<Vector3Int, string>();

    public Tilemap tilemap; // Default Tilemap
    public List<TileBase> tilePack; // (Testing) Default Tile
    public GameObject tileBox;
    bool EditingMode = false;
    private GameObject tileSlotprefab;
    private Tile SelectedTile;

    public GameObject TileBoxSelector;
    [SerializeField] private static GameObject m_Player;
    private static Camera m_Camera;
    private static GameObject m_canvas;
    private static GameObject m_EditMode;

    // Testing
    public static Vector3Int CellPosition { get; set; }
    public static int TileIndex { get; set; }
    public static int m_TileMapIndex { get; set; }
    public static bool Changed { get; set; }
    public static string Data { get; set; }
    public static bool DataEntered { get; set; }

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
        m_TileMapIndex = 0;
       

        Changed = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("At TileMap");
            if (Input.GetKeyDown(KeyCode.E))
            {
                EditingMode = true;
                Player.isEditing = true;
                m_EditMode.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EditingMode = false;
                Player.isEditing = false;
                m_EditMode.SetActive(false); // Get Edit Mode UI Out of the WAY!!!!!
            }

            // Editing Mode
            if (Player.isEditing && EditingMode && PhotonNetwork.IsMasterClient)
            {
                if (Input.GetKeyDown(KeyCode.Tab)) TileBoxSelector.SetActive(!tileBox.activeInHierarchy);

                if (!tileBox.activeInHierarchy)
                {
                    if (Input.GetMouseButtonDown(0)) // Insert Tile
                    {
                        if (photonView.IsMine)
                        {
                            NetworkMapEdit.OnClickEditTile(GetCellPosition(GetClickPosition()), tilePack.IndexOf(SelectedTile));
                        }
                    }
                    if (Input.GetMouseButtonDown(1)) // Store Data
                    {

                    }
                }
                Debug.Log("Editing Mode");
            }

            
            
        }

        // Update All RPC Stuff
        if (Changed) // Tile Map Tile Edit
        {
            Netowork_ChangeTile();
            Changed = false;
        }
        if (DataEntered) // Tile Map Tile Data
        {
            Network_AddData();
            DataEntered = false;
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
        m_TileMaps[m_TileMapIndex].SetTile(CellPosition, tilePack[TileIndex]);
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

                    // Set Canvas and EditMode Buttons
                    m_canvas = m_Player.transform.GetChild(1).gameObject;
                    m_EditMode = m_canvas.transform.GetChild(1).gameObject;

                    // Set Button Listner
                    m_EditMode.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => On_CollisionButtonClick());

                    // Activate Canvas for Host
                    m_canvas.SetActive(true);
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

    public static void On_CollisionButtonClick()
    {
        // Change Button Text
        if(m_TileMapIndex == 0) // If Its on 
            m_EditMode.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Non-Collision Block";
        else m_EditMode.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Collision Block";

        // Change Index
        //m_TileMapIndex = (m_TileMapIndex + 1) % 2; // 1 Collision : 0 Path (Non Collision)
        NetworkMapEdit.OnClickChangeBlockState();
        Debug.LogWarning("Tile Map Index: " + m_TileMapIndex);
    }

    public void AddData(Vector3Int position, string data)
    {
        if(!tileData.ContainsKey(position))
            tileData.Add(position, data);
        else
            EditData(position, data);
    }

    public void RemoveData(Vector3Int position)
    {
        tileData.Remove(position);
    }

    public void EditData(Vector3Int position, string data)
    {
        tileData.Remove(position);
        tileData.Add(position, data);
    }

    void Network_AddData()
    {
        Debug.LogWarning("Adding Data");
        AddData(CellPosition, Data);
    }
}
