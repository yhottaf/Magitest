using UnityEngine;

namespace fantec.Battle.Manager.Placement
{
    public class PlacementPointer : MonoBehaviour
    {
        [SerializeField] private Transform m_DefaultPoint;      // 基準位置
        [SerializeField] private Transform m_BeAttackedPoint;   // 攻撃を受ける際の位置
        [SerializeField] private Transform m_OffScreenPoint;    // 画面外にいる際の位置
        [SerializeField] private Transform m_CenterPoint;       // キャラクターの真芯
        [SerializeField] private Transform m_KnockBackPoint;    // ノックバックの位置
        [SerializeField] private Transform m_BlowBackPoint;     // 後ろ吹き飛びの位置

        public Vector2 DefaultPosition => m_DefaultPoint ? m_DefaultPoint.position : this.transform.position;
        public Vector2 BeAttackedPosition => m_BeAttackedPoint ? m_BeAttackedPoint.position : this.transform.position;
        public Vector2 OffScreenPoint => m_OffScreenPoint ? m_OffScreenPoint.position : this.transform.position;
        public Vector2 CenterPoint => m_CenterPoint ? m_CenterPoint.position : this.transform.position;
        public Vector2 KnockBackPoint => m_KnockBackPoint ? m_KnockBackPoint.position : this.transform.position;
        public Vector2 BlowBackPoint => m_BlowBackPoint ? m_BlowBackPoint.position : this.transform.position;
    }
}