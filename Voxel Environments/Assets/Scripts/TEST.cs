using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TEST : MonoBehaviour
{
	Texture2D texture;

	VoronoiNoise noise;

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
		int pointCount = 10;
		List<Vector2Int> points = new List<Vector2Int>();

		while(points.Count <= pointCount)
        {
			Vector2Int point = new Vector2Int(Random.Range(0, 101), Random.Range(0, 101));

			if(!points.Contains(point))
            {
				points.Add(point);
            }
        }

		for (int y = 0; y < 100; y++)
		{
			for (int x = 0; x < 100; x++)
			{
				//float value = noise.Sample2D(x, y);
				texture.SetPixel(x, y, Color.black);
			}
		}

		foreach(Vector2Int point in points)
        {
			
			for(int x = -3; x < 4; x++)
            {
				for(int y = -3; y < 4; y++)
                {
					if((x <= 1 && x >= -1) && (y <= 1 && y >= -1))
                    {
						texture.SetPixel(point.x + x, point.y + y, Color.red);
					}
					else
                    {
						texture.SetPixel(point.x + x, point.y + y, Color.blue);
					}
                }
            }

			texture.SetPixel(point.x, point.y, Color.green);
        }			

		texture.Apply();
	}
}
