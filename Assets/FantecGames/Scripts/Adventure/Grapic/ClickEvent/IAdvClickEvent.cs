using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace fantec
{

    /// <summary>
    /// グラフィックオブジェクトのデータ
    /// </summary>
    internal interface IAdvClickEvent
    {
        GameObject gameObject { get; }

        /// <summary>
        /// クリックイベントを設定
        /// </summary>
        void AddClickEvent(bool isPolygon, StringGridRow row, UnityAction<BaseEventData> action);

        /// <summary>
        /// クリックイベントを削除
        /// </summary>
        void RemoveClickEvent();
    }
}
