using System.Collections.Generic;
using UnityEngine;

namespace fantec.Master
{
    public interface IData
    {

    }

    public class MasterBase<TData> : ScriptableObject where TData : IData
    {
        public List<TData> dataList;
    }
}