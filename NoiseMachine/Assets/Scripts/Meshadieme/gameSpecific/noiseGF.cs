//----------------------------------------
//		Unity3D Games Template (C#)
// Copyright © 2015 Lord Meshadieme
// 	   skype : lord_meshadieme
//----------------------------------------

/// <version>
/// 0.1.0
/// </version>
/// <summary>
/// A Parent Class for most of our scripts,
/// To make it easier to manage debugging code and modify debug output as needed.
/// Aswell as avoiding debug messages is a non-development build.
/// </summary>
/// CHANGELOG: 
///	*	0.1.0: C# Game Template Base
/// TODO: Consolidate Debug Toggles in Editor for easiers access (C#)
/// 

using Meshadieme;
using Meshadieme.Math;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

namespace Meshadieme
{

    //List of Game States
    public enum GameMode
    {
        Main = 0,
    }
    

    //Main Framework class
    public class noiseGF : gameFramework
    {
        //Declaration of Global Variables and other useful objects
        public GameMode gMode;

        //noise vars - perlin
        [Range(2, 2048)]
        public int res;

        public Gradient color;

        public GameObject skinObj;

        public NoiseType type;

        public Noise noise;

        public AnimationCurve hCurve;

        public int lod = 2;

        public int multiplier = 12;

        public float freq = 10f;
        public int octave = 0;
        public float lacunarity = 4f;
        public float persistence = 0.7f;
        public int dimension = 3;

        public MeshFilter mf;

        public MeshRenderer mr;


        //Init Function
        protected override void Awake()
        {
            gMode = GameMode.Main;
            GM.Get().scene.miscRefs[0].SetActive(false);
            GM.Get().scene.buttonRefs[3].GetComponent<InputField>().text = "" + lod;
            GM.Get().scene.buttonRefs[4].GetComponent<InputField>().text = "" + multiplier;
            GM.Get().scene.buttonRefs[5].GetComponent<InputField>().text = "" + res;
            GM.Get().scene.buttonRefs[6].GetComponent<InputField>().text = "" + freq;
            GM.Get().scene.buttonRefs[7].GetComponent<InputField>().text = "" + octave;
            GM.Get().scene.buttonRefs[8].GetComponent<InputField>().text = "" + lacunarity;
            GM.Get().scene.buttonRefs[9].GetComponent<InputField>().text = "" + persistence;
            GM.Get().scene.buttonRefs[10].GetComponent<Slider>().value = (float) dimension;
        }

        public void generateNoise()
        {
            if (noise != null) { noise = null; }
            switch (type)
            {
                case NoiseType.BasicPerlin:
                    noise = new Perlin();
                    Perlin pNoise = noise as Perlin;
                    pNoise.frequency = freq;
                    pNoise.octaves = octave;
                    pNoise.lacunarity = lacunarity;
                    pNoise.persistence = persistence;
                    pNoise.dimensions = dimension;
                    break;
                case NoiseType.DiamondSquare:
                    noise = new DiamondSquare();
                    break;
            }
            noise.obj = this.transform;
            noise.color = color;
            if (noise.skin == null)
            {
                noise.skin = new Texture2D(res, res, TextureFormat.RGB24, true);
                noise.skin.name = "Procedurally Generated Noise";
                noise.skin.wrapMode = TextureWrapMode.Clamp;
                noise.skin.filterMode = FilterMode.Bilinear;
                noise.skin.anisoLevel = 9;
                //skinObj.GetComponent<MeshRenderer>().material.mainTexture = noise.skin;
                skinObj.GetComponent<Image>().material.mainTexture = noise.skin;
                noise.skin.Apply();
            }
            noise.FillColor();
            noise.WriteToFile();
            MeshGenData mgd = generateTerrain();
            Texture2D mt = generateTexture();
            Mesh terrain = mgd.MakeMesh();
            mf.sharedMesh = terrain;
            mr.sharedMaterial.mainTexture = mt;
        }

        public Texture2D generateTexture()
        {
            Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    tex.SetPixel(x, y, noise.skin.GetPixel(x, y));
                }
            }
            tex.Apply();
            return tex;
        }

        public MeshGenData generateTerrain()
        {

            float startX = (res - 1) / -2.0f;
            float startZ = (res - 1) / 2.0f;
            

            MeshGenData md = new MeshGenData(res * lod, res * lod);
            int ind = 0;

            for (int y = 0; y < res; y += 1)
            {
                for (int x = 0; x < res; x += 1)
                {
                    md.vertices[ind] = new Vector3(startX + x, hCurve.Evaluate(noise.skin.GetPixel(x, y).grayscale) * multiplier, startZ - y);
                    md.uvs[ind] = new Vector2(x / (float)res, y / (float)res);

                    if (x < res - 1 && y < res - 1)
                    {
                        md.AddTriangle(ind, ind + res, ind + res - 1);
                        md.AddTriangle(ind + res, ind, ind + 1);
                    }

                    ind++;
                }
            }

            return md;
        }
        
        public void procCmds(int buttonIndex)
        {
            Debug.Log(gMode);
            switch (buttonIndex)
            {
                //
 //           GM.Get().scene.miscRefs[0].SetActive(false);
                case 0: //Diamond Square
                    if (GM.Get().scene.buttonRefs[0].GetComponent<Toggle>().isOn)
                    {
                        GM.Get().scene.miscRefs[0].SetActive(false);
                        GM.Get().scene.buttonRefs[1].GetComponent<Toggle>().isOn = false;
                        GM.Get().framework.type = NoiseType.DiamondSquare;
                    } else
                    {
                        GM.Get().scene.miscRefs[0].SetActive(true);
                        GM.Get().scene.buttonRefs[1].GetComponent<Toggle>().isOn = true;
                        GM.Get().framework.type = NoiseType.BasicPerlin;
                    }
                    break;
                case 1: //Perlin
                    if (GM.Get().scene.buttonRefs[1].GetComponent<Toggle>().isOn)
                    {
                        GM.Get().scene.miscRefs[0].SetActive(true);
                        GM.Get().scene.buttonRefs[0].GetComponent<Toggle>().isOn = false;
                        GM.Get().framework.type = NoiseType.BasicPerlin;
                    }
                    else
                    {
                        GM.Get().scene.miscRefs[0].SetActive(false);
                        GM.Get().scene.buttonRefs[0].GetComponent<Toggle>().isOn = true;
                        GM.Get().framework.type = NoiseType.DiamondSquare;
                    }
                    break;
                case 2: //Generate
                    generateNoise();
                    skinObj.GetComponent<Image>().enabled = false;
                    skinObj.GetComponent<Image>().enabled = true;
                    break;
                case 3: //LOD
                    int.TryParse(GM.Get().scene.buttonRefs[3].GetComponent<InputField>().text, out lod);
                    break;
                case 4: //Multiplier
                    int.TryParse(GM.Get().scene.buttonRefs[4].GetComponent<InputField>().text, out multiplier);
                    break;
                case 5: //Resolution
                    int.TryParse(GM.Get().scene.buttonRefs[5].GetComponent<InputField>().text, out res);
                    break;
                case 6: //Frequency
                    float.TryParse(GM.Get().scene.buttonRefs[6].GetComponent<InputField>().text, out freq);
                    break;
                case 7: //Octave
                    int.TryParse(GM.Get().scene.buttonRefs[7].GetComponent<InputField>().text, out octave);
                    break;
                case 8: //Lacunarity
                    float.TryParse(GM.Get().scene.buttonRefs[8].GetComponent<InputField>().text, out lacunarity);
                    break;
                case 9: //Persistence
                    float.TryParse(GM.Get().scene.buttonRefs[9].GetComponent<InputField>().text, out persistence);
                    break;
                case 10: //Dimension
                    dimension = (int) GM.Get().scene.buttonRefs[10].GetComponent<Slider>().value + 2;
                    break;
            }

        }
    }
}

public class MeshGenData
{

    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshGenData(int width, int height)
    {
        vertices = new Vector3[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];
        uvs = new Vector2[width * height];
    }

    public void AddTriangle(int v1, int v2, int v3)
    {
        triangles[triangleIndex] = v1;
        triangles[triangleIndex + 1] = v2;
        triangles[triangleIndex + 2] = v3;
        triangleIndex += 3;
    }

    public Mesh MakeMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
