using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockTypes : byte
{
    NULL,
    STRUCTURE_PLACEHOLDER,
    Air,
    Bedrock,
    Stone,
    Dirt,
    Grass,
    Sand,
    Sandstone,
    Mesa,
    Saltflat,
    Tiaga_Dirt,
    Tiaga_Grass,
    Permafrost,
    Cactus,
    Oak_Log,
    Fir_Log,
    Oak_Leaves,
    Fir_Leaves,
    Cracked_Stone,
    Muskeg
}

[CreateAssetMenu(fileName = "Block", menuName = "Block Type")]
public class BlockType : ScriptableObject
{
    public BlockTypes Type;
    public bool isSolid;
    public bool renderNeighborFaces;
    public bool isChangeable;

    [Header("Texture Values")]
    public int backTexture;
    public int frontTexture;
    public int topTexture;
    public int bottomTexture;
    public int leftTexture;
    public int rightTexture;

    public string GetName()
    {
        return Type.ToString();
    }

    //BACK, FRONT, TOP, BOTTOM, LEFT, RIGHT
    public int GetTextureID(int faceIndex)
    {
        switch(faceIndex)
        {
            case 0:
                return backTexture;
            case 1:
                return frontTexture;
            case 2:
                return topTexture;
            case 3:
                return bottomTexture;
            case 4:
                return leftTexture;
            case 5:
                return rightTexture;
            default:
                Debug.LogError("ERROR IN GetTextureID; INVALID FACE INDEX");
                return 0;
        }
    }
}
