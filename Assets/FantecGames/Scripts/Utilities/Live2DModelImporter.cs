using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using UnityEngine;
using Live2D.Cubism.Rendering;

using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fantec.Utilities
{
    public class Live2DModelImporter : MonoBehaviour
    {
        [Header("Live2D Model Files")]
        public TextAsset Moc3File;                // .moc3 ファイル
        public List<Texture2D> Textures;          // テクスチャリスト
        public GameObject ModelRoot;              // モデルの親オブジェクト

        private CubismModel cubismModel;
        private CubismMoc cubismMoc;

    }
}