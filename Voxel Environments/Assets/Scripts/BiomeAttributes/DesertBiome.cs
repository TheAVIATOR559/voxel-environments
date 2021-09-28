using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Desert", menuName = "Biome Attributes/Desert")]
public class DesertBiome : BiomeAttributes
{
    [Tooltip("Percent height max of a mesa")] public float mesaChance = 0;
    [Tooltip("Vertical offset height for a mesa")] public int mesaHeight = 0;
    [Tooltip("Number of blocks vertically the mesa bleeds out to")] public int mesaBleedOff = 1;
    [Tooltip("Percent height minimum of a playa")] public float playaChance = 0;
    private int minMesaHeight = 0;
    private int maxPlayaHeight = 0;
    private int minPlayaHeight = 0;

    public override void CreateBiomeHeightMap(int mapWidth, int mapHeight, int seed)
    {
        heightMap = new int[mapWidth, mapHeight];

        int maxHeight = int.MinValue;
        int minHeight = int.MaxValue;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                heightMap[x, z] = Mathf.FloorToInt(terrainWeight * Noise.Get2DPerlin(new Vector2(x, z), seed, terrainScale)) + solidGroundHeight;

                if (heightMap[x, z] > maxHeight)
                {
                    maxHeight = heightMap[x, z];
                }
                if (heightMap[x, z] < minHeight)
                {
                    minHeight = heightMap[x, z];
                }
            }
        }

        //Debug.Log("MAX " + maxHeight + " :: MIN " + minHeight);

        minMesaHeight = maxHeight - Mathf.FloorToInt(maxHeight * mesaChance);
        maxPlayaHeight = minHeight + Mathf.FloorToInt(minHeight * playaChance);
        minPlayaHeight = minHeight;
        //Debug.Log("MESA " + minMesaHeight + " :: PLAYA " + maxPlayaHeight);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                //if above max height - (some amount controlled by mesa chance)
                //add mesa height to index height
                if(heightMap[x,z] >= minMesaHeight)
                {
                    heightMap[x, z] += mesaHeight;
                }
                
                //if below min height + (some amount controlled by saltflat chance)
                //subtract to min flat value
                if(heightMap[x,z] <= maxPlayaHeight)
                {
                    heightMap[x, z] = minHeight;
                }
            }
        }

        minMesaHeight -= mesaBleedOff;
    }

    public override byte CreateBiomeSpecificVoxel(Vector3Int pos, int seed)
    {
        if (!World.IsVoxelInWorld(pos))
        {
            return (byte)BlockTypes.Air;//empty block
        }

        if (pos.y == 0)
        {
            return (byte)BlockTypes.Bedrock;//bedrock
        }

        if (pos.y > heightMap[pos.x, pos.z])//above ground
        {
            return (byte)BlockTypes.Air;//air
        }
        else if(pos.y >= minMesaHeight)
        {
            return (byte)BlockTypes.Mesa;
        }
        else if(pos.y <= maxPlayaHeight && pos.y >= minPlayaHeight && !m_world.CheckForVoxel(new Vector3(pos.x, pos.y + 2, pos.z)))
        {
            return (byte)BlockTypes.Saltflat;
        }
        else if (pos.y == heightMap[pos.x, pos.z])//top layer
        {
            return (byte)BlockTypes.Sand;
        }
        else if (pos.y < heightMap[pos.x, pos.z] && pos.y >= heightMap[pos.x, pos.z] - upperSoilDepth)//upper soil layer
        {
            return (byte)BlockTypes.Sand;
        }
        else if (pos.y < heightMap[pos.x, pos.z] && pos.y >= (heightMap[pos.x, pos.z] - upperSoilDepth) - middleSoilDepth)//mid soil layer
        {
            return (byte)BlockTypes.Sandstone;
        }
        else
        {
            return (byte)BlockTypes.Stone;//stone, ores, other underground stuff
        }
    }
}
