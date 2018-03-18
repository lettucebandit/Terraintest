using UnityEngine;
using System.Collections;
using System;

public class RaiseLowerTerrain : MonoBehaviour {
    public Terrain myTerrain;
    public static float sigma=1f;
    public static float scale = 0.01f;
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
    private float raiseMagnitude;


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
                //raiselowerTerrainArea(hit.point, 10, 10, SmoothArea, 0.001f);
                raiseGaussian(hit.point, 10);
            }
        }
        if (Input.GetMouseButton(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                // area middle point x and z, area width, area height, smoothing distance, area height adjust
                //raiselowerTerrainArea(hit.point, 10, 10, SmoothArea, -0.001f);
                raiseGaussian(hit.point, 10);
            }
        }

    }


    private void raiseGaussian(Vector3 point, float size) {

        float halfPoint = size / 2;
        int sizeInt = (int)(size);
        int xCorner = (int)(point.x - halfPoint);
        int zCorner = (int)(point.z - halfPoint);


        float[,] baseHeights = myTerrain.terrainData.GetHeights(xCorner, zCorner, sizeInt, sizeInt);
        float[,] gaussianAddition = applyGaussianAdditor(sizeInt);

        for (int i=0;i<sizeInt;i++) {
            for (int j = 0; j < sizeInt; j++) {
                
                baseHeights[i, j] += gaussianAddition[i, j];
            }
        }
        myTerrain.terrainData.SetHeights(xCorner, zCorner, baseHeights);

    }

    private float[,] applyGaussianAdditor(int sizeInt) {
        float[,] additionalHeights = new float[sizeInt,sizeInt];
        Vector2 midpoint = new Vector2();
        midpoint.x = (float)sizeInt/2;
        midpoint.y = (float)sizeInt/2;

        for (int xIterator = 0; xIterator<sizeInt; xIterator++) {
            for (int yIterator = 0; yIterator < sizeInt; yIterator++) {
                Vector2 point = new Vector2();
                point.x = (float)xIterator;
                point.y = (float)yIterator;
                float DistanceToCentre = Vector2.Distance(midpoint, point);
                float raise = gaussianRaise(DistanceToCentre);
                additionalHeights[xIterator, yIterator] = raise;
            }
        }
        

        return additionalHeights;
    }

    private float gaussianRaise(float d) {

       
        float prefix = Mathf.Pow(Mathf.Sqrt(2 * Mathf.PI*Mathf.Pow(sigma,2)), -1);
        float e = 2.71828f;
        float exponent = -Mathf.Pow(d, 2) / (2 * Mathf.Pow(sigma, 2));
        
        return scale*prefix * Mathf.Pow(e,exponent);

    }


    //zombie code


    private void raiselowerTerrainArea(Vector3 point, int lenx, int lenz, int smooth, float incdec) {
        smooth += 1;
        float smoothing;
        int terX = (int)((point.x / myTerrain.terrainData.size.x) * xResolution); // do not understand why these two lines are here
        int terZ = (int)((point.z / myTerrain.terrainData.size.z) * zResolution);

        lenx += smooth; // the upper bound for the raising square
        lenz += smooth;
        terX -= (lenx / 2); // the lower bound for raising square
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
