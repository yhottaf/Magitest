
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace fantec
{
    [Serializable]
    public class AdvGraphicInfoEvent : UnityEvent<AdvGraphicObject, AdvGraphicInfo>
    {
    }
}
