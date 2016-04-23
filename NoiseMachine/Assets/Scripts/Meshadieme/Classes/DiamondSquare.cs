using Meshadieme;
using Meshadieme.Math;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiamondSquare : Noise
{
    
    Color32[] colors;
    int GRAIN = 8;
    int dummy = 0;


    public DiamondSquare()
    {
        pTypes = new NoiseMethod[] {
            DiamondP2
        };
        colors = new Color32[res * res];
        
    }

    public override void FillColor()
    {
        if (skin.width != res)
        {
            skin.Resize(res, res);
        }

        Vector3 vecaa = obj.TransformPoint(new Vector3(-0.5f, -0.5f));
        Vector3 vecba = obj.TransformPoint(new Vector3(0.5f, -0.5f));
        Vector3 vecab = obj.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 vecbb = obj.TransformPoint(new Vector3(0.5f, 0.5f));

        NoiseMethod method = pTypes[0];


        float r1, r2, r3, r4;
   
        r1 = Random.value;
        r2 = Random.value;
        r3 = Random.value;
        r4 = Random.value;

        iterateDivision(0.0f, 0.0f, res, r1, r2, r3, r4);
        skin.SetPixels32(colors);
        skin.Apply();
    }

    public float DiamondP2(Vector3 point, float frequency, int[] hash)
    {
        return 1.0f;
    }

    Color recolor(float c)
    {
        float Red = 0;
        float Green = 0;
        float Blue = 0;

        if (c < 0.5f)
        {
            Red = c * 2;
        }
        else
        {
            Red = (1.0f - c) * 2;
        }
        if (c >= 0.3 && c < 0.8f)
        {
            Green = (c - 0.3f) * 2;
        }
        else if (c < 0.3f)
        {
            Green = (0.3f - c) * 2;
        }
        else
        {
            Green = (1.3f - c) * 2;
        }

        if (c >= 0.5f)
        {
            Blue = (c - 0.5f) * 2;
        }
        else
        {
            Blue = (0.5f - c) * 2;
        }

        return new Color(Red, Green, Blue);
    }

    float moveAround(float num)
    {
        float max = (num) / (res) * GRAIN;
        return Random.Range(-0.5f, 0.5f) * max;
    }
    //Main Stuff is here
    //Subdivides the image until single pixels remain
    void iterateDivision(float x, float y, int _res, float r1, float r2, float r3, float r4)
    {

        float newres = _res * 0.5f;

        if (_res < 1.0f)// Last pixels are corners they get avaraged.
        {
            float c = (r1 + r2 + r3 + r4) * 0.25f;
            colors[(int)x + (int)y * res] = recolor(c);
        }
        else //move pixels around randomly and redivide 
        {
            float middle = (r1 + r2 + r3 + r4) * 0.25f + moveAround(newres + newres);      //Randomly move middle
            float edge1 = (r1 + r2) * 0.5f; //Calculate the edges
            float edge2 = (r2 + r3) * 0.5f;
            float edge3 = (r3 + r4) * 0.5f;
            float edge4 = (r4 + r1) * 0.5f;

            //fix it from going past limits
            if (middle <= 0)
            {
                middle = 0;
            }
            else if (middle > 1.0f)
            {
                middle = 1.0f;
            }

            //iterate for all four edges                 
            iterateDivision(x, y, (int) newres, r1, edge1, middle, edge4);
            iterateDivision(x + newres, y, (int)newres, edge1, r2, edge2, middle);
            iterateDivision(x + newres, y + newres, (int) newres, middle, edge2, r3, edge3);
            iterateDivision(x, y + newres, (int)newres, edge4, middle, edge3, r4);
        }
    }

}