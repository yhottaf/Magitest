using DG.Tweening;
using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace fantec.Battle.Ui.Window
{
    public class WinResultDropItemView : MonoBehaviour
    {
        [SerializeField] private WinResultDropItemCell m_Cell;
        [SerializeField] private Sprite m_NewLabelSprite;
        [SerializeField] private Sprite m_InitLabelSprite;

        private readonly List<WinResultDropItemCell>m_CellList=new List<WinResultDropItemCell>();

        private Sequence m_Sequence;

        public void Init()
        {
            m_Cell.gameObject.SetActive(false);
        }

        public void PlayDropItem(IEnumerable<RewerdInfo>rewardInfos,Action onCompleted)
        {
            m_Sequence = DOTween.Sequence();

            foreach(var data in rewardInfos)
            {
                m_Sequence.AppendCallback(() =>
                {
                    var clone = Instantiate(m_Cell, m_Cell.transform.parent);
                    clone.gameObject.SetActive(true);
                    clone.transform.localScale = Vector2.one;
                    clone.SetIconImage(data.sprite);
                    clone.SetCountText($"x {data.Count}");
                    clone.PlayFlash();
                    m_CellList.Add(clone);  
                }).AppendInterval(0.3f)
                .OnComplete(() => onCompleted.Invoke());
            }
        }

        public void ClearDropItem()
        {
            foreach(var cell in m_CellList)
            {
                Destroy(cell.gameObject);
            }

            m_CellList.Clear();
        }
    }
}