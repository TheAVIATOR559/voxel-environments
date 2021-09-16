using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tiaga", menuName = "Biome Attributes/Taiga")]
public class TiagaBiome : BiomeAttributes
{
    public override byte CreateBiomeSpecificVoxel(Vector3 pos, int seed)
    {
        return base.CreateBiomeSpecificVoxel(pos, seed);
    }
}
