
using Meshadieme;
using Meshadieme.Math;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Perlin : Noise
{

    [Range(5f, 20f)]
    public float frequency = 10.0f;

    [Range(1, 8)]
    public int octaves = 8;

    [Range(1f, 4f)]
    public float lacunarity = 4f;

    [Range(0f, 1f)]
    public float persistence = 0.7f;

    [Range(2, 3)]
    public int dimensions = 3;

    public int[] hash;


    public Perlin()
    {
        pTypes = new NoiseMethod[] {
            Perlin2D,
            Perlin3D
        };
        hash = new int[0];
    }
    
    private const int hashMask = 255;

    private Vector2[] gradients2D = {
        new Vector2( 1f, 0f),
        new Vector2(-1f, 0f),
        new Vector2( 0f, 1f),
        new Vector2( 0f,-1f),
        new Vector2( 1f, 1f).normalized,
        new Vector2(-1f, 1f).normalized,
        new Vector2( 1f,-1f).normalized,
        new Vector2(-1f,-1f).normalized
    };

    private const int gradientsMask2D = 7;

    private Vector3[] gradients3D = {
        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 1f,-1f, 0f),
        new Vector3(-1f,-1f, 0f),
        new Vector3( 1f, 0f, 1f),
        new Vector3(-1f, 0f, 1f),
        new Vector3( 1f, 0f,-1f),
        new Vector3(-1f, 0f,-1f),
        new Vector3( 0f, 1f, 1f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f, 1f,-1f),
        new Vector3( 0f,-1f,-1f),

        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f,-1f,-1f)
    };

    private const int gradientsMask3D = 15;

    public override void FillColor()
    {
        if (hash.Length != 512) resetHash();
        Debug.Log("Hash Length = " + hash.Length);
        if (skin.width != res)
        {
            skin.Resize(res, res);
        }

        Vector3 vecaa = obj.TransformPoint(new Vector3(-0.5f, -0.5f));
        Vector3 vecba = obj.TransformPoint(new Vector3(0.5f, -0.5f));
        Vector3 vecab = obj.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 vecbb = obj.TransformPoint(new Vector3(0.5f, 0.5f));

        NoiseMethod method = pTypes[dimensions - 2];
        float step = 1.0f / res;
        for (int y = 0; y < res; y++)
        {
            Vector3 veca = Vector3.Lerp(vecaa, vecab, (y + 0.5f) * step);
            Vector3 vecb = Vector3.Lerp(vecba, vecbb, (y + 0.5f) * step);
            for (int x = 0; x < res; x++)
            {
                Vector3 vec = Vector3.Lerp(veca, vecb, (x + 0.5f) * step);
                float sample = Sum(method, vec, frequency, octaves, lacunarity, persistence, hash);
                skin.SetPixel(x, y, color.Evaluate(sample));
            }
        }
        skin.Apply();
    }

    public float Perlin2D(Vector3 point, float frequency, int[] hash)
    {
        point *= frequency;
        int ix0 = Mathf.FloorToInt(point.x);
        int iy0 = Mathf.FloorToInt(point.y);
        float tx0 = point.x - ix0;
        float ty0 = point.y - iy0;
        float tx1 = tx0 - 1f;
        float ty1 = ty0 - 1f;
        ix0 &= hashMask;
        iy0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;

        int h0 = hash[ix0];
        int h1 = hash[ix1];
        Vector2 g00 = gradients2D[hash[h0 + iy0] & gradientsMask2D];
        Vector2 g10 = gradients2D[hash[h1 + iy0] & gradientsMask2D];
        Vector2 g01 = gradients2D[hash[h0 + iy1] & gradientsMask2D];
        Vector2 g11 = gradients2D[hash[h1 + iy1] & gradientsMask2D];

        float v00 = Math.Dot(g00, tx0, ty0);
        float v10 = Math.Dot(g10, tx1, ty0);
        float v01 = Math.Dot(g01, tx0, ty1);
        float v11 = Math.Dot(g11, tx1, ty1);

        float tx = Math.Smooth(tx0);
        float ty = Math.Smooth(ty0);
        return Mathf.Lerp(
            Mathf.Lerp(v00, v10, tx),
            Mathf.Lerp(v01, v11, tx),
            ty) * Math.sqr2;
    }

    public float Perlin3D(Vector3 point, float frequency, int[] hash)
    {
        point *= frequency;
        int ix0 = Mathf.FloorToInt(point.x);
        int iy0 = Mathf.FloorToInt(point.y);
        int iz0 = Mathf.FloorToInt(point.z);
        float tx0 = point.x - ix0;
        float ty0 = point.y - iy0;
        float tz0 = point.z - iz0;
        float tx1 = tx0 - 1f;
        float ty1 = ty0 - 1f;
        float tz1 = tz0 - 1f;
        ix0 &= hashMask;
        iy0 &= hashMask;
        iz0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;
        int iz1 = iz0 + 1;

        int h0 = hash[ix0];
        int h1 = hash[ix1];
        int h00 = hash[h0 + iy0];
        int h10 = hash[h1 + iy0];
        int h01 = hash[h0 + iy1];
        int h11 = hash[h1 + iy1];
        Vector3 g000 = gradients3D[hash[h00 + iz0] & gradientsMask3D];
        Vector3 g100 = gradients3D[hash[h10 + iz0] & gradientsMask3D];
        Vector3 g010 = gradients3D[hash[h01 + iz0] & gradientsMask3D];
        Vector3 g110 = gradients3D[hash[h11 + iz0] & gradientsMask3D];
        Vector3 g001 = gradients3D[hash[h00 + iz1] & gradientsMask3D];
        Vector3 g101 = gradients3D[hash[h10 + iz1] & gradientsMask3D];
        Vector3 g011 = gradients3D[hash[h01 + iz1] & gradientsMask3D];
        Vector3 g111 = gradients3D[hash[h11 + iz1] & gradientsMask3D];

        float v000 = Math.Dot(g000, tx0, ty0, tz0);
        float v100 = Math.Dot(g100, tx1, ty0, tz0);
        float v010 = Math.Dot(g010, tx0, ty1, tz0);
        float v110 = Math.Dot(g110, tx1, ty1, tz0);
        float v001 = Math.Dot(g001, tx0, ty0, tz1);
        float v101 = Math.Dot(g101, tx1, ty0, tz1);
        float v011 = Math.Dot(g011, tx0, ty1, tz1);
        float v111 = Math.Dot(g111, tx1, ty1, tz1);

        float tx = Math.Smooth(tx0);
        float ty = Math.Smooth(ty0);
        float tz = Math.Smooth(tz0);
        return Mathf.Lerp(
            Mathf.Lerp(Mathf.Lerp(v000, v100, tx), Mathf.Lerp(v010, v110, tx), ty),
            Mathf.Lerp(Mathf.Lerp(v001, v101, tx), Mathf.Lerp(v011, v111, tx), ty),
            tz);
    }

    public float Sum(NoiseMethod method, Vector3 point, float frequency, int octaves, float lacunarity, float persistence, int[] hash)
    {
        float sum = method(point, frequency, hash);
        float amplitude = 1f;
        float range = 1f;
        for (int o = 1; o < octaves; o++)
        {
            frequency *= lacunarity;
            amplitude *= persistence;
            range += amplitude;
            sum += method(point, frequency, hash) * amplitude;
        }
        return sum / range;
    }
    public void resetHash()
    {
        Debug.Log("new hash");
        List<int> newHash = new List<int>();
        int[] bag = new int[256];
        for (int i = 0; i < 256; i++)
        {
            bag[i] = i;
        }
        bag = Math.FisherYatesShuffle(new List<int>(bag)).ToArray();
        newHash.InsertRange(0, bag);
        newHash.InsertRange(0, bag);
        hash = newHash.ToArray();
    }
}