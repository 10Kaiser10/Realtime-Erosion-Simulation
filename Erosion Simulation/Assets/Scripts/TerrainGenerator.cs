using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator: MonoBehaviour
{
    public float[,] map;

    public int sizeX;
    public int sizeY;
    public float scale;
    public float maxHeight;
    public int octaves;
    public float persis;
    public float lac;
    public int seed;
    public Vector2 offset;
    public int numIters;
    public float rainRate;
    public float timeStep;
    public float pipeArea;
    public float gravity;
    public float pipeLength;
    public float cellSizeX;
    public float cellSizeY;
    public float minSin;
    public float Kc;
    public float Ks;
    public float Kd;
    public float Ke;
    public ComputeShader NoiseMapShader;
    public ComputeShader erosionShader;
    public MeshFilter meshFilter;

    private void Start()
    {
        generate();
    }

    public void generate()
    {
        float t = Time.realtimeSinceStartup;

        map = NoiseMapGPU.NoiseMapGenerator(sizeX, sizeY, scale, maxHeight, octaves, persis, lac, seed, offset, NoiseMapShader);
        Debug.Log(Time.realtimeSinceStartup - t); t = Time.realtimeSinceStartup;

        float[,] waterMap = new float[sizeX, sizeY];
        Mesh terrainMesh = MeshGeneratorWater.GenerateMesh(map, waterMap);
        meshFilter.sharedMesh = terrainMesh;

        /*map = ErosionSimulation.Erode(map, numIters, rainRate, timeStep, pipeArea, gravity, pipeLength, cellSizeX, cellSizeY, minSin, Kc, Ks, Kd, Ke, erosionShader);
        Debug.Log(Time.realtimeSinceStartup - t); t = Time.realtimeSinceStartup;

        Mesh terrainMesh = MeshGenerator.GenerateMesh(map);
        meshFilter.sharedMesh = terrainMesh;
        Debug.Log(Time.realtimeSinceStartup - t); t = Time.realtimeSinceStartup;*/
    }
}
