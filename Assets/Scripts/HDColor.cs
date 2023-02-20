using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDColor
{
    public byte R = 15, G = 15, B = 15;

    public Color ReturnColor(float Alpha = 1)
    {
        return new Color(((float)(R * 16 + 8) / 256), ((float)(G * 16 + 8) / 256), ((float)(B * 16 + 8) / 256), Alpha);
    }
}
