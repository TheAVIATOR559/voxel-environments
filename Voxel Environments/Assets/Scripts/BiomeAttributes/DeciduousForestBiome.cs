using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deciduous Forest", menuName = "Biome Attributes/Deciduous Forest")]
public class DeciduousForestBiome : BiomeAttributes
{
    public override byte CreateBiomeSpecificVoxel(Vector3Int pos, int seed)
    {
        return base.CreateBiomeSpecificVoxel(pos, seed);
    }
}
