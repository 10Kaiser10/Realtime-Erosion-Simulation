using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTextureGen : MonoBehaviour
{
    public Material terrainMat;
    public Color ground;
    public Color water;

    int sizeX, sizeY;

    public void GenerateTexture(float[,] map, float[,] waterMap, float[,] sediment)
    {
        sizeX = map.GetLength(0);
        sizeY = map.GetLength(1);

        Color[,] colorMap = new Color[sizeX, sizeY];

        for(int i=0; i<sizeX; i++)
        {
            for (int j=0; j < sizeY; j++)
            {
                if(waterMap[i,j] < 0.1)
                {
                    colorMap[i, j] = ground;
                }
                else if(waterMap[i,j] > 10)
                {
                    colorMap[i, j] = water;
                }
                else
                {
                    //colorMap[i, j] = Color.Lerp(ground, water, waterMap[i, j] / 10);
                    colorMap[i, j] = ground + (water-ground) * (1 - Mathf.Exp(-waterMap[i, j]));
                    //colorMap[i, j] = sediment[i, j] >= 0 ? Color.red: Color.green;
                }
            }
        }

        Texture2D texture = new Texture2D(sizeX, sizeY);

        for (int i=0; i<sizeX; i++)
        {
            for(int j=0; j<sizeY; j++)
            {
                texture.SetPixel(i, j, colorMap[i, j]);                
            }
        }

        texture.Apply();

        terrainMat.SetTexture("_MainTex", texture);
    }
}
