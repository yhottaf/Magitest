using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using Newtonsoft.Json;
using fantec.Menu.Notice.View;

namespace fantec.Menu.Notice.Presenter
{
    public class NoticePresenter : MonoBehaviour
    {

        public GameObject noticeEntryPrefab;
        public Transform noticeListParent;
        [SerializeField]
        private NoticeView m_View;

        async void Start()
        {
            var notices = await GetAllNoticesAsync();
            int i = 0;
            foreach (var notice in notices)
            {
                var entry = Instantiate(noticeEntryPrefab, noticeListParent);
                await entry.GetComponent<NoticeEntryCell>().Setup(notice,m_View.GetNoticeTextObj(),m_View.GetReceiveButton());
                if(i==0)
                {
                    // ループの初めならその初めのお知らせを最初に表示させる
                    entry.GetComponent<NoticeEntryCell>().ShowNotice(notice).Forget(); // 非同期で呼び出し
                }
                i++;
            }
        }

        /// <summary>
        /// 全お知らせデータ一覧の取得
        /// </summary>
        /// <returns></returns>
        private async UniTask<List<NoticeData>> GetAllNoticesAsync()
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "GetAllNotices",
                GeneratePlayStreamEvent = false
            };

            var result = await PlayFabClientAPI.ExecuteCloudScriptAsync(request);
            if (result.Error != null)
            {
                Debug.LogError("お知らせ取得失敗: " + result.Error.GenerateErrorReport());
                return null;
            }

            var json = result.Result.FunctionResult.ToString();

            var noticeList = JsonConvert.DeserializeObject<NoticeListResponse>(json);
            return noticeList?.notices;
        }
    }

}