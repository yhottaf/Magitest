using System.Collections.Generic;
using System.Linq;
using UniRx;
using PlayFab;
using PlayFab.ClientModels;
using fantec.Common;
using Cysharp.Threading.Tasks;

namespace fantec.PlayFabClient
{
    public class CardManager
    {
        private static List<ItemInstance> PlayFabCardDatas;

        public static List<CardData> CardDatas { get; private set; }

        /// <summary>
        /// PlayFab から Client へデータを同期する。
        /// </summary>
        /// <param name="inventory"></param>
        public static void SyncPlayFabToClient(IEnumerable<ItemInstance>inventory)
        {
            IEnumerable<ItemInstance> cardDatas = inventory.Where(x => x.ItemClass == ItemClass.Card.ToString());

            PlayFabCardDatas=cardDatas.ToList();
            CardDatas = new List<CardData>();
            List<int> count = new List<int> { 7, 7, 7, 7, 7 };

            foreach (var cardData in cardDatas)
            {

                if (cardData.CustomData == null)
                {
                    cardData.CustomData = new Dictionary<string, string>();
                }
                int cardId = int.Parse(cardData.ItemId);
                CardRarityType rarityType = cardData.CustomData.ContainsKey(nameof(CardData.rarityType))
                    ? (CardRarityType)int.Parse(cardData.CustomData[nameof(CardData.rarityType)])
                    : MasterDataManager.Instance.PlayerCardMaster.GetData(cardId).rarityType;

                int totalExp = cardData.CustomData.ContainsKey(nameof(CardData.totalExp))
                    ? int.Parse(cardData.CustomData[nameof(CardData.totalExp)])
                    : 0;
                List<int> positionIndex = cardData.CustomData.TryGetValue(nameof(CardData.positionIndex), out var posStr)
                    ? ParseIntList(posStr)
                    : new List<int> { 7, 7, 7, 7, 7 };

                // 限凸数などの設定があるならこことCardDataに記載
                CardDatas.Add(new CardData(cardId,rarityType,totalExp,cardData.PurchaseDate.Value,1,positionIndex));
            }
        }

        /// <summary>
        /// レアリティを上げるときに使用する
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public static async UniTask UpdateCardRarity(int cardId)
        {
            string itemInstanceId = PlayFabCardDatas.FirstOrDefault(x => x.ItemId == cardId.ToString()).ItemInstanceId;
            CardData cardData = GetCardData(cardId);

            if (cardData.rarityType == CardRarityType.R6)
            {
                return;
            }

            int rarity = ((int)cardData.rarityType + 1);

            var result = await PlayFabServerAPI.UpdateUserInventoryItemCustomDataAsync(new PlayFab.ServerModels.UpdateUserInventoryItemDataRequest()
            {
                ItemInstanceId = itemInstanceId,
                Data = new Dictionary<string, string>()
                {
                    {nameof(CardData.rarityType),rarity.ToString()}
                },
                PlayFabId = PlayFabSettings.staticPlayer.PlayFabId,
            });

            if (result.Error == null)
            {
                GetCardData(cardId).rarityType = (CardRarityType)rarity;
            }
        }

        /// <summary>
        /// レアリティを下げるときに使用する
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public static async UniTask DownGradeCardRarity(int cardId)
        {
            string itemInstanceId = PlayFabCardDatas.FirstOrDefault(x => x.ItemId == cardId.ToString()).ItemInstanceId;
            CardData cardData = GetCardData(cardId);

            if (cardData.rarityType == CardRarityType.R1)
            {
                return;
            }

            int rarity = ((int)cardData.rarityType - 1);

            var result = await PlayFabServerAPI.UpdateUserInventoryItemCustomDataAsync(new PlayFab.ServerModels.UpdateUserInventoryItemDataRequest()
            {
                ItemInstanceId = itemInstanceId,
                Data = new Dictionary<string, string>()
                {
                    {nameof(CardData.rarityType),rarity.ToString()}
                },
                PlayFabId = PlayFabSettings.staticPlayer.PlayFabId,
            });

            if (result.Error == null)
            {
                GetCardData(cardId).rarityType = (CardRarityType)rarity;
            }
        }

        /// <summary>
        /// 経験値を更新する
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static async UniTask UpdateCardTotalExp(int cardId,int exp)
        {
            string itemInstanceId=PlayFabCardDatas.FirstOrDefault(x=>x.ItemId==cardId.ToString()).ItemInstanceId;
            CardData cardData=GetCardData(cardId);

            int totalExp = cardData.totalExp + exp;

            var result = await PlayFabServerAPI.UpdateUserInventoryItemCustomDataAsync(new PlayFab.ServerModels.UpdateUserInventoryItemDataRequest()
            {
                ItemInstanceId = itemInstanceId,
                Data = new Dictionary<string, string>()
                {
                    {nameof(CardData.totalExp),totalExp.ToString()}
                },
                PlayFabId = PlayFabSettings.staticPlayer.PlayFabId,
            });

            if(result.Error==null)
            {
                GetCardData(cardId).totalExp = totalExp;
            }
        }

        // ディレクトリ(カードの編成) のカードの配置場所を保存する
        public static async UniTask UpdateCardPositionIndex(int cardId, List<int> positionIndex)
        {
            // -1 は「空きスロット」を意味するので、PlayFabには保存せず return
            if (cardId == -1)
            {
                return;
            }
            string itemInstanceId = PlayFabCardDatas.FirstOrDefault(x => x.ItemId == cardId.ToString())?.ItemInstanceId;

            if (string.IsNullOrEmpty(itemInstanceId)) return;

            var serializedPosition = SerializeIntList(positionIndex);

            var result = await PlayFabServerAPI.UpdateUserInventoryItemCustomDataAsync(new PlayFab.ServerModels.UpdateUserInventoryItemDataRequest()
            {
                ItemInstanceId = itemInstanceId,
                Data = new Dictionary<string, string>()
                {
                     { nameof(CardData.positionIndex), serializedPosition }
                },
                PlayFabId = PlayFabSettings.staticPlayer.PlayFabId,
            });

            if (result.Error == null)
            {
                GetCardData(cardId).positionIndex = positionIndex;
            }
        }

        // 限凸レベルやスキルレベルの更新(必要あれば)もここに記載するようにする

        /// <summary>
        /// カードの情報を取得する
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public static CardData GetCardData(int cardId)
        {
            return CardDatas.FirstOrDefault(x => x.cardId == cardId);
        }

        /// <summary>
        ///  ロード時に使用する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static List<int> ParseIntList(string value)
        {
            return value.Split(',').Select(s => int.TryParse(s, out var v) ? v : 0).ToList();
        }

        /// <summary>
        /// シリアライズ関数 (保存時に使用する)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static string SerializeIntList(List<int> list)
        {
            return string.Join(",", list);
        }
    }
}