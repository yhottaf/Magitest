using DG.Tweening;
using UnityEngine;

namespace fantec.Battle.Manager.Pool
{
    public class IndexingPopNumeralObject : AbstractIndexingNumeralObject<IndexingPopNumeralObject>
    {
        public override void Return()
        {
            Locator.Resolve<IBattlePoolManager>().Return(this);
        }

        public virtual IndexingPopNumeralObject Setup(int num)
        {
            // サイズ調整
            this.transform.localScale = Vector3.one;
            var startPos = this.transform.localPosition;
            // パスの定義：右上 → 右中 → 右下（半円風）
            var path = new Vector3[]
            {
    startPos + new Vector3(0.5f,  1.2f, 0f),  // 右上
    startPos + new Vector3(1.0f,  1.0f, 0f),  // 中央
    startPos + new Vector3(1.1f,  0.9f, 0f),  // 右下（落ちる）
            };

            // 特殊演出中に再生された場合
            if (base.m_IsDirectingPause)
            {
                base.SetThroughDirectingPause(true);
                base.m_SortingGroup.sortingLayerName = BD.SortingLayer.NAME_FIELD_BLACKOUT;
            }
            else 
            {
                // 設定を戻す
                base.SetThroughDirectingPause(false);
                base.m_SortingGroup.sortingLayerName = BD.SortingLayer.NAME_FIELD_DEFAULT;
            }

            base.m_SpriteNumber.SetScale(m_Size);
            base.m_SpriteNumber.Show(num);

            base.m_Sequence.Kill();
            base.m_Sequence.Value = DOTween.Sequence()
                .Append(DOVirtual.Float(0f, 1f, 0.3f, value => base.m_SpriteNumber.SetAlpha(value)))
                .Join(this.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.3f).From(new Vector3(30.5f, 30.5f, 1f)))
                .Append(DOVirtual.Float(1f, 0f, 0.5f, value => base.m_SpriteNumber.SetAlpha(value)))
                .Join(this.transform.DOLocalPath(path, 0.7f, PathType.CatmullRom))
                .SetLink(this.gameObject);

            return this;
        }
    }
}