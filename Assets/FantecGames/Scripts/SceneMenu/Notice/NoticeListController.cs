using System.Collections.Generic;

namespace fantec.Menu.Notice
{
    [System.Serializable]
    public class NoticeData
    {
        public string key;
        public string title;
        public string body;
        public string[] rewardItemIds;
        public string titletype;
        public bool canReceive;
    }

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
}