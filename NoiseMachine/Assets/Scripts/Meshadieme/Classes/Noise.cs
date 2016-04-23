//Partial cited from http://catlikecoding.com/unity/tutorials/noise/#a-bitwise-and


using Meshadieme;
using UnityEngine;
using System.Collections;
using System.IO;

namespace Meshadieme
{

    public enum NoiseType
    {
        BasicPerlin,
        DiamondSquare,
    }


    public class Noise {

        public delegate float NoiseMethod(Vector3 point, float frequency, int[] hash);

        public NoiseMethod[] pTypes;

        [Range(2, 2048)]
        public int res = 1024;

        public Texture2D skin;

        public Gradient color;

        public NoiseType type;

        public Transform obj;

        public void WriteToFile()
        {
            byte[] byteData = skin.EncodeToPNG();

            File.WriteAllBytes(Application.dataPath + "/../Noise.png", byteData);

        }

        public virtual void FillColor()
        {
            //per noise type fill (uses their own variables)
        }


    }

}


