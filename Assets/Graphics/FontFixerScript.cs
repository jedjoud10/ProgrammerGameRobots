using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A small script that takes the texture of every font and sets the filtering mode to point instead of bilinear
public class FontFixerScript : MonoBehaviour
{
    public Font[] fonts;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var font in fonts)
        {
            font.material.mainTexture.filterMode = FilterMode.Point;//Set's font filtering mode
        }
    }
}
