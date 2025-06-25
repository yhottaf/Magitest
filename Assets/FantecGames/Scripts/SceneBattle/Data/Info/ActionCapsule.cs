using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.Battle
{
    public class ActionCapsule
    {
        public List<AbstructSkillEntity> EntityList { get; private set; }
        public bool IsDanger { get; private set; }

        public bool IsMimic { get; private set; }

        public bool IsConsumable=>EntityList.Count > 0;

        private const int TwoUnitValue = 2;
        private const int ThreeUnitValue = 3;

        public ActionCapsule() { }
        public ActionCapsule(List<AdventSkillEntity> entityList) : this(Lottely(entityList)) { }

        public ActionCapsule(AbstructSkillEntity entity) : this(new List<AbstructSkillEntity>() { entity }) { }

        public ActionCapsule(List<AbstructSkillEntity>entityList)
        {
            Update(entityList);
        }

        public ActionCapsule ShallowCopy()
        {
            return (ActionCapsule)MemberwiseClone();
        }

        public void Copy(ActionCapsule actionCapsule)
        {
            this.EntityList = actionCapsule.EntityList;
            this.IsDanger=actionCapsule.IsDanger;
            this.IsMimic=actionCapsule.IsMimic;
        }

        public void Update(List<AbstructSkillEntity>entityList)
        {
            this.EntityList = entityList;
            if (entityList.Count == 0) return; // 空なら何もしない

            this.IsDanger = entityList.First().GetIs<DangerEntity>(); // デンジャー保留識別
            //this.IsMimic = 30 >= Random.Range(0, 100) && entityList.Count >= 2;
        }

        public void Clear()
        {
            this.EntityList.Clear();
        }

        /// <summary>
        /// ミミック状態フラグ解除
        /// </summary>
        public void LiftMimic()
        {
            this.IsMimic = false;
        }

        public int GetIndex()
        {
            if(IsDanger)
            {
                // デンジャー保留は-1 として返す
                return -1;
            }
            // それ以外の通常の場合
            else
            {
                // 発動予定のスキル数を返す
                return EntityList.Count - 1;
            }
        }

        /// <summary>
        /// 発動するスキルを取り出す
        /// </summary>
        public AbstructSkillEntity Dequeue()
        {
            try
            {
                var result = EntityList.First();
                EntityList.Remove(result);
                return result;
            }
            catch { throw new System.InvalidOperationException("スキルを消費し切った状態で呼ばれました。"); }
        }

        /// <summary>
        /// 保有スキルと重なっているユニットのOriginIDを元に結果を取得
        /// </summary>
        private static List<AbstructSkillEntity>Lottely(List<AdventSkillEntity> entityList,bool isBoot=false)
        {
            var resultList=new List<AbstructSkillEntity>();
            foreach(var entity in entityList)
            {
                resultList.Add(entity);
                //if (GetIsActivateSkill(entity,originId))
                //{
                //    // 発動許可されたスキルのみ、発動予定リストに入れる
                //    resultList.Add(entity);
                //}
            }
            return resultList; // 発動予定のスキルのリストを返す
        }

        // 対象になっているカードのoriginID
        private static bool GetIsActivateSkill(OverrideSkillEntity entity, int[] originId)
        {
            // そのスキルの発動に必要な固有カードIDの数と現在重なっているカードの固有IDの数があっていない場合、
            // そのスキルは発動の候補から外す (→ゼタオーバーライドが使用可能な人数であるのに、エクサオーバーライドを使用
            // するなどの条件を外す)
            if (originId.Length != entity.triggerCardOriginId.Length) return false;


            int OverrideValue = 0; // 発動に必要なoriginIdと重なっている対象のoriginIdが一致している数。
            for (int i = 0; i < originId.Length; i++)
            {
                for (int k = 0; i < entity.triggerCardOriginId.Length; k++)
                {
                    if (entity.triggerCardOriginId[k] == originId[i])
                    {
                        OverrideValue++;
                    }
                }
            }
            if(entity.overrideType==AffectOverrideType.Override
                &&OverrideValue.Equals(TwoUnitValue))
            {
                // 発動予定のスキルがオーバーライドの種別で、特定の固有IDを持つキャラが2体被っている場合
                // 発動候補から外す(→エクサオーバーライドを使用するようにする)
                return false;
            }
            else if(entity.overrideType==AffectOverrideType.ゼタオーバーライド
                &&OverrideValue.Equals(ThreeUnitValue))
            {
                // 発動予定のスキルがゼタオーバーライドの種別であり、特定の固有IDを持つキャラが3体被っている場合
                // 発動候補から外す(→クエタオーバーライドを使用するようにする)
                return false;
            }

            return true; // entity (発動予定スキルを発動予定リストに加えるのを許可する)
        }
    }
}