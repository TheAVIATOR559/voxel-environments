using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TEST : MonoBehaviour
{
    Texture2D texture;

    private void Awake()
    {
        //noise = new VoronoiNoise(100, 4, 1);
        texture = new Texture2D(100, 100, TextureFormat.ARGB32, true);
        texture.name = "Procedural Texture";
        GetComponent<MeshRenderer>().material.mainTexture = texture;
        FillTexture();
    }

    private void FillTexture()
    {
        int pointCount = 1;
        List<Vector2Int> points = new List<Vector2Int>();
        int radius = 10;
        int threshold = radius * radius;
        int TreeDepletionRadius = 15;
        int TreeCanopyRadius = 3;
        int TreeNeighborRadius = 1;

        while (points.Count < pointCount)
        {
            Vector2Int point = new Vector2Int(50,50);

            if (!points.Contains(point))
            {
                points.Add(point);
            }
        }

        for (int y = 0; y < 100; y++)
        {
            for (int x = 0; x < 100; x++)
            {
                float value = Random.Range(0, 1f);
                texture.SetPixel(x, y, new Color(value, value, value));
            }
        }

        foreach (Vector2Int point in points)
        {
            int depletionThreshold = TreeDepletionRadius * TreeDepletionRadius;
            int canopyThreshold = TreeCanopyRadius * TreeCanopyRadius;
            int neighborThreshold = TreeNeighborRadius * TreeNeighborRadius;

            for (int x = -TreeDepletionRadius; x < TreeDepletionRadius; x++)
            {
                for (int y = -TreeDepletionRadius; y < TreeDepletionRadius; y++)
                {
                    if (point.x + x < 0 || point.x + x >= VoxelData.WorldSizeInVoxels
                        || point.y + y < 0 || point.y + y >= VoxelData.WorldSizeInVoxels)
                    {
                        continue;
                    }

                    int value = x * x + y * y;

                    if (x == 0 && y == 0)
                    {
                        texture.SetPixel(x, y, Color.red);
                    }
                    else if (value <= neighborThreshold)
                    {
                        texture.SetPixel(x, y, Color.blue);
                    }
                    else if (value >= neighborThreshold && value < canopyThreshold)
                    {
                        texture.SetPixel(x, y, Color.green);
                    }
                    else if (value >= canopyThreshold && value < depletionThreshold)
                    {
                        texture.SetPixel(x, y, Color.yellow);
                    }
                }
            }
        }

        texture.Apply();
    }
}
