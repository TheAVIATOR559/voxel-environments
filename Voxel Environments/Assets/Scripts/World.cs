using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int Seed;
    public BiomeAttributes biome;

    public Material mat;
    public List<BlockType> blockTypes;

    private Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

    private void Start()
    {
        blockTypes.Sort((x, y) => x.Type.CompareTo(y.Type));

        GenerateWorld();
    }

    public void RegenWorld()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

        blockTypes.Sort((x, y) => x.Type.CompareTo(y.Type));

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

        return blockTypes[CreateVoxel(pos)].isSolid;
    }

    public byte CreateVoxel(Vector3 pos)
    {
        return biome.CreateBiomeSpecificVoxel(pos, Seed);
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

    public static bool IsVoxelInWorld(Vector3 pos)
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
