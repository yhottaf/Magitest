using System;
using UnityEngine;

namespace fantec
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public class ExcelAssetAttribute:Attribute
    {
        public string AssetPath { get; set; }
    }
}