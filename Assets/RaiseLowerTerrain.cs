using UnityEngine;
using System.Collections;

public class RaiseLowerTerrain : MonoBehaviour {
    public Terrain myTerrain;
    public int SmoothArea;
    private int xResolution;
    private int zResolution;
    private float[,] heights;
    private float[,] heightMapBackup;
    protected const float DEPTH_METER_CONVERT = 0.05f;
    public int DeformationTextureNum = 1;
    protected int alphaMapWidth;
    protected int alphaMapHeight;
    protected int numOfAlphaLayers;
    private float[,,] alphaMapBackup;


    void Start() {
        xResolution = myTerrain.terrainData.heightmapWidth;
        zResolution = myTerrain.terrainData.heightmapHeight;
        alphaMapWidth = myTerrain.terrainData.alphamapWidth;
        alphaMapHeight = myTerrain.terrainData.alphamapHeight;
        numOfAlphaLayers = myTerrain.terrainData.alphamapLayers;

        if (Debug.isDebugBuild) {
            heights = myTerrain.terrainData.GetHeights(0, 0, xResolution, zResolution);
            heightMapBackup = myTerrain.terrainData.GetHeights(0, 0, xResolution, zResolution);
            alphaMapBackup = myTerrain.terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);
        }

    }
    void OnApplicationQuit() {
        if (Debug.isDebugBuild) {
            myTerrain.terrainData.SetHeights(0, 0, heightMapBackup);
            myTerrain.terrainData.SetAlphamaps(0, 0, alphaMapBackup);
        }
    }


    void Update() {

        if (Input.GetMouseButton(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                // area middle point x and z, area width, area height, smoothing distance, area height adjust
                raiselowerTerrainArea(hit.point, 10, 10, SmoothArea, 0.001f);
                //TextureDeformation(hit.point, 2000f, 0);

            }
        }
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                // area middle point x and z, area width, area height, smoothing distance, area height adjust
                raiselowerTerrainArea(hit.point, 10, 10, SmoothArea, -0.01f);
                // area middle point x and z, area size, texture ID from terrain textures
                //TextureDeformation(hit.point, 2000f, 0);
            }
        }

    }


    private void raiselowerTerrainArea(Vector3 point, int lenx, int lenz, int smooth, float incdec) {
        smooth += 1;
        float smoothing;
        int terX = (int)((point.x / myTerrain.terrainData.size.x) * xResolution); // do not understand why these two lines are here
        int terZ = (int)((point.z / myTerrain.terrainData.size.z) * zResolution);

        lenx += smooth;
        lenz += smooth;
        terX -= (lenx / 2);
        terZ -= (lenz / 2);

        //stops the pointer from going out of bounds
        if (terX < 0) {
            terX = 0;
        } else if (terX > xResolution) {
            terX = xResolution;
        }

        if (terZ < 0) {
            terZ = 0;
        } else if (terZ > zResolution) {
            terZ = zResolution;
        }

        Debug.Log(smooth);


        float[,] heights = myTerrain.terrainData.GetHeights(terX, terZ, lenx, lenz);

        float y = heights[lenx / 2, lenz / 2];
        y += incdec;
        for (smoothing = 1; smoothing < smooth + 1; smoothing++) {
            float multiplier = smoothing / smooth;
            for (int areax = (int)(smoothing / 2); areax < lenx - (smoothing / 2); areax++) {
                for (int areaz = (int)(smoothing / 2); areaz < lenz - (smoothing / 2); areaz++) {
                    if ((areax > -1) && (areaz > -1) && (areax < xResolution) && (areaz < zResolution)) {
                        heights[areax, areaz] = Mathf.Clamp((float)y * multiplier, 0, 1);
                    }
                }
            }
        }
        myTerrain.terrainData.SetHeights(terX, terZ, heights);
    }
}
