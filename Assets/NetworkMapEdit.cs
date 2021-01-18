using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Tilemaps;
using TMPro;

public class NetworkMapEdit : MonoBehaviour
{
    public GameObject TM;
    private static PhotonView photonView;

    private static  Vector3Int _Cell_Position;
    private static int _Tile_index;
    private static Tile _Selected_Tile;
    private static Tilemap _tilemap;

    public static Vector3Int Cell_Position_
    {
        set { _Cell_Position = value; }
        get { return _Cell_Position; }
    }
    public static Tile Selected_Tile_
    {
        set { _Selected_Tile = value; }
        get { return _Selected_Tile; }
    }
    public static Tilemap Tilemap_
    {
        set { _tilemap = value; }
        get { return _tilemap; }
    }
    public static int Tile_Index
    {
        get { return _Tile_index; }
        set { _Tile_index = value; }
    }

    [SerializeField] TMP_Text test_text;


    private void Awake()
    {
        photonView = this.GetComponent<PhotonView>();
        Debug.Log("PhotonView: " + photonView);
    }

    public static void OnClickEditTile(Vector3Int CP, int TI)
    {
        Debug.Log("Clicked On Tile");

        foreach (var player in PhotonNetwork.PlayerList)
            Debug.Log("Player in room: " + player.NickName);

        photonView.RPC("RPC_UpdateTileMap", RpcTarget.AllBuffered, CP.x, CP.y, CP.z, TI);
    }

    [PunRPC]
    public void RPC_UpdateTileMap(int x, int y, int z, int index)
    {
        // Testing 
        Debug.LogWarning("RPC Call Executing...");

        // 
        Cell_Position_ = new Vector3Int(x,y,z);
        Tile_Index = index;

        // Debugging 
        Debug.LogError(string.Format("X: {0}, Y: {1}, Z: {2}", x, y, z));
        Debug.LogError("Cell Positoin: " + Cell_Position_);
        Debug.LogError("Tile Index   : " + Tile_Index);

        Edit_Map();
    }

    public void Edit_Map()
    {
        Debug.Log("Editing Map...");
        Debug.LogWarning("Values: " + Cell_Position_ + "Second Value: " + Tile_Index);
        tilemapEdit.CellPosition = Cell_Position_;
        tilemapEdit.TileIndex = Tile_Index;
        tilemapEdit.Changed = true;
    }
}
