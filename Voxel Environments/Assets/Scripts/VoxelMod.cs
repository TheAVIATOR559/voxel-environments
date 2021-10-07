using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMod
{
    public Vector3 position;
    public byte id;

    public VoxelMod()
    {
        position = new Vector3();
        id = 0;
    }

    public VoxelMod(Vector3Int pos, byte id)
    {
        position = pos;
        this.id = id;
    }
}
