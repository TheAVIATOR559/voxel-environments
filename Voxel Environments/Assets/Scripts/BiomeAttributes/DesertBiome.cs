using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Desert", menuName = "Biome Attributes/Desert")]
public class DesertBiome : BiomeAttributes
{
    [Tooltip("")] public float mesaChance = 0;
    [Tooltip("")] public int mesaCooldownDistance = 0;
    [Tooltip("")] public int mesaHeight = 0;
    [Tooltip("")] public int mesaWidth = 0;

    public override void CreateBiomeHeightMap(int mapWidth, int mapHeight, int seed)
    {
        heightMap = new int[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                heightMap[x, z] = Mathf.FloorToInt(terrainWeight * Noise.Get2DPerlin(new Vector2(x, z), seed, terrainScale)) + solidGroundHeight;
                Debug.Log(heightMap[x, z]);
            }
        }
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
