using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int seed;
    public BiomeAttributes biome;

    public Material mat;
    public BlockType[] blockTypes;

    private Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

    List<Vector2Int> chunksToCreate = new List<Vector2Int>();

    private void Start()
    {
        Random.InitState(seed);

        GenerateWorld();
    }

    private void GenerateWorld()
    {
        for(int x = 0; x < VoxelData.WorldSizeInChunks; x++)
        {
            for(int z = 0; z < VoxelData.WorldSizeInChunks; z++)
            {
                chunks[x, z] = new Chunk(new Vector2Int(x, z), this, true);
            }
        }
    }

    private Vector2Int GetChunkCoordFromVector3(Vector3 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth), Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth));
    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        return chunks[Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth), Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth)];
    }

    public bool CheckForVoxel(Vector3 pos)
    {
        Vector2Int thisChunk = VoxelData.Vector3ToVector2Int(pos);

        if(!IsVoxelInWorld(pos))
        {
            return false;
        }
        if (chunks[thisChunk.x, thisChunk.y] != null && chunks[thisChunk.x, thisChunk.y].IsVoxelMapPopulated)
        {
            return blockTypes[chunks[thisChunk.x, thisChunk.y].GetVoxelFromGlobalVector3(pos)].isSolid;
        }

        return blockTypes[GetVoxel(pos)].isSolid;
    }

    public byte GetVoxel(Vector3 pos)
    {
        int yPos = Mathf.FloorToInt(pos.y);

        if(!IsVoxelInWorld(pos))
        {
            return 0;//empty block
        }

        if(yPos == 0)
        {
            return 1;//bedrock
        }

        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;
        byte voxelValue = 0;

        if(yPos == terrainHeight)
        {
            voxelValue = 4;//grass
        }
        else if(yPos < terrainHeight && yPos > biome.stoneHeight)//btwn grass and stone
        {
            voxelValue = 3;//dirt
        }
        else if(yPos > terrainHeight)//above ground
        {
            return 0;//air
        }
        else
        {
            return 2;//stone, ores, other underground stuff
        }

        return voxelValue;
    }

    private bool IsChunkInWorld(Vector2Int pos)
    {
        if (pos.x > 0 && pos.x < VoxelData.WorldSizeInChunks - 1
            && pos.y > 0 && pos.y < VoxelData.WorldSizeInChunks - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels
            && pos.y >= 0 && pos.y < VoxelData.ChunkHeight
            && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
