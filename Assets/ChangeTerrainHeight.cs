using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTerrainHeight : MonoBehaviour {

    public Terrain Terrain;

    void OnGUI()
    {
        if (GUI.Button(new Rect(30, 30, 200, 30), "Change Terrain Height"))
        {
            


            //get the terrain heightmap width and height.
            int xRes = Terrain.terrainData.heightmapWidth;
            int yRes = Terrain.terrainData.heightmapHeight;
            Debug.Log(xRes + "," + yRes);

            //GetHeights - gets the heightmap points of the terrain. Store those values in a float array
            float[,] heights = Terrain.terrainData.GetHeights(0, 0, xRes, yRes);


            //manipulate the height data
            float currentheight = heights[10, 10];
            heights[10, 10] = currentheight + 0.01f;






            //SetHeights to change the terrain height.
            Terrain.terrainData.SetHeights(0, 0, heights);
        }
        


    }

}
