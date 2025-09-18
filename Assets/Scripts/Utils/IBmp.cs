using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class IBmp
    {

        public static Texture2D getDataPicture(int w, int h, Color[] data)
        {
            Texture2D texture2D = new Texture2D(w, h, TextureFormat.ARGB32, false);
            texture2D.SetPixels(0, 0, w, h, data);
            texture2D.Apply();
            return texture2D;
        }
    }
}
