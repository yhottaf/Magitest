using Cysharp.Threading.Tasks;
using fantec.Utilities;
using System;
using UnityEngine;
using UniRx;
using NUnit.Framework;
using System.Collections.Generic;

namespace fantec.Debugger {
    public class DebuggerInItemPresenter : ItemDuplicator<DebuggerInItemPresenter>
    {
        [SerializeField] private DebuggerInItemView m_View;

        protected override void Release()
        {
        }

        protected override void Setup(ItemDuplicatorData data)
        {
            var itemData = data as DebuggerInItemData;

            m_View.SetTitleText(itemData.titleText);
            m_View.OnClickObservable.Subscribe(_ => itemData.onClick?.Invoke()).AddTo(this);
        }
    }

    public class DebuggerInItemData:ItemDuplicatorData
    {
        public string titleText;
        public Action onClick;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DebuggerInItemData(string titleText,Action onClick)
        {
            this.titleText = titleText;
            this.onClick = onClick;
        }

        public DebuggerInItemData() { }

        /// <summary>
        /// 複数まとめて登録する際に使用
        /// </summary>
        /// <param name="titleText">表示タイトル</param>
        /// <param name="onClick">クリック時の処理</param>
        /// <param name="count">回す回数</param>
        /// <returns>DebuggerInItemData の配列</returns>
        public static DebuggerInItemData[]GetDatas(string titleText,Action<int>onClick,int count)
        {
            List<DebuggerInItemData>inItemDataList=new List<DebuggerInItemData>();
            for(int i=0;i<count;i++)
            {
                var index = i;
                inItemDataList.Add(new DebuggerInItemData() { titleText = titleText + (index + 1), onClick = () => onClick?.Invoke(index) });
            }

            return inItemDataList.ToArray();
        }

        public static DebuggerInItemData[] GetDatas(string[]titleText,Action<int>onClick)
        {
            List<DebuggerInItemData>inItemDataList=new List<DebuggerInItemData>();
            for(int i=0;i<titleText.Length;i++)
            {
                var index = i;
                inItemDataList.Add(new DebuggerInItemData() { titleText = titleText[i], onClick = () => onClick?.Invoke(index) });
            }

            return inItemDataList.ToArray();
        }
    }
}