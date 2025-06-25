using fantec.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Master
{
    [System.Serializable]
    public class NoticeResult
    {
        public string status;
        public string[] granted;
        public string title;
        public string body;
        public string titletype;
    }

    [System.Serializable]
    public class NoticeListResponse
    {
        public List<NoticeData> notices;
    }
    [System.Serializable]
    public class NoticeData : IData
    {
        public string key;                    // お知らせのキー (固有番号のような識別番号)
        public string title;                  // お知らせタイトル
        public string body;                   // お知らせ本文
        public List<string> rewardItemIds;    // 付与したいプレゼントのIDやBundle番号を格納
        public string titletype;              // お知らせタイプ (運営からのおしらせ　不具合など)
        public bool canReceive;               // プレゼントを受け取ったかどうか
        public string ScheduledStartDate;   // 配信する日
        public string ScheduledEndDate;     // 配信終了日 
    }

    [ExcelAsset(AssetPath = AssetPath.MasterLocalDataFolderPath), CreateAssetMenu(fileName = "NoticeMaster", menuName = "ScriptableObjects/NoticeMaster")]
    public class NoticeMaster : MasterBase<NoticeData>
    {
        public NoticeData GetData(string key)
        {
            try
            {
                return dataList.First(x => x.key == key);
            }
            catch
            {
                throw new InvalidOperationException($"[ItemId : {key}] は存在しません。");
            }
        }
    }
}