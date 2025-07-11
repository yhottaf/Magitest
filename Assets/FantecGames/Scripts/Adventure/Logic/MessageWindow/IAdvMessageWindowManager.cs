using UnityEngine;
using System.Collections.Generic;

namespace fantec
{
    //実際にUIとしてヒエラルキーに存在するメッセージウィンドウの管理オブジェクトのコンポーネントの共通インターフェース
    public interface IAdvMessageWindowManager
    {
        GameObject gameObject { get; }
        Dictionary<string, IAdvMessageWindow> AllWindows { get; }
    }
}