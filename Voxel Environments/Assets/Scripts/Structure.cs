using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structure
{
    public static void MakeTree(Vector3 position, Queue<VoxelMod> queue, int minTrunkHeight, int maxTrunkHeight)
    {
        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));
        //Debug.Log(height + "::" + minTrunkHeight);

        if(height < minTrunkHeight)
        {
            //Debug.Log("firing");
            height = minTrunkHeight;
        }

        for(int i = 1; i < height; i++)
        {
            queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + i, (int)position.z), (byte)BlockTypes.Oak_Log));
        }

        for (int x = -3; x < 4; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                for (int z = -3; z < 4; z++)
                {
                    if((int)position.x + x >= VoxelData.WorldSizeInVoxels || (int)position.z + z >= VoxelData.WorldSizeInVoxels
                        || (int)position.x + x < 0 || (int)position.z + z < 0)//checking the upper bound but not the lower bound
                    {
                        continue;
                    }

                    queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x + x, (int)position.y + height + y, (int)position.z + z), (byte)BlockTypes.Oak_Leaves));
                }
            }
        }
    }

    public static void MakeCactus(Vector3 position, Queue<VoxelMod> queue, int minCactusHeight, int maxCactusHeight)
    {
        int height = (int)(maxCactusHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));
        //Debug.Log(height + "::" + minTrunkHeight);

        if (height < minCactusHeight)
        {
            //Debug.Log("firing");
            height = minCactusHeight;
        }

        for (int i = 1; i < height; i++)
        {
            queue.Enqueue(new VoxelMod(new Vector3Int((int)position.x, (int)position.y + i, (int)position.z), (byte)BlockTypes.Cactus));
        }
    }
}
