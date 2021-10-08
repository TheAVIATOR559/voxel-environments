using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Biome Attributes/Default")]
public class BiomeAttributes : ScriptableObject
{
    [Tooltip("Biome Name")] public string biomeName;
    [Tooltip("Minimum ground height")] public int solidGroundHeight = 0;
    [Tooltip("Weight of the terrain")] public int terrainWeight = 0;
    [Tooltip("Depth of upper soil layer")] public int upperSoilDepth = 0;
    [Tooltip("Depth of midd soil layer")] public int middleSoilDepth = 0;
    [Tooltip("How agressively noise is applied")] public float terrainScale = 0;

    [Header("Trees")]
    public float treeZoneScale = 1.3f;
    [Range(0f, 1f)] public float treeZoneThreshold = 0.6f;
    public float treePlacementScale = 15f;
    [Range(0f, 1f)] public float treePlacementThreshold = 0.8f;

    public int maxTreeHeight = 12;
    public int minTreeHeight = 5;

    protected int[,] heightMap;
    protected World m_world;

    public virtual void CreateBiomeHeightMap(int mapWidth, int mapHeight, int seed)
    {
        heightMap = new int[mapWidth, mapHeight];

        for(int x = 0; x < mapWidth; x++)
        {
            for(int z = 0; z < mapHeight; z++)
            {
                heightMap[x,z] = Mathf.FloorToInt(terrainWeight * Noise.Get2DPerlin(new Vector2(x, z), seed, terrainScale)) + solidGroundHeight;
            }
        }
    }

    public virtual byte CreateBiomeSpecificVoxel(Vector3Int pos, int seed)
    {
        byte voxelValue = 0;

        if (!World.IsVoxelInWorld(pos))
        {
            voxelValue = (byte)BlockTypes.Air;//empty block
        }
        else if (pos.y == 0)
        {
            voxelValue = (byte)BlockTypes.Bedrock;//bedrock
        }
        else if (pos.y > heightMap[pos.x, pos.z])//above ground
        {
            voxelValue = (byte)BlockTypes.Air;//air
        }
        else if (pos.y == heightMap[pos.x, pos.z])//top layer
        {
            voxelValue = (byte)BlockTypes.Grass;
        }
        else if (pos.y < heightMap[pos.x, pos.z] && pos.y >= heightMap[pos.x, pos.z] - upperSoilDepth)//upper soil layer
        {
            voxelValue = (byte)BlockTypes.Dirt;
        }
        else if (pos.y < heightMap[pos.x, pos.z] && pos.y > (heightMap[pos.x, pos.z] - upperSoilDepth) - middleSoilDepth)//mid soil layer
        {
            voxelValue = (byte)BlockTypes.Dirt;
        }
        else
        {
            voxelValue = (byte)BlockTypes.Stone;//stone, ores, other underground stuff
        }

        //tree pass
        if(pos.y == heightMap[pos.x, pos.z])
        {
            if(Noise.Get2DPerlin(new Vector2(pos.x, pos.z), -seed, treeZoneScale) > treeZoneThreshold)
            {
                if(Noise.Get2DPerlin(new Vector2(pos.x, pos.z), -seed, treePlacementScale) > treePlacementThreshold)
                {
                    Structure.MakeOakTree(pos, m_world.modifications, minTreeHeight, maxTreeHeight);
                }
            }
        }

        return voxelValue;
    }

    public void SetUpReferences(World world)
    {
        m_world = world;
    }
}
