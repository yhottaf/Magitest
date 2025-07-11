using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fantec
{
    [System.Serializable]
    public class AssetFileDummyOnLoadError
    {
        public bool isEnable = false;

        public bool outputErrorLog = true;

        public Texture2D texture;

        public AudioClip sound;

        public TextAsset text;

        public Object asset;
    }
}