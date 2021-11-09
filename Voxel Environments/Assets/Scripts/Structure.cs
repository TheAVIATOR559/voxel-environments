using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structure
{
    public static void MakeOakTree(Vector3 position, Queue<VoxelMod> queue, int minTrunkHeight, int maxTrunkHeight)
    {
        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));
        //Debug.Log(height + "::" + minTrunkHeight);

        if (height < minTrunkHeight)
        {
            //Debug.Log("firing");
            height = minTrunkHeight;
        }

        for (int i = 1; i < height; i++)
        {
            queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + i, (int)position.z), (byte)BlockTypes.Oak_Log));
        }


        for (int x = -3; x < 4; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                for (int z = -3; z < 4; z++)
                {
                    if ((int)position.x + x >= VoxelData.WorldSizeInVoxels || (int)position.z + z >= VoxelData.WorldSizeInVoxels
                        || (int)position.x + x < 0 || (int)position.z + z < 0)
                    {
                        continue;
                    }

                    if(x == 0 && z == 0 && y < 6)
                    {
                        queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + height + y, (int)position.z), (byte)BlockTypes.Oak_Log));
                    }
                    else
                    {
                        //queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x + x, (int)position.y + height + y, (int)position.z + z), (byte)BlockTypes.Oak_Leaves));
                    }
                }
            }
        }
    }

    public static void MakeFirTree(Vector3 position, Queue<VoxelMod> queue, int minTrunkHeight, int maxTrunkHeight, float growthRate)
    {
        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));
        //Debug.Log(seed);

        if (height < minTrunkHeight)
        {
            //Debug.Log("firing");
            height = minTrunkHeight;
        }

        for (int i = 1; i < height; i++)
        {
            queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + i, (int)position.z), (byte)BlockTypes.Fir_Log));
        }

        queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + height, (int)position.z), (byte)BlockTypes.Fir_Leaves));

        float horizGrowth = 1;

        for (int y = height - 1; y > 3; y -= 2)
        {
            if((int)position.x + 1 < VoxelData.WorldSizeInVoxels)
            {
                queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x + 1, (int)position.y + y, (int)position.z), (byte)BlockTypes.Fir_Leaves));
            }

            if ((int)position.x - 1 >= 0)
            {
                queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x - 1, (int)position.y + y, (int)position.z), (byte)BlockTypes.Fir_Leaves));
            }

            if((int)position.z + 1 < VoxelData.WorldSizeInVoxels)
            {
                queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + y, (int)position.z + 1), (byte)BlockTypes.Fir_Leaves));
            }

            if((int)position.z - 1 >= 0)
            {
                queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + y, (int)position.z - 1), (byte)BlockTypes.Fir_Leaves));
            }

            horizGrowth += growthRate;
            int horizGrowthBlocks = Mathf.RoundToInt(horizGrowth);

            for(int x = -horizGrowthBlocks; x <= horizGrowthBlocks; x++)
            {
                for(int z = -horizGrowthBlocks; z <= horizGrowthBlocks; z++)
                {
                    if ((int)position.x + x >= VoxelData.WorldSizeInVoxels || (int)position.z + z >= VoxelData.WorldSizeInVoxels
                        || (int)position.x + x < 0 || (int)position.z + z < 0)//checking the upper bound but not the lower bound
                    {
                        continue;
                    }

                    if((x == 0 && z == 0)
                        || ((x == -horizGrowthBlocks || x == horizGrowthBlocks) && (z == -horizGrowthBlocks || z == horizGrowthBlocks)))
                    {
                        continue;
                    }

                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x + x, (int)position.y + y-1, (int)position.z + z), (byte)BlockTypes.Fir_Leaves));
                }
            }
        }
    }

    public static void MakeCactus(Vector3 position, Queue<VoxelMod> queue,  int minCactusHeight, int maxCactusHeight, float barrelCactusChance, float saguaroCactusChance)
    {
        float result = Random.Range(0, barrelCactusChance + saguaroCactusChance + 0.2f);
        //Debug.Log(result);

        if(result <= barrelCactusChance)
        {
            MakeBarrelCactus(position, queue);
        }
        else if(result > barrelCactusChance && result <= barrelCactusChance + saguaroCactusChance)
        {
            MakeSaguaroCactus(position, queue, minCactusHeight, maxCactusHeight);
        }
        else
        {
            MakeStandardCactus(position, queue, minCactusHeight, maxCactusHeight);
        }
    }

    private static void MakeBarrelCactus(Vector3 position, Queue<VoxelMod> queue)
    {
        queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + 1, (int)position.z), (byte)BlockTypes.Cactus));
    }

    private static void MakeSaguaroCactus(Vector3 position, Queue<VoxelMod> queue, int minCactusHeight, int maxCactusHeight)
    {
        int height = (int)(maxCactusHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));

        if (height < minCactusHeight)
        {
            height = minCactusHeight;
        }

        for (int i = 1; i < height; i++)
        {
            queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + i, (int)position.z), (byte)BlockTypes.Cactus));
        }

        int armCount = Random.Range(1, 5);
        int armStartHeight = (height / 2);
        List<Vector3> armStartPoints = new List<Vector3>();
        int armStartHeightOffset;

        switch (armCount)
        {
            case 4:
                if ((int)position.z + 2 < VoxelData.WorldSizeInVoxels && (int)position.z + 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z + 1), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z + 2));
                }
                goto case 3;
            case 3:
                if ((int)position.x + 2 < VoxelData.WorldSizeInVoxels && (int)position.x + 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x + 1, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x + 2, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z));
                }
                goto case 2;
            case 2:
                if ((int)position.z - 2 < VoxelData.WorldSizeInVoxels && (int)position.z - 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z - 1), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z - 2));
                }
                goto case 1;
            case 1:
                if ((int)position.x - 2 < VoxelData.WorldSizeInVoxels && (int)position.x - 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x - 1, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x - 2, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z));
                }
                break;
        }

        foreach(Vector3 armPoint in armStartPoints)
        {
            int armHeight = Random.Range(1, height - armStartHeight - 1);

            for (int i = 0; i <= armHeight; i++)
            {
                queue.Enqueue(new VoxelMod(new Vector3Int((int)armPoint.x, (int)armPoint.y + i, (int)armPoint.z), (byte)BlockTypes.Cactus));
            }
        }
    }

    private static void MakeStandardCactus(Vector3 position, Queue<VoxelMod> queue, int minCactusHeight, int maxCactusHeight)
    {
        int height = (int)(maxCactusHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));

        if (height < minCactusHeight)
        {
            height = minCactusHeight;
        }

        for (int i = 1; i < height; i++)
        {
            queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + i, (int)position.z), (byte)BlockTypes.Cactus));
        }
    }

    private static void MakeCrazyCactus(Vector3 position, Queue<VoxelMod> queue, int minCactusHeight, int maxCactusHeight)
    {
        int height = (int)(maxCactusHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));

        if (height < minCactusHeight)
        {
            height = minCactusHeight;
        }

        for (int i = 1; i < height; i++)
        {
            queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + i, (int)position.z), (byte)BlockTypes.Cactus));
        }

        int armCount = Random.Range(1, 5);
        int armStartHeight = (height / 2);
        List<Vector3> armStartPoints = new List<Vector3>();
        int armStartHeightOffset;

        switch (armCount)
        {
            case 4://south invalid
                if ((int)position.z + 2 < VoxelData.WorldSizeInVoxels && (int)position.z + 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z + 1), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z + 2));
                }
                goto case 3;
            case 3://west invalid
                if ((int)position.x + 2 < VoxelData.WorldSizeInVoxels && (int)position.x + 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x + 1, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x + 2, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z));
                }
                goto case 2;
            case 2://north invalid
                if ((int)position.z - 2 < VoxelData.WorldSizeInVoxels && (int)position.z - 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z - 1), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z - 2));
                }
                goto case 1;
            case 1://east invalid
                if ((int)position.x - 2 < VoxelData.WorldSizeInVoxels && (int)position.x - 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x - 1, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x - 2, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z));
                }
                break;
        }

        foreach (Vector3 armPoint in armStartPoints)
        {
            int armHeight = Random.Range(1, height - armStartHeight - 1);

            for (int i = 0; i <= armHeight; i++)
            {
                queue.Enqueue(new VoxelMod(new Vector3Int((int)armPoint.x, (int)armPoint.y + i, (int)armPoint.z), (byte)BlockTypes.Cactus));
            }
        }
    }

    private static void MakeNorthInvalidCrazyCactusArms(Vector3 position, Queue<VoxelMod> queue)
    {
        int armCount = Random.Range(0, 4);
        int armStartHeight = Random.Range(1, 4);
        List<Vector3> armStartPoints = new List<Vector3>();
        int armStartHeightOffset;

        switch (armCount)
        {
            case 3:
                if ((int)position.x + 2 < VoxelData.WorldSizeInVoxels && (int)position.x + 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x + 1, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x + 2, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z));
                }
                goto case 2;
            case 2:
                if ((int)position.z - 2 < VoxelData.WorldSizeInVoxels && (int)position.z - 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z - 1), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z - 2));
                }
                goto case 1;
            case 1:
                if ((int)position.x - 2 < VoxelData.WorldSizeInVoxels && (int)position.x - 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x - 1, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x - 2, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z));
                }
                break;
            case 0:
                break;
        }

        foreach (Vector3 armPoint in armStartPoints)
        {
            int armHeight = Random.Range(1, armStartHeight - 1);

            for (int i = 0; i <= armHeight; i++)
            {
                queue.Enqueue(new VoxelMod(new Vector3Int((int)armPoint.x, (int)armPoint.y + i, (int)armPoint.z), (byte)BlockTypes.Cactus));
            }
        }
    }

    private static void MakeEastInvalidCrazyCactusArms(Vector3 position, Queue<VoxelMod> queue)
    {
        int armCount = Random.Range(0, 4);
        int armStartHeight = Random.Range(1, 4);
        List<Vector3> armStartPoints = new List<Vector3>();
        int armStartHeightOffset;

        switch (armCount)
        {
            case 3:
                if ((int)position.z + 2 < VoxelData.WorldSizeInVoxels && (int)position.z + 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z + 1), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z + 2));
                }
                goto case 2;
            case 2:
                if ((int)position.z - 2 < VoxelData.WorldSizeInVoxels && (int)position.z - 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z - 1), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z - 2));
                }
                goto case 1;
            case 1:
                if ((int)position.x - 2 < VoxelData.WorldSizeInVoxels && (int)position.x - 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x - 1, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x - 2, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z));
                }
                break;
            case 0:
                break;
        }

        foreach (Vector3 armPoint in armStartPoints)
        {
            int armHeight = Random.Range(1, armStartHeight - 1);

            for (int i = 0; i <= armHeight; i++)
            {
                queue.Enqueue(new VoxelMod(new Vector3Int((int)armPoint.x, (int)armPoint.y + i, (int)armPoint.z), (byte)BlockTypes.Cactus));
            }
        }
    }

    private static void MakeSoutInvalidCrazyCactusArms(Vector3 position, Queue<VoxelMod> queue)
    {
        int armCount = Random.Range(0, 4);
        int armStartHeight = Random.Range(1, 4);
        List<Vector3> armStartPoints = new List<Vector3>();
        int armStartHeightOffset;

        switch (armCount)
        {
            case 3:
                if ((int)position.x + 2 < VoxelData.WorldSizeInVoxels && (int)position.x + 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x + 1, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x + 2, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z));
                }
                goto case 2;
            case 2:
                if ((int)position.z + 2 < VoxelData.WorldSizeInVoxels && (int)position.z + 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z + 1), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z + 2));
                }
                goto case 1;
            case 1:
                if ((int)position.x - 2 < VoxelData.WorldSizeInVoxels && (int)position.x - 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x - 1, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x - 2, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z));
                }
                break;
            case 0:
                break;
        }

        foreach (Vector3 armPoint in armStartPoints)
        {
            int armHeight = Random.Range(1, armStartHeight - 1);

            for (int i = 0; i <= armHeight; i++)
            {
                queue.Enqueue(new VoxelMod(new Vector3Int((int)armPoint.x, (int)armPoint.y + i, (int)armPoint.z), (byte)BlockTypes.Cactus));
            }
        }
    }

    private static void MakeWestInvalidCrazyCactusArms(Vector3 position, Queue<VoxelMod> queue)
    {
        int armCount = Random.Range(0, 4);
        int armStartHeight = Random.Range(1, 4);
        List<Vector3> armStartPoints = new List<Vector3>();
        int armStartHeightOffset;

        switch (armCount)
        {
            case 3:
                if ((int)position.x + 2 < VoxelData.WorldSizeInVoxels && (int)position.x + 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x + 1, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x + 2, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z));
                }
                goto case 2;
            case 2:
                if ((int)position.z - 2 < VoxelData.WorldSizeInVoxels && (int)position.z - 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z - 1), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z - 2));
                }
                goto case 1;
            case 1:
                if ((int)position.z + 2 < VoxelData.WorldSizeInVoxels && (int)position.z + 2 >= 0)
                {
                    armStartHeightOffset = Random.Range(-1, 2);
                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z + 1), (byte)BlockTypes.Cactus));
                    armStartPoints.Add(new Vector3Int((int)position.x, (int)position.y + armStartHeight + armStartHeightOffset, (int)position.z + 2));
                }
                break;
            case 0:
                break;
        }

        foreach (Vector3 armPoint in armStartPoints)
        {
            int armHeight = Random.Range(1, armStartHeight - 1);

            for (int i = 0; i <= armHeight; i++)
            {
                queue.Enqueue(new VoxelMod(new Vector3Int((int)armPoint.x, (int)armPoint.y + i, (int)armPoint.z), (byte)BlockTypes.Cactus));
            }
        }
    }

    public static void MakeMuskeg(Vector3 position, Queue<VoxelMod> queue)
    {
        queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y, (int)position.z), (byte)BlockTypes.Muskeg));
    }
}
