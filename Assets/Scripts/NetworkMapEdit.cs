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
    private static string _Data;

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
    public static string Data { get { return _Data; } set { _Data = value; } }

    private void Awake()
    {
        photonView = this.GetComponent<PhotonView>();
        Debug.Log("PhotonView: " + photonView);
    }

    // Adding a Tile (Draw Tile) OnClick Event
    public static void OnClickEditTile(Vector3Int CP, int TI)
    {
        Debug.Log("Clicked On Tile");

        foreach (var player in PhotonNetwork.PlayerList)
            Debug.Log("Player in room: " + player.NickName);

        photonView.RPC("RPC_UpdateTileMap", RpcTarget.AllBuffered, CP.x, CP.y, CP.z, TI);
    }
    
    // Adding Data to Tile (Data Entry) OnClick Event
    public static void OnClickDataEntry(Vector3Int pos, string data)
    {
        photonView.RPC("RPC_UpdateTileData", RpcTarget.AllBuffered, pos.x, pos.y, pos.z, data);
    }

    // Changing TileMap Index
    public static void OnClickChangeBlockState()
    {
        photonView.RPC("RPC_ChangeIndex", RpcTarget.AllBuffered);
    }

    // PUN RPC Calls
    // Update Tile Map Drawing
    [PunRPC]
    public void RPC_UpdateTileMap(int x, int y, int z, int index)
    {
        // Testing 
        Debug.LogWarning("RPC Call Executing...");

        // 
        Cell_Position_ = new Vector3Int(x,y,z);
        Tile_Index = index;
        Edit_Map();
    }
    // Update Data Entry
    [PunRPC]
    public void RPC_UpdateTileData(int x, int y, int z, string data)
    {
        Debug.LogWarning("Adding Data");
        Cell_Position_ = new Vector3Int(x, y, z);
        _Data = data;

        Debug.LogWarning("Cell Position Vector: " + Cell_Position_);
        Debug.LogWarning("Number of Datas in all Tiles: " + tilemapEdit.tileData.Count);

        EditData();
    }

    [PunRPC]
    public void RPC_ChangeIndex()
    {
        tilemapEdit.m_TileMapIndex = (tilemapEdit.m_TileMapIndex + 1) % 2;
    }
    public void EditData()
    {
        Debug.Log("Adding Data To Tile");
        tilemapEdit.CellPosition = Cell_Position_;
        tilemapEdit.Data = _Data;
        tilemapEdit.DataEntered = true;
    }

    public void Edit_Map()
    {
        Debug.Log("Editing Map...");
        tilemapEdit.CellPosition = Cell_Position_;
        tilemapEdit.TileIndex = Tile_Index;
        tilemapEdit.Changed = true;
    }
}
