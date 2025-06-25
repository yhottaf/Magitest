using fantec.Battle.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Battle
{
    public class RewerdInfo
    {
        public readonly Sprite sprite;
        public readonly string itemName;
        public readonly int itemId;

        public int Count { get; private set; }

        public RewerdInfo(Master.ConsumeItemData data)
        {
            this.sprite = Locator.Resolve<IBattleResourceManager>().GetDropItemSprite(data.itemId);
            this.itemName = data.name;
            this.itemId = data.itemId;
            this.Count = 1;
        }

        /// <summary>
        /// 報酬リストをもとに Info を作りだし返す。
        /// </summary>
        public static IEnumerable<RewerdInfo>CreateInfos (IEnumerable<Master.ConsumeItemData>rewards)
        {
            var rewardInfoList=new List<RewerdInfo>();
            foreach(var reward in rewards)
            {
                var temp=rewardInfoList.FirstOrDefault(x=>x.itemId== reward.itemId);
                if(temp==null)
                {
                    rewardInfoList.Add(new RewerdInfo(reward));
                }
                else
                {
                    temp.AddCount();
                }
            }

            return rewardInfoList;
        }

        public void AddCount()
        {
            Count++;
        }
    }
}