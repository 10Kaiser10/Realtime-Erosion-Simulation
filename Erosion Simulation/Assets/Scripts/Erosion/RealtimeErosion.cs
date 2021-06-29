using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealtimeErosion : MonoBehaviour
{
    float[,] map;

    int sizeX;
    int sizeY;
    float rainRate;
    float dt;
    float A;
    float g;
    float l;
    float lx;
    float ly;
    float minSin;
    float Kc;
    float Ks;
    float Kd;
    float Ke;
    ComputeShader NoiseMapShader;
    ComputeShader erosionCompute;
    MeshFilter meshFilter;

    float[,] heightsArr;
    int arrLen;
    ComputeBuffer heightsBuffer;
    ComputeBuffer flowBuffer;
    ComputeBuffer flowTherBuffer;
    ComputeBuffer speedsBuffer;
    float[,] flowArr;
    float[,] speedsArr;

    TerrainTextureGen textureGen;
    TerrainGenerator terraGen;

    void Start()
    {
        terraGen = gameObject.GetComponent<TerrainGenerator>();
        textureGen = gameObject.GetComponent<TerrainTextureGen>();
        
        map = terraGen.map;
        if(map == null) { Debug.Log(1); }
        sizeX = terraGen.sizeX;
        sizeY = terraGen.sizeY;
        rainRate = terraGen.rainRate;
        dt = terraGen.timeStep;
        A = terraGen.pipeArea;
        g = terraGen.gravity;
        l = terraGen.pipeLength;
        lx = terraGen.cellSizeX;
        ly = terraGen.cellSizeY;
        minSin = terraGen.minSin;
        Kc = terraGen.Kc;
        Ks = terraGen.Ks;
        Kd = terraGen.Kd;
        Ke = terraGen.Ke;
        NoiseMapShader = terraGen.NoiseMapShader;
        erosionCompute = terraGen.erosionShader;
        meshFilter = terraGen.meshFilter;

        initializeShaders();
    }

    void FixedUpdate()
    {
        //rainRate = Mathf.Max(0,terraGen.rainRate);
        for (int j = 0; j < 50; j++)
        {
            //erosionCompute.SetFloat("rainRate", rainRate);
            erosionCompute.Dispatch(0, arrLen / 32, 1, 1);
            erosionCompute.Dispatch(1, arrLen / 32, 1, 1);
            erosionCompute.Dispatch(2, arrLen / 32, 1, 1);
            erosionCompute.Dispatch(3, arrLen / 32, 1, 1);
            erosionCompute.Dispatch(4, arrLen / 32, 1, 1);
            erosionCompute.Dispatch(5, arrLen / 32, 1, 1);
            erosionCompute.Dispatch(6, arrLen / 32, 1, 1);
            erosionCompute.Dispatch(7, arrLen / 32, 1, 1);
        }

        heightsBuffer.GetData(heightsArr);

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                map[i, j] = heightsArr[i * sizeY + j, 0];
            }
        }

        float[,] waterMap = new float[sizeX, sizeY];
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                waterMap[i, j] = heightsArr[i * sizeY + j, 1];
            }
        }

        float[,] sedimentMap = new float[sizeX, sizeY];
        /*for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                sedimentMap[i, j] = heightsArr[i * sizeY + j, 2];
            }
        }*/

        Mesh terrainMesh = MeshGeneratorWater.GenerateMesh(map, waterMap);
        meshFilter.sharedMesh = terrainMesh;

        textureGen.GenerateTexture(map, waterMap, sedimentMap);
    }

    void initializeShaders()
    {
        sizeX = map.GetLength(0);
        sizeY = map.GetLength(1);
        arrLen = sizeX * sizeY;

        heightsArr = new float[arrLen, 3];
        flowArr = new float[arrLen, 4];
        speedsArr = new float[arrLen, 2];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                heightsArr[i * sizeY + j, 0] = map[i, j];
            }
        }

        heightsBuffer = new ComputeBuffer(arrLen, 3 * sizeof(float));
        flowBuffer = new ComputeBuffer(arrLen, 4 * sizeof(float));
        flowTherBuffer = new ComputeBuffer(arrLen, 4 * sizeof(float));
        speedsBuffer = new ComputeBuffer(arrLen, 2 * sizeof(float));

        heightsBuffer.SetData(heightsArr);
        flowBuffer.SetData(flowArr);
        flowTherBuffer.SetData(flowArr);
        speedsBuffer.SetData(speedsArr);

        for (int i = 0; i < 8; i++)
        {
            erosionCompute.SetBuffer(i, "heights", heightsBuffer);
            erosionCompute.SetBuffer(i, "flow", flowBuffer);
            erosionCompute.SetBuffer(i, "flowTher", flowTherBuffer);
            erosionCompute.SetBuffer(i, "speeds", speedsBuffer);
        }

        erosionCompute.SetInt("sizeX", sizeX);
        erosionCompute.SetInt("sizeY", sizeY);
        erosionCompute.SetFloat("rainRate", rainRate);
        erosionCompute.SetFloat("dt", dt);
        erosionCompute.SetFloat("A", A);
        erosionCompute.SetFloat("g", g);
        erosionCompute.SetFloat("l", l);
        erosionCompute.SetFloat("lx", lx);
        erosionCompute.SetFloat("ly", ly);
        erosionCompute.SetFloat("minSin", minSin);
        erosionCompute.SetFloat("Kc", Kc);
        erosionCompute.SetFloat("Ks", Ks);
        erosionCompute.SetFloat("Kd", Kd);
        erosionCompute.SetFloat("Ke", Ke);
    }

    private void OnDestroy()
    {
        heightsBuffer.Release();
        flowBuffer.Release();
        flowTherBuffer.Release();
        speedsBuffer.Release();
    }
}
