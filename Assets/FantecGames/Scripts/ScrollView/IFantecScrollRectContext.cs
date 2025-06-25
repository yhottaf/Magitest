using fantec;
using System;
using UnityEngine;

namespace FantecScrollView
{
    public interface IFantecScrollRectContext
    {
        ScrollDirection ScrollDirection { get; set; }
        Func<(float ScrollSize, float ReuseMargin)> CalculateScrollSize { get; set; }
    }
}