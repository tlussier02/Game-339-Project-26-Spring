using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridPopulator : MonoBehaviour
{
    public Vector3Int gridStartCell = new  Vector3Int(0, 5, 0);
    public Tilemap Tilemap;
    public List<TileBase> TilesToPlace;
    
    public void Start()
    {
        // hi mom
        for (int x = 0; x <= 8; x++)
        {
            for (int y = 0; y <= 8; y++)
            {
                Vector3Int cellPosition = gridStartCell + new Vector3Int(x, -y, 0);
                Tilemap.SetTile(cellPosition, TilesToPlace[Random.Range(0, TilesToPlace.Count)]);
            }
        }
    }
}
