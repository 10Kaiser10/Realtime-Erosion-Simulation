using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ErosionSimulation
{
    public static float[,] Erode(float[,] map, int numIters, float rainRate, float dt, float A, float g, float l, float lx, float ly, float minSin, float Kc, float Ks, float Kd, float Ke, ComputeShader erosionCompute)
    {
        int sizeX = map.GetLength(0);
        int sizeY = map.GetLength(1);
        int arrLen = sizeX * sizeY;

        float[,] heightsArr = new float[arrLen, 3];
        float[,] flowArr = new float[arrLen, 4];
        float[,] speedsArr = new float[arrLen, 2];

        for (int i=0; i<sizeX; i++)
        {
            for(int j=0; j<sizeY; j++)
            {
                heightsArr[i * sizeY + j, 0] = map[i, j];
            }
        }

        ComputeBuffer heightsBuffer = new ComputeBuffer(arrLen, 3 * sizeof(float));
        ComputeBuffer flowBuffer = new ComputeBuffer(arrLen, 4 * sizeof(float));
        ComputeBuffer speedsBuffer = new ComputeBuffer(arrLen, 2 * sizeof(float));

        heightsBuffer.SetData(heightsArr);
        flowBuffer.SetData(flowArr);
        speedsBuffer.SetData(speedsArr);

        for(int i=0; i<6; i++)
        {
            erosionCompute.SetBuffer(i, "heights", heightsBuffer);
            erosionCompute.SetBuffer(i, "flow", flowBuffer);
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

        for(int i=0; i<numIters; i++)
        {
            for(int j=0; j<6; j++)
            {
                erosionCompute.Dispatch(j, arrLen / 32, 1, 1);
            }
        }

        flowBuffer.Release();
        speedsBuffer.Release();

        heightsBuffer.GetData(heightsArr);
        heightsBuffer.Release();

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                map[i, j] = heightsArr[i * sizeY + j, 0];
            }
        }

        return map;
    }
}
