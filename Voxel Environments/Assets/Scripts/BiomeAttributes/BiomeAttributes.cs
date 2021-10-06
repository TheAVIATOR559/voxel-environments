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

    //public Lode[] lodes;

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
        int yPos = Mathf.FloorToInt(pos.y);

        if (!World.IsVoxelInWorld(pos))
        {
            return (byte)BlockTypes.Air;//empty block
        }

        if (yPos == 0)
        {
            return (byte)BlockTypes.Bedrock;//bedrock
        }

        if (yPos > heightMap[pos.x, pos.z])//above ground
        {
            return (byte)BlockTypes.Air;//air
        }
        else if (yPos == heightMap[pos.x, pos.z])//top layer
        {
            return (byte)BlockTypes.Cactus;
        }
        else if (yPos < heightMap[pos.x, pos.z] && yPos >= heightMap[pos.x, pos.z] - upperSoilDepth)//upper soil layer
        {
            return (byte)BlockTypes.Dirt;
        }
        else if (yPos < heightMap[pos.x, pos.z] && yPos > (heightMap[pos.x, pos.z] - upperSoilDepth) - middleSoilDepth)//mid soil layer
        {
            return (byte)BlockTypes.Dirt;
        }
        else
        {
            return (byte)BlockTypes.Stone;//stone, ores, other underground stuff
        }
    }

    public void SetUpReferences(World world)
    {
        m_world = world;
    }
}
