using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chunk 
{
    public Vector2Int coord;

    GameObject chunkObj;
    MeshRenderer meshRend;
    MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    public byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    public Queue<VoxelMod> mods = new Queue<VoxelMod>();

    World m_world;

    public bool IsVoxelMapPopulated = false;
    private bool m_IsActive;
    public bool IsActive
    {
        get
        {
            return m_IsActive;
        }

        set
        {
            m_IsActive = value;
            if(chunkObj != null)
            {
                chunkObj.SetActive(value);
            }
        }
    }

    public Vector3 Position
    {
        get
        {
            return chunkObj.transform.position;
        }
    }

    public Chunk(Vector2Int position, World world, bool generateOnLoad)
    {
        coord = position;
        m_world = world;
        IsActive = true;

        if(generateOnLoad)
        {
            Init();
        }
    }

    ~Chunk()
    {
        ClearMeshData();
        //Debug.Log("destroying chunk");
    }

    public void Init()
    {
        chunkObj = new GameObject();
        meshFilter = chunkObj.AddComponent<MeshFilter>();
        meshRend = chunkObj.AddComponent<MeshRenderer>();

        meshRend.material = m_world.mat;

        chunkObj.transform.SetParent(m_world.transform);
        chunkObj.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0, coord.y * VoxelData.ChunkWidth);
        chunkObj.name = "Chunk (" + coord.x + ", " + coord.y + ")";

        PopulateVoxelChunk();
        UpdateChunk();    
    }

    private void PopulateVoxelChunk()
    {
        for(int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for(int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for(int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    voxelMap[x, y, z] = m_world.CreateVoxel(new Vector3(x, y, z) + Position);
                }
            }
        }

        IsVoxelMapPopulated = true;
    }

    public void UpdateChunk()
    {
        while(mods.Count > 0)
        {
            VoxelMod voxMod = mods.Dequeue();
            Vector3 pos = voxMod.position -= Position;
            voxelMap[(int)pos.x, (int)pos.y, (int)pos.z] = voxMod.id;
        }

        ClearMeshData();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if(m_world.blockTypes[voxelMap[x,y,z]].isSolid)
                    {
                        UpdateMeshData(new Vector3(x, y, z));
                    }
                }
            }
        }

        CreateMesh();
    }

    private void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    private void UpdateMeshData(Vector3 pos)
    {
        for(int i = 0; i < 6; i++)
        {
            if(!CheckVoxel(pos + VoxelData.faceChecks[i]))
            {
                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[i, 0]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[i, 1]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[i, 2]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[i, 3]]);

                AddTexture(m_world.blockTypes[blockID].GetTextureID(i));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4;
            }
        }
    }

    private void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private bool IsVoxelInChunk(int x, int y, int z)
    {
        if(x < 0 || x > VoxelData.ChunkWidth - 1
            || y < 0 || y > VoxelData.ChunkWidth - 1
            || z < 0 || z > VoxelData.ChunkWidth - 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void EditVoxel(Vector3 pos, byte newID)
    {
        int xCheck = Mathf.FloorToInt(pos.x) - Mathf.FloorToInt(chunkObj.transform.position.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z) - Mathf.FloorToInt(chunkObj.transform.position.z);

        voxelMap[xCheck, yCheck, zCheck] = newID;

        UpdateSurroundingVoxels(xCheck, yCheck, zCheck);

        UpdateChunk();
    }

    private void UpdateSurroundingVoxels(int x, int y, int z)
    {
        Vector3 thisVoxel = new Vector3(x, y, z);

        for(int p = 0; p < 6; p++)
        {
            Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[p];

            if(!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
            {
                m_world.GetChunkFromVector3(currentVoxel + Position).UpdateChunk();
            }
        }
    }

    private bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if(!IsVoxelInChunk(x,y,z))
        {
            return m_world.CheckForVoxel(pos + Position);
        }

        return m_world.blockTypes[voxelMap[x, y, z]].isSolid;
    }

    private void AddTexture(int textureID)
    {
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
    }

    public byte GetVoxelFromGlobalVector3(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x) - Mathf.FloorToInt(chunkObj.transform.position.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z) - Mathf.FloorToInt(chunkObj.transform.position.z);

        return voxelMap[xCheck, yCheck, zCheck];
    }
}
