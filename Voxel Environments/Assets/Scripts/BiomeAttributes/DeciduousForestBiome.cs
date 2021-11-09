using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deciduous Forest", menuName = "Biome Attributes/Deciduous Forest")]
public class DeciduousForestBiome : BiomeAttributes
{
    private int[,] biomassMap;
    VoronoiNoise voroNoise;

    [Tooltip("I DONT KNOW")] public float frequency = 0.2f;
    [Tooltip("I DONT KNOW")] public float amplitude = 1f;

    public override void SetUpReferences(World world)
    {
        voroNoise = new VoronoiNoise(world.Seed, frequency, amplitude);

        base.SetUpReferences(world);
    }

    public override void CreateBiomeHeightMap(int mapWidth, int mapHeight, int seed)
    {
        base.CreateBiomeHeightMap(mapWidth, mapHeight, seed);

        CreateBiomassMap();
        CalculateTreePlacement();
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
        if (pos.y == heightMap[pos.x, pos.z])
        {
            if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), -seed, treeZoneScale) > treeZoneThreshold)
            {
                if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), -seed, treePlacementScale) > treePlacementThreshold)
                {
                    Structure.MakeOakTree(pos, m_world.modifications, minTreeHeight, maxTreeHeight);
                }
            }
        }

        return voxelValue;
    }

    private void CreateBiomassMap()
    {
        biomassMap = new int[VoxelData.WorldSizeInVoxels, VoxelData.WorldSizeInVoxels];

        int maxValue = int.MinValue;
        int minValue = int.MaxValue;

        for(int x = 0; x < VoxelData.WorldSizeInVoxels; x++)
        {
            for(int y = 0; y < VoxelData.WorldSizeInVoxels; y++)
            {
                biomassMap[x,y] = (int)(100 * voroNoise.Sample2D(x,y));
                
                if(biomassMap[x,y] > maxValue)
                {
                    maxValue = biomassMap[x, y];
                }
                if(biomassMap[x,y] < minValue)
                {
                    minValue = biomassMap[x, y];
                }

            }
        }

        Debug.Log(minValue + "::" + maxValue);
    }

    private void UpdateBiomassMap()
    {

    }

    private void CalculateTreePlacement()
    {

    }
}
