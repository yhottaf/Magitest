using Cysharp.Threading.Tasks;
using UnityEngine;
using fantec.Menu.Notice.View;
using fantec.Common;
using UniRx;

namespace fantec.Menu.Notice.Presenter
{
    public class NoticePresenter : MonoBehaviour
    {

        public GameObject noticeEntryPrefab;
        public Transform noticeListParent;
        [SerializeField]
        private NoticeView m_View;
        private bool isCheckNotice;
        async void Start()
        {
            m_View.InitCanvas();
            m_View.ShowCanvas();

            // 全お知らせデータの読み込み
            var notices = MasterDataManager.Instance.NoticeMaster.dataList; 
            int i = 0;
            foreach (var notice in notices)
            {
                var entry = Instantiate(noticeEntryPrefab, noticeListParent);
                await entry.GetComponent<NoticeEntryCell>().Setup(
                    notice, 
                    m_View.GetNoticeTextObj(), 
                    m_View.GetReceiveButton(),
                    m_View.GetNoticeHeader(),
                    m_View.GetScheduleStartDate(),
                    m_View.GetRewardCellPrefab(),
                    m_View.GetScrollRect(),
                    m_View.GetBtnTextObj()
                    );
                if (i == 0)
                {
                    // ループの初めならその初めのお知らせを最初に表示させる
                    await entry.GetComponent<NoticeEntryCell>().ShowNotice(notice); // 非同期で呼び出し
                    m_View.SetTitleTypeObj(notice);
                }
                i++;
            }

            m_View.OnClickCloseButtonObservable.Subscribe(OnClickCloseButton).AddTo(this);
            m_View.OnValueChangeCheckToggleObservable.Subscribe(isOn => { if (isOn) OnValueChangedToggle(isOn); }).AddTo(this);
        }

        /// <summary>
        /// お知らせ画面を閉じる
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickCloseButton(Unit unit)
        {
            //SE 再生


            if(isCheckNotice)
            {
                // 再度ログインしたときにお知らせを非表示にする
                PlayerPrefsManager.IsNoticeFlag = true;

            }
            else
            {
                // 再度ログインしたときまたお知らせを表示する
                PlayerPrefsManager.IsNoticeFlag = false;
            }
            m_View.HideCanvas();
        }

        /// <summary>
        /// 本日はお知らせを再度表示しないかどうかのチェック
        /// </summary>
        /// <param name="enable"></param>
        private void OnValueChangedToggle(bool enable)
        {
            isCheckNotice = enable;
        }
    }
}