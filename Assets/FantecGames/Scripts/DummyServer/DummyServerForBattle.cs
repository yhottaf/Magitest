using Cysharp.Threading.Tasks;
using fantec.Battle.Manager;
using fantec.Battle;
using fantec.Common;
using fantec.Master;
using fantec.PlayFabClient;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace fantec
{

    public class DummyServerForBattle
    {
        public class Cashe
        {
            public static bool isCleard;
            public static int stageId;
            public static int stamina = 1;
            public static int initalRewardTableId;
            public static int stageRewardTableId;
            public static int rewardExp;

            public static LotteryForBattleData CreateErrorLotteryData()
            {
                return new LotteryForBattleData(false, -1, -1, 0);
            }

            public static LotteryForBattleData CreateSuccessLotteryData()
            {
                return new LotteryForBattleData(true,
                    initialRewardTableId:        Cashe.initalRewardTableId,
                    stageRewardTableId:          Cashe.stageRewardTableId,
                    rewardExp:                   Cashe.rewardExp);
            }
        }

        /// <summary>
        /// オンラインであるか否か
        /// </summary>
        public static bool IsOfflineMode => UserDataManager.User == null;

        /// <summary>
        /// バトル開始時に抽選し結果を保持する
        /// </summary>
        public static async UniTask<LotteryForBattleData>GetLotteryAsync(int stageId)
        {
            await DummyDelayAsync();

            var stageData = MasterDataManager.Instance.StageMaster.GetData(stageId);

            Cashe.stageId = stageId;
            Cashe.stamina = stageData.stamina;
            Cashe.isCleard = UserDataManager.IsQuestClear(stageId);
            Cashe.initalRewardTableId = stageData.clearReword;
            Cashe.stageRewardTableId = stageData.rewardTableId;
            Cashe.rewardExp = stageData.rewardExp;

            return Cashe.CreateSuccessLotteryData();
        }

        /// <summary>
        /// 当選結果を指定する場合
        /// </summary>
        public static async UniTask<LotteryForBattleData> GetLotteryChoiseAsync(int stageId)
        {
            await DummyDelayAsync();

            var stageData = MasterDataManager.Instance.StageMaster.GetData(stageId);

            Cashe.stageId = stageId;
            Cashe.stamina = stageData.stamina;
            Cashe.isCleard = false;
            Cashe.initalRewardTableId = stageData.clearReword;
            Cashe.stageRewardTableId = stageData.rewardTableId;
            Cashe.rewardExp = stageData.rewardExp;

            return Cashe.CreateSuccessLotteryData();
        }


        public static async UniTask<List<ConsumeItemData>>GetDropItemAsync()
        {
            if(IsOfflineMode)
            {
                return await GetDropItemForLocalAsync();
            }
            else
            {
                return await GetDropItemForPlayFabAsync();
            }
        }

        /// <summary>
        /// 所持石数を取得する
        /// </summary>
        public static(int freeStone,int paidStone)GetStones()
        {
            if (IsOfflineMode) return (0, 0);
            return(VirtualCurrencyManager.FreeStone,VirtualCurrencyManager.PaidStone);
        }

        /// <summary>
        /// 保有スタミナ数を取得する
        /// </summary>
        public static int GetCurrentStamina()
        {
            if (IsOfflineMode) return 0;
            return VirtualCurrencyManager.Stamina;
        }

        /// <summary>
        /// スタミナの消費を試みる
        /// </summary>
        public static async UniTask<bool>TryConsumeStaminaAsync()
        {
            if (IsOfflineMode) return true;
            if(Cashe.stamina<=VirtualCurrencyManager.Stamina)
            {
                await VirtualCurrencyManager.SubtractStaminaAsync(Cashe.stamina);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 石の消費を試みる
        /// </summary>
        public static async UniTask<bool>TryConsumeStoneAsync()
        {
            var consumeStoneCount = 1;
            if (IsOfflineMode) return true;
            if(consumeStoneCount<=VirtualCurrencyManager.FreeStone)
            {
                await VirtualCurrencyManager.SubtractStoneAsync(consumeStoneCount);
                return true;
            }
            else if(consumeStoneCount<=VirtualCurrencyManager.PaidStone)
            {
                // TODO : 有償石消費処理
                return false;
            }
            else
            {
                return false;
            }
        }

        public static async UniTask<bool>SaveStageclearFlagAsync()
        {
            if (IsOfflineMode)
            {
                // 何もしない
                return false;
            }
            else
            {
                UserDataManager.AddClearQuestId(Cashe.stageId);
                await UserDataManager.UpdatePlayFab();
                return true;
            }
        }

        public static async UniTask<List<ConsumeItemData>> GetDropItemForLocalAsync()
        {
            await DummyDelayAsync();
            var resourceManager = Locator.Resolve<IBattleResourceManager>();
            var result=new List<ConsumeItemData>();
            if (Cashe.stageRewardTableId == -1) return result;
            var table = MasterDataManager.Instance.RewardStageMaster.GetDataLocal(Cashe.stageRewardTableId);
            var weightList=new List<int>();
            var lotteryCount = UnityEngine.Random.Range(table.minLottery, table.maxLottery + 1);
            if (table.itemId1 != "-1") weightList.Add(table.weight1);else weightList.Add(0);
            if (table.itemId2 != "-1") weightList.Add(table.weight2);else weightList.Add(0);
            if (table.itemId3 != "-1") weightList.Add(table.weight3);else weightList.Add(0);

            for(int i=0;i<lotteryCount;i++)
            {
                switch(GetRandomIndex(weightList.ToArray()))
                {
                    case 0:result.Add(MasterDataManager.Instance.ConsumeItemMaster.GetData(int.Parse(table.itemId1)));
                        await resourceManager.CasheSpriteAsync(AssetPath.GetSpriteItemIcon(int.Parse(table.itemId1)), System.Threading.CancellationToken.None);
                        break;
                    case 1:result.Add(MasterDataManager.Instance.ConsumeItemMaster.GetData(int.Parse(table.itemId2)));
                        await resourceManager.CasheSpriteAsync(AssetPath.GetSpriteItemIcon(int.Parse(table.itemId2)), System.Threading.CancellationToken.None);
                        break;
                    case 2:result.Add(MasterDataManager.Instance.ConsumeItemMaster.GetData(int.Parse(table.itemId3)));
                        await resourceManager.CasheSpriteAsync(AssetPath.GetSpriteItemIcon(int.Parse(table.itemId3)), System.Threading.CancellationToken.None);
                        break;
                }
            }

            if (Cashe.isCleard == false && Cashe.initalRewardTableId != -1)
            {
                var initialTable = MasterDataManager.Instance.InitialRewardStageMaster.GetData(Cashe.initalRewardTableId);

                if (initialTable.itemId1 != "-1") for (int i = 0; i < initialTable.quantity1; i++)
                    { 
                        result.Add(MasterDataManager.Instance.ConsumeItemMaster.GetData(int.Parse(initialTable.itemId1)));
                        await resourceManager.CasheSpriteAsync(AssetPath.GetSpriteItemIcon(int.Parse(initialTable.itemId1)), System.Threading.CancellationToken.None);
                    }
                if (initialTable.itemId2 != "-1") for (int i = 0; i < initialTable.quantity2; i++)
                    { 
                        result.Add(MasterDataManager.Instance.ConsumeItemMaster.GetData(int.Parse(initialTable.itemId2)));
                        await resourceManager.CasheSpriteAsync(AssetPath.GetSpriteItemIcon(int.Parse(initialTable.itemId2)), System.Threading.CancellationToken.None);
                    }
                if (initialTable.itemId3 != "-1") for (int i = 0; i < initialTable.quantity3; i++)
                    { 
                        result.Add(MasterDataManager.Instance.ConsumeItemMaster.GetData(int.Parse(initialTable.itemId3)));
                        await resourceManager.CasheSpriteAsync(AssetPath.GetSpriteItemIcon(int.Parse(initialTable.itemId3)), System.Threading.CancellationToken.None);
                    }

                if (initialTable.cardId.Length != 0)
                {
                    foreach(var cardId in initialTable.cardId)
                    {
                        result.Add(new ConsumeItemData() { itemId = cardId });
                        await resourceManager.CasheSpriteAsync(AssetPath.GetCharacterSpriteSpherePath(cardId), System.Threading.CancellationToken.None);
                    }
                }
            }

            return result;
        }

        public static async UniTask<List<ConsumeItemData>>GetDropItemForPlayFabAsync()
        {
            var result =new List<ConsumeItemData>();
            if (Cashe.stageRewardTableId == -1) return result;
            var resourceManager = Locator.Resolve<IBattleResourceManager>();
            var table = MasterDataManager.Instance.RewardStageMaster.GetDataLocal(Cashe.stageRewardTableId);
            var lotteryCount=UnityEngine.Random.Range(table.minLottery,table.maxLottery+1);
            
            Debug.Log($"抽選回数[{lotteryCount}]");

            List<string>itemIdList=new List<string>();

            for(int i=0;i<lotteryCount;i++)
            {
                itemIdList.Add("RewardStage" + Cashe.stageRewardTableId);
            }

            if(Cashe.isCleard==false&&Cashe.initalRewardTableId!=-1)
            {
                itemIdList.Add("InitialReward" + Cashe.initalRewardTableId);
            }

            var response = await PlayFab.PlayFabServerAPI.GrantItemsToUserAsync(new PlayFab.ServerModels.GrantItemsToUserRequest()
            {
                ItemIds=itemIdList,
                PlayFabId=PlayerProfileManager.PlayFabId,
            });

            await UserDataManager.UpdatePlayFab();

            var bundleContents = response.Result.ItemGrantResults.Where(x => x.BundleContents != null).SelectMany(x => x.BundleContents);

            foreach(var bundle in bundleContents)
            {
                var catalogItem = CatalogManager.CatalogItems[bundle];
                var itemClass=(ItemClass)Enum.Parse(typeof(ItemClass),catalogItem.ItemClass);

                switch(itemClass)
                {
                    case ItemClass.Item:result.Add(MasterDataManager.Instance.ConsumeItemMaster.GetData(int.Parse(catalogItem.ItemId.Replace("Item", ""))));
                        await resourceManager.CasheSpriteAsync(AssetPath.GetSpriteItemIcon(int.Parse(catalogItem.ItemId.Replace("Item", ""))),System.Threading.CancellationToken.None);
                        break;
                    case ItemClass.Card:result.Add(new ConsumeItemData() { itemId = int.Parse(catalogItem.ItemId) });
                        await resourceManager.CasheSpriteAsync(AssetPath.GetCharacterSpriteSpherePath(int.Parse(catalogItem.ItemId)), System.Threading.CancellationToken.None);
                        break;
                    
                    default:throw new NotImplementedException($"[ItemClass {itemClass}] には対応していません。");
                }
            }

            return result;
        }

        /// <summary>
        /// ユーザーランクを上げ結果を返す
        /// </summary>
        public static async UniTask<(int oldExp,int gainExp)>UserRankUpAsync()
        {
            var oldExp = 0;
            var gainExp = Cashe.rewardExp;

            if(UserDataManager.User!=null)
            {
                oldExp = VirtualCurrencyManager.Exp;
                await VirtualCurrencyManager.AddExpAsync(gainExp);
            }
            return (oldExp, gainExp);
        }

        // ---------------------------------------------------------------------
        // Utilities
        // ---------------------------------------------------------------------
        
        public static async UniTask DummyDelayAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        }

        public static int GetRandom(params int[]Params)
        {
            return Params[UnityEngine.Random.Range(0, Params.Length)];
        }

        public static bool GetIsWinner(int probability)
        {
            return UnityEngine.Random.Range(1, 100) <= probability;
        }

        public static int GetRandomIndex(params int[] weightTable)
        {
            var totalWeight = weightTable.Sum();
            var value = UnityEngine.Random.Range(1, totalWeight + 1);
            var retIndex = -1;
            for(var i=0; i<weightTable.Length;++i)
            {
                if (weightTable[i]>=value)
                {
                    retIndex = i;
                    break;
                }
                value -= weightTable[i];
            }
            return retIndex;
        }
    }
}