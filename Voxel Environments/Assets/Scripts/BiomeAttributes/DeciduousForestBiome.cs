using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deciduous Forest", menuName = "Biome Attributes/Deciduous Forest")]
public class DeciduousForestBiome : BiomeAttributes
{
    private int[,] biomassMap;

    [Tooltip("Minumum Tree Biomass")] public int TreeThreshold = 35;
    [Tooltip("Initial Number of Trees")] public int InitialTreeCount = 10;
    [Tooltip("Resource Depletion Radius")] public int TreeDepletionRadius = 10;
    [Tooltip("Resource Depletion Amount")] public int TreeDepletionValue = 25;
    [Tooltip("Resource Canopy Radius")] public int TreeCanopyRadius = 3;
    [Tooltip("Resource Canopy Amount")] public int TreeCanopyValue = 35;
    [Tooltip("Neighbor Radius")] public int TreeNeighborRadius = 1;
    [Tooltip("Neighbor Resource Amount")] public int TreeNeighborValue = 50;

    public override void CreateBiomeHeightMap(int mapWidth, int mapHeight, int seed)
    {
        base.CreateBiomeHeightMap(mapWidth, mapHeight, seed);

        CreateBiomassMap();

        int treeCount = 0;

        List<Vector2Int> initTreeList = new List<Vector2Int>();

        for(int i = 0; i < InitialTreeCount; i++)
        {
            Vector2Int point = new Vector2Int(Random.Range(0, VoxelData.WorldSizeInVoxels), Random.Range(0, VoxelData.WorldSizeInVoxels));

            while (initTreeList.Contains(point))
            {
                point = new Vector2Int(Random.Range(0, VoxelData.WorldSizeInVoxels), Random.Range(0, VoxelData.WorldSizeInVoxels));
            }

            initTreeList.Add(point);

            UpdateBiomassMap(point);
        }

        for (int x = 0; x < VoxelData.WorldSizeInVoxels; x++)
        {
            for (int y = 0; y < VoxelData.WorldSizeInVoxels; y++)
            {
                if(CalcTreePosition(new Vector2Int(x, y)))
                {
                    treeCount++;
                }
            }
        }

        Debug.Log(treeCount);
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
        if (pos.y == heightMap[pos.x, pos.z] && biomassMap[pos.x, pos.z] == -1)
        {
            //Debug.Log("Making Tree");
            Structure.MakeOakTree(pos, m_world.modifications, minTreeHeight, maxTreeHeight);
        }

        return voxelValue;
    }

    private void CreateBiomassMap()
    {
        biomassMap = new int[VoxelData.WorldSizeInVoxels, VoxelData.WorldSizeInVoxels];

        for(int x = 0; x < VoxelData.WorldSizeInVoxels; x++)
        {
            for(int y = 0; y < VoxelData.WorldSizeInVoxels; y++)
            {
                biomassMap[x, y] = Random.Range(0, 100);
            }
        }
    }

    private void UpdateBiomassMap(Vector2Int treePoint)
    {
        if(biomassMap[treePoint.x, treePoint.y] <= 0)
        {
            return;
        }
        //Debug.Log(treePoint);
        int depletionThreshold = TreeDepletionRadius * TreeDepletionRadius;
        int canopyThreshold = TreeCanopyRadius * TreeCanopyRadius;
        int neighborThreshold = TreeNeighborRadius * TreeNeighborRadius;

        for (int x = -TreeDepletionRadius; x < TreeDepletionRadius; x++)
        {
            for (int y = -TreeDepletionRadius; y < TreeDepletionRadius; y++)
            {
                if(treePoint.x + x < 0 || treePoint.x + x >= VoxelData.WorldSizeInVoxels
                    || treePoint.y + y < 0 || treePoint.y + y >= VoxelData.WorldSizeInVoxels
                    || biomassMap[treePoint.x + x, treePoint.y + y] == -1)
                {
                    continue;
                }

                int value = x * x + y * y;

                if (x == 0 && y == 0)
                {
                    biomassMap[treePoint.x + x, treePoint.y + y] = -1;
                }
                else if(value <= neighborThreshold)
                {
                    if (biomassMap[treePoint.x + x, treePoint.y + y] - TreeNeighborValue < 0)
                    {
                        biomassMap[treePoint.x + x, treePoint.y + y] = 0;
                    }
                    else
                    {
                        biomassMap[treePoint.x + x, treePoint.y + y] -= TreeNeighborValue;
                    }
                }
                else if(value >= neighborThreshold && value < canopyThreshold)
                {
                    if (biomassMap[treePoint.x + x, treePoint.y + y] - TreeCanopyValue < 0)
                    {
                        biomassMap[treePoint.x + x, treePoint.y + y] = 0;
                    }
                    else
                    {
                        biomassMap[treePoint.x + x, treePoint.y + y] -= TreeCanopyValue;
                    }
                }
                else if(value >= canopyThreshold && value < depletionThreshold)
                {
                    if (biomassMap[treePoint.x + x, treePoint.y + y] - TreeDepletionValue < 0)
                    {
                        biomassMap[treePoint.x + x, treePoint.y + y] = 0;
                    }
                    else
                    {
                        biomassMap[treePoint.x + x, treePoint.y + y] -= TreeDepletionValue;
                    }
                }
            }
        }
    }

    private bool CalcTreePosition(Vector2Int treePoint)
    {
        if(biomassMap[treePoint.x, treePoint.y] > TreeThreshold)
        {
            UpdateBiomassMap(treePoint);
            return true;
        }

        return false;
    }
}
