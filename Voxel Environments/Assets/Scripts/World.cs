using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int Seed;
    public BiomeAttributes biome;

    public Material mat;
    public Material transparentMat;
    public List<BlockType> blockTypes;

    private Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();
    [SerializeField] private List<Chunk> chunksToUpdate = new List<Chunk>();

    private void Awake()
    {
        biome.SetUpReferences(this);

        RegenWorld();
    }

    public void RegenWorld()
    {
        modifications.Clear();
        chunksToUpdate.Clear();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

        blockTypes.Sort((x, y) => x.Type.CompareTo(y.Type));

        Seed = Random.Range(-1000, 1001);

        GenerateWorld();


    }

    private void GenerateWorld()
    {
        biome.CreateBiomeHeightMap(VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth, VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth, Seed);

        for (int x = 0; x < VoxelData.WorldSizeInChunks; x++)
        {
            for (int z = 0; z < VoxelData.WorldSizeInChunks; z++)
            {
                chunks[x, z] = new Chunk(new Vector2Int(x, z), this, true);
            }
        }

        while (modifications.Count > 0)
        {
            VoxelMod voxMod = modifications.Dequeue();

            Vector2Int chunkCoord = GetChunkCoordFromVector3(voxMod.position);
            //Debug.Log(chunkCoord + "::" + voxMod.position);

            //if(!IsChunkInWorld(chunkCoord))
            //{
            //    continue;
            //}

            if (chunks[chunkCoord.x, chunkCoord.y] == null)
            {
                chunks[chunkCoord.x, chunkCoord.y] = new Chunk(chunkCoord, this, true);
            }

            chunks[chunkCoord.x, chunkCoord.y].mods.Enqueue(voxMod);

            if (!chunksToUpdate.Contains(chunks[chunkCoord.x, chunkCoord.y]))
            {
                chunksToUpdate.Add(chunks[chunkCoord.x, chunkCoord.y]);
            }
        }

        foreach (Chunk chunk in chunksToUpdate)
        {
            chunk.UpdateChunk();
        }
        chunksToUpdate.Clear();
    }

    private Vector2Int GetChunkCoordFromVector3(Vector3 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth), Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth));
    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        if (x < 0 || x >= VoxelData.WorldSizeInChunks
            || z < 0 || z >= VoxelData.WorldSizeInChunks)
        {
            return null;
        }

        return chunks[x, z];
    }

    public bool CheckForVoxel(Vector3 pos)
    {
        Vector2Int thisChunk = VoxelData.Vector3ToVector2Int(pos);

        if (!IsVoxelInWorld(pos))
        {
            return false;
        }
        if (chunks[thisChunk.x, thisChunk.y] != null && chunks[thisChunk.x, thisChunk.y].IsVoxelMapPopulated)
        {
            return blockTypes[chunks[thisChunk.x, thisChunk.y].GetVoxelFromGlobalVector3(pos)].isSolid;
        }

        return blockTypes[CreateVoxel(pos)].isSolid;
    }

    public bool CheckForTransparentVoxel(Vector3 pos)
    {
        Vector2Int thisChunk = VoxelData.Vector3ToVector2Int(pos);

        if (!IsVoxelInWorld(pos))
        {
            return true;
        }
        if (chunks[thisChunk.x, thisChunk.y] != null && chunks[thisChunk.x, thisChunk.y].IsVoxelMapPopulated)
        {
            return blockTypes[chunks[thisChunk.x, thisChunk.y].GetVoxelFromGlobalVector3(pos)].renderNeighborFaces;
        }

        return blockTypes[CreateVoxel(pos)].renderNeighborFaces;
    }

    public byte CreateVoxel(Vector3 pos)
    {
        return biome.CreateBiomeSpecificVoxel(Vector3Int.FloorToInt(pos), Seed);
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

    public void AddVoxelMod(VoxelMod mod)
    {
        modifications.Enqueue(mod);
    }

    public void RemoveVoxelMod()
    {
        modifications.Dequeue();
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

    public byte GetVoxel(Vector3 worldPos)
    {
        if (IsVoxelInWorld(worldPos))
        {
            Chunk chunk = GetChunkFromVector3(worldPos);
            if (chunk != null)
            {
                Vector3 localPosition = new Vector3(worldPos.x - (chunk.coord.x * VoxelData.ChunkWidth), worldPos.y, worldPos.z - (chunk.coord.y * VoxelData.ChunkWidth));
                //Debug.Log(worldPos + "::" + chunk.coord + " :: " + localPosition);
                return chunk.voxelMap[(int)localPosition.x, (int)localPosition.y, (int)localPosition.z];
            }
        }

        return (byte)BlockTypes.NULL;
    }
}
