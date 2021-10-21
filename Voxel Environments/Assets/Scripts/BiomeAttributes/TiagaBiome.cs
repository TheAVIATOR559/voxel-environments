using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tiaga", menuName = "Biome Attributes/Taiga")]
public class TiagaBiome : BiomeAttributes
{
    [Tooltip("# of blocks tree grows out by")] public float firTreeGrowthRate = 0.5f;

    [Header("Tiaga Specific")]
    public float treeLinePercentage = 0.15f;
    public float muskegChance = 0.1f;
    public float riverChance = 0.1f;

    private int treeLineHeight;
    private int maxMuskegHeight;
    private int minMuskegHeight;

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

        treeLineHeight = maxHeight - Mathf.RoundToInt(maxHeight * treeLinePercentage);

        //Debug.Log(treeLineHeight + " :: " + maxHeight + " - " + Mathf.RoundToInt(maxHeight * treeLinePercentage));

        maxMuskegHeight = minHeight + Mathf.RoundToInt(minHeight * muskegChance);
        minMuskegHeight = minHeight;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                if (heightMap[x, z] <= maxMuskegHeight)
                {
                    heightMap[x, z] = maxMuskegHeight;
                }
            }
        }
    }

    public override byte CreateBiomeSpecificVoxel(Vector3Int pos, int seed)
    {
        byte voxelValue = 0;

        if (!World.IsVoxelInWorld(pos))
        {
            voxelValue = (byte)BlockTypes.NULL;//empty block
        }
        else if (pos.y == 0)
        {
            voxelValue = (byte)BlockTypes.Bedrock;//bedrock
        }
        else if (pos.y > heightMap[pos.x, pos.z])//above ground
        {
            voxelValue = (byte)BlockTypes.Air;//air
        }
        else if(pos.y == heightMap[pos.x, pos.z] && pos.y >= treeLineHeight)
        {
            voxelValue = (byte)BlockTypes.Cracked_Stone;
        }
        else if(pos.y <= maxMuskegHeight && pos.y >= minMuskegHeight && !m_world.CheckForVoxel(new Vector3(pos.x, pos.y + 2, pos.z)))
        {
            voxelValue = (byte)BlockTypes.STRUCTURE_PLACEHOLDER;
            Structure.MakeMuskeg(pos, m_world.modifications);
        }
        else if (pos.y == heightMap[pos.x, pos.z] && pos.y < treeLineHeight)//top layer
        {
            voxelValue = (byte)BlockTypes.Tiaga_Grass;
        }
        else if (pos.y < heightMap[pos.x, pos.z] && pos.y >= heightMap[pos.x, pos.z] - upperSoilDepth)//upper soil layer
        {
            voxelValue = (byte)BlockTypes.Tiaga_Dirt;
        }
        else if (pos.y < heightMap[pos.x, pos.z] && pos.y >= (heightMap[pos.x, pos.z] - upperSoilDepth) - middleSoilDepth)//mid soil layer
        {
            voxelValue = (byte)BlockTypes.Permafrost;
        }
        else
        {
            voxelValue = (byte)BlockTypes.Stone;//stone, ores, other underground stuff
        }

        if (pos.y == heightMap[pos.x, pos.z] && pos.y < treeLineHeight && pos.y > maxMuskegHeight)
        {
            if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), -seed, treeZoneScale) > treeZoneThreshold)
            {
                if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), -seed, treePlacementScale) > treePlacementThreshold)
                {
                    Structure.MakeFirTree(pos, m_world.modifications, minTreeHeight, maxTreeHeight, firTreeGrowthRate);
                }
            }
        }

        return voxelValue;
    }
}
