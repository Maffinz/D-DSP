using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelect : MonoBehaviour
{
    private Tile m_Tile;

    public Tile tile
    {
        get { return m_Tile; }
        set { m_Tile = value; }
    }
}
