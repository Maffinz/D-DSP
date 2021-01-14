using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class tilemapEdit : MonoBehaviour
{
    public Tilemap tilemap; // Default Tilemap
    public List<TileBase> tilePack; // (Testing) Default Tile
    public GameObject tileBox;
    bool EditingMode = false;
    private GameObject tileSlotprefab;
    private Tile SelectedTile;

    void Start()
    {
        tileSlotprefab = Resources.Load("Prefabs/TileSlot") as GameObject;
        FillTileParent();

        // Set Default Selected Tile
        SelectedTile = (Tile)tilePack[0];
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) EditingMode = true;
        if (Input.GetKeyDown(KeyCode.Escape)) EditingMode = false;

        Debug.Log(EditingMode);
        // Editing Mode
        if(EditingMode)
        {
            if (Input.GetKeyDown(KeyCode.Tab)) tileBox.SetActive(!tileBox.activeInHierarchy);

            if (!tileBox.activeInHierarchy)
            {
                if (Input.GetMouseButtonDown(0)) // Insert Tile
                {
                    DrawTile();
                }
            }
            
        }
        
        
    }

    Vector3 GetClickPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void DrawTile()
    {
        Vector3 click_position = GetClickPosition();
        Vector3Int cell_pos = tilemap.WorldToCell(click_position);
        tilemap.SetTile(cell_pos, SelectedTile);
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
}
