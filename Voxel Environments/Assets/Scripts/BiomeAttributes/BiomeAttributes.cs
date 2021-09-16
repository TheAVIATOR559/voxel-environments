using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Biome Attributes/Default")]
public class BiomeAttributes : ScriptableObject
{
    [Tooltip("Biome Name")] public string biomeName;
    [Tooltip("Minimum ground height")] public int solidGroundHeight = 0;
    [Tooltip("Average height of the terrain")] public int terrainHeight = 0;
    [Tooltip("Depth of upper soil layer")] public int upperSoilDepth = 0;
    [Tooltip("Depth of midd soil layer")] public int middleSoilDepth = 0;
    [Tooltip("How agressively noise is applied")] public float terrainScale = 0;

    //public Lode[] lodes;

    public virtual byte CreateBiomeSpecificVoxel(Vector3 pos, int seed)
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

        int terrainHeight = Mathf.FloorToInt(this.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), seed, terrainScale)) + solidGroundHeight;

        if (yPos > terrainHeight)//above ground
        {
            return (byte)BlockTypes.Air;//air
        }
        else if (yPos == terrainHeight)//top layer
        {
            return (byte)BlockTypes.Grass;
        }
        else if (yPos < terrainHeight && yPos >= terrainHeight - upperSoilDepth)//upper soil layer
        {
            return (byte)BlockTypes.Dirt;
        }
        else if (yPos < terrainHeight && yPos > (terrainHeight - upperSoilDepth) - middleSoilDepth)//mid soil layer
        {
            return (byte)BlockTypes.Dirt;
        }
        else
        {
            return (byte)BlockTypes.Stone;//stone, ores, other underground stuff
        }
    }
}
