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

    public override byte CreateBiomeSpecificVoxel(Vector3 pos, int seed)
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
            return (byte)BlockTypes.Sand;
        }
        else if (yPos < terrainHeight && yPos >= terrainHeight - upperSoilDepth)//upper soil layer
        {
            return (byte)BlockTypes.Sand;
        }
        else if (yPos < terrainHeight && yPos >= (terrainHeight - upperSoilDepth) - middleSoilDepth)//mid soil layer
        {
            return (byte)BlockTypes.Sandstone;
        }
        else
        {
            return (byte)BlockTypes.Stone;//stone, ores, other underground stuff
        }
    }
}
