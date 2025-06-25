using DG.Tweening;
using fantec.Master;
using fantec.Menu.Manager;
using fantec.Notice.View;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Menu.Notice.View
{
    public class NoticeView : MonoBehaviour
    {
        [SerializeField]
        private Button m_CloseButton; // お知らせの閉じるボタン
        [SerializeField]
        private RectTransform m_dialog; // お知らせのCanvas
        [SerializeField]
        private Text m_NoticeBodyText; // お知らせ本文
        [SerializeField]
        private Button m_ReceiveButton; // 受け取りボタン
        [SerializeField]
        private Text m_BtnText;         // 受け取りボタンのテキスト
        [SerializeField]
        private Text m_NoticeHeader; // お知らせの見出し
        [SerializeField]
        private Text m_ScheduleStartDate; // お知らせ掲載日
        [SerializeField]
        private GameObject[] m_TitleTypeObj; // タイトルの種類によってオンオフを切り替えるオブジェクト群 (運営からのお知らせなど)
        [SerializeField]
        private RewardNoticeCell m_RewardNoticeCell; // 受け取れるアイテムを表示するプレハブ
        [SerializeField]
        private ScrollRect m_RewardScrollView;       // プレハブの親にするScrollRect
        [SerializeField]
        private Toggle m_CheckToggle; // 今日はゲーム開始時にお知らせを再度表示するかどうかのチェック。チェックありで表示しない
        [SerializeField]
        private Scrollbar m_Scrollbar; // お知らせ内のスクロールバー
        public IObservable<Unit>OnClickCloseButtonObservable=>m_CloseButton.OnClickAsObservable();
        public IObservable<bool> OnValueChangeCheckToggleObservable => m_CheckToggle.OnValueChangedAsObservable();
        public Text GetNoticeTextObj()
        {
            return m_NoticeBodyText;
        }

        public Button GetReceiveButton()
        {
            return m_ReceiveButton;
        }

        public Text GetNoticeHeader()
        {
            return m_NoticeHeader;
        }

        public Text GetScheduleStartDate()
        {
            return m_ScheduleStartDate;
        }

        public RewardNoticeCell GetRewardCellPrefab()
        {
            return m_RewardNoticeCell;
        }

        public ScrollRect GetScrollRect()
        {
            return m_RewardScrollView;
        }

        public Text GetBtnTextObj()
        {
            return m_BtnText;
        }

        public void SetTitleTypeObj(NoticeData notice)
        {
            if (notice != null)
            {
                switch (notice.titletype)
                {
                    case "運営のお知らせ":
                        m_TitleTypeObj[0].SetActive(true);
                        m_TitleTypeObj[1].SetActive(false);
                        break;
                    case "不具合":
                        m_TitleTypeObj[1].SetActive(true);
                        m_TitleTypeObj[0].SetActive(false);
                        break;
                    default:
                       for(int i=0;i<m_TitleTypeObj.Length;i++)
                        {
                            m_TitleTypeObj[i].SetActive(false);
                        }
                        break;
                }
            }
        }

        public void InitCanvas()
        {
            m_dialog.localScale = Vector3.zero;
            m_Scrollbar.value = 1.0f;
        }

        public void ShowCanvas()
        {
            MenuManager.Instance.onLive2DState.OnNext(Unit.Default); // 非アクティブ状態にするため通知
            m_dialog.gameObject.SetActive(true);
            m_dialog.localScale = Vector3.zero; // 念のため
            m_Scrollbar.value = 1.0f;
            m_dialog.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack). // アニメーション
                OnComplete(() =>ShowEndAnim());
        }

        public void HideCanvas()
        {
            m_dialog.DOScale(Vector3.zero, 0.2f)
                  .SetEase(Ease.InBack)
                  .OnComplete(() =>HideEndAnim());
        }

        private void ShowEndAnim()
        {
            MenuWindowManager.Instance.Remove(MenuWindowManager.CreateType.InputGuard);
            m_Scrollbar.value = 1.0f;
        }

        private void HideEndAnim()
        {
            MenuWindowManager.Instance.Remove(MenuWindowManager.CreateType.Notice);
            MenuManager.Instance.onLive2DState.OnNext(Unit.Default); // 非アクティブ状態にするため通知
        }
    }
}