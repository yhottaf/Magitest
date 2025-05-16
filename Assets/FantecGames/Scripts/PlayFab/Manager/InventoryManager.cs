using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.PlayFabClient
{
    public class InventoryManager
    {
        public static Dictionary<int, ItemInstance> ConsumeItems { get; private set; }

        /// <summary>
        /// PlayFabから Client へデータを同期する。
        /// </summary>
        /// <param name="inventory"></param>
        public static void SyncPlayFabToClient(IEnumerable<ItemInstance> inventory)
        {
            ConsumeItems=inventory
                .Where(x=>x.ItemClass==ItemClass.Item.ToString())
                .ToDictionary(y=>int.Parse(y.ItemId.Replace("Item",string.Empty)));
        }

        /// <summary>
        /// アイテムを消費する
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="consumeCount"></param>
        /// <returns></returns>
        public static async UniTask<bool>ConsumeItemAsync(int itemId,int consumeCount)
        {
            // アイテム所持の確認
            if(ConsumeItems.ContainsKey(itemId)==false)
            {
                Debug.Log("アイテムを所持していません。");
                return false;
            }

            // アイテム数の確認
            if (ConsumeItems[itemId].RemainingUses<consumeCount)
            {
                Debug.Log("アイテム数が不足しています。");
                return false;
            }

            var consumeItemRequest = new ConsumeItemRequest
            {
                ItemInstanceId = ConsumeItems[itemId].ItemInstanceId,
                ConsumeCount = consumeCount
            };

            var result=await PlayFabClientAPI.ConsumeItemAsync(consumeItemRequest);

            if(result.Error!=null)
            {
                throw new PlayFabErrorException(result.Error);
                return false;
            }

            ConsumeItems[itemId].RemainingUses = result.Result.RemainingUses;

            return true;
        }
    }
}