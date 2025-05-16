using fantec.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace fantec.Common
{
    public class LocalDataManager : PersistentSingleton<LocalDataManager>
    {
        private string filePath;
        public LocalData LocalData { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            filePath = Application.persistentDataPath + "/" + ".localdata.json";
            LocalData = new LocalData();
            Load();
        }

        protected override void OnDestroy()
        {
            Save();
            base.OnDestroy();
        }

        private void Save()
        {
            if(LocalData!=null)
            {
                string json=JsonConvert.SerializeObject(LocalData,Formatting.None);
                StreamWriter streamWriter=new StreamWriter(filePath);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        private void Load()
        {
            if(File.Exists(filePath))
            {
                StreamReader streamReader;
                streamReader = new StreamReader(filePath);
                string data = streamReader.ReadToEnd();
                streamReader.Close();
                LocalData=JsonConvert.DeserializeObject<LocalData>(data);
            }
        }
    }

    [JsonObject]
    public class LocalData
    {
        [JsonProperty("FavoriteList")]
        private List<int> FavoriteCardList = new List<int>();
        [JsonProperty("CheckCardLIst")]
        private List<int> CheckCardList = new List<int>();
        [JsonProperty("QuestClearIdList")]
        private List<int> QuestClearIdList = new List<int>();

        public void AddFavoriteCard(int cardId)
        {
            if(FavoriteCardList.Contains(cardId))
            {
                return;
            }

            FavoriteCardList.Add(cardId);
        }

        /// <summary>
        /// お気に入りを削除する
        /// </summary>
        /// <param name="cardId"></param>
        public void RemoveFavoriteCard(int cardId)
        {
            if(FavoriteCardList.Contains(cardId))
            {
                FavoriteCardList.Remove(cardId);
            }
        }

        /// <summary>
        /// お気に入りリストに含まれているか
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public bool IsFavorite(int cardId)
        {
            return FavoriteCardList.Contains(cardId);
        }

        /// <summary>
        /// 表示したことのあるカードを追加する
        /// </summary>
        /// <param name="cardId"></param>
        public void AddCheckCard(int cardId)
        {
            if(CheckCardList.Contains(cardId))
            {
                return;
            }

            CheckCardList.Add(cardId);
        }

        /// <summary>
        /// 表叔父したことのあるリストから、cardIdを表示したことがあるかを返す
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public bool IsNew(int cardId)
        {
            return CheckCardList.Contains(cardId) == false;
        }
    }
}