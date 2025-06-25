using fantec.Battle.Manager.Placement;
using fantec.Battle.Model;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public interface IBattlePlacementManager : IRegistable
    {
        int GetNextPositionIndex(IBattler battler);
        void SetDefaultPosition(IBattler affecter, int positionindex);
        Vector3 GetBeAttackPosition(IBattler battler);
        Vector3 GetOffScreenPosition(IBattler battler);
        Vector3 GetBattlerPosition(IBattler battler);
        Vector3 GetCenterPosition(IBattler battler);
        Vector3 GetKnockBackPosition(IBattler battler);
        Vector3 GetBlowBackPosition(IBattler battler);
        Vector3 GetBeAttackCenterPosition(IBattler battler);
        Vector3 GetBuffCenterPosition(IBattler battler);
        Vector3 GetHomingPosition(IBattler battler);
        Vector3 GetCenterPosition();
        Vector3 GetBottomPosition();
        Vector3 GetTopPosition();
        Vector3 GetDestination(AffectInfo info);
    }
    public class BattlePlacementManager : MonoBehaviour, IBattlePlacementManager
    {
        [Header("UI")]
        [Header("Field")]
        [SerializeField] private PlacementPointer[] m_PlayerPoints;
        [SerializeField] private PlacementPointer[] m_EnemyPoints;
        [SerializeField] private Transform m_PlayerCenterPoint;
        [SerializeField] private Transform m_EnemyCenterPoint;
        [SerializeField] private Transform m_CenterPoint;
        [SerializeField] private Transform m_BottomPoint;
        [SerializeField] private Transform m_TopPoint;

        public void Register()
        {
            Locator.Register<IBattlePlacementManager>(this);
        }

        // マス目に移動する際に、現在のUnitのいるマス目情報を書き換える
        public void SetDefaultPosition(IBattler affecter, int positionindex)
        {
            affecter.Unit.Entity.positionIndex = positionindex;
        }

        private PlacementPointer GetPointer(IBattler affecter)
        {
            try
            {
                return affecter.GetIsEnemy() ?
                    m_EnemyPoints[affecter.Unit.Entity.positionIndex] :
                    m_PlayerPoints[affecter.Unit.Entity.positionIndex];
            }
            catch { throw new System.Exception($"[{affecter.Unit.Entity.positionIndex}] は範囲外です。"); }
        }


        public Vector3 GetBeAttackPosition(IBattler battler) => GetPointer(battler).BeAttackedPosition;
        public Vector3 GetOffScreenPosition(IBattler battler) => GetPointer(battler).OffScreenPoint;
        public Vector3 GetCenterPosition(IBattler battler) => GetPointer(battler).CenterPoint;
        public Vector3 GetBattlerPosition(IBattler affecter) => GetPointer(affecter).DefaultPosition;
        public Vector3 GetKnockBackPosition(IBattler battler) => GetPointer(battler).KnockBackPoint;

        public Vector3 GetBlowBackPosition(IBattler battler) => GetPointer(battler).BlowBackPoint;
        public Vector3 GetHomingPosition(IBattler battler)
        {
            return GetPointer(battler).CenterPoint;
        }

        /// <summary>
        /// 全体攻撃する際の位置を取得
        /// </summary>
        /// <param name="affecter"></param>
        public Vector3 GetBeAttackCenterPosition(IBattler affecter)
        {
            return affecter.GetIsEnemy() ?
                m_PlayerCenterPoint.position :
                m_EnemyCenterPoint.position;
        }

        /// <summary>
        /// 全体バフをする際の位置を取得
        /// </summary>
        /// <param name="affecter"></param>
        public Vector3 GetBuffCenterPosition(IBattler affecter)
        {
            return affecter.GetIsEnemy() ?
                m_EnemyCenterPoint.position :
                m_PlayerCenterPoint.position;
        }

        public Vector3 GetCenterPosition() => m_CenterPoint.position;
        public Vector3 GetBottomPosition() => m_BottomPoint.position;
        public Vector3 GetTopPosition() => m_TopPoint.position;


        // TODO : 攻撃する際の移動先を決定したい場合ここをいじる

        /// <summary>
        /// 発動スキルによって移動先を取得
        /// </summary>
        public Vector3 GetDestination(AffectInfo info)
        {
            var rangeType = info.Command.rangeType;
            var affecter = info.Owner;
            var targets = info.Targets;

            switch (rangeType)
            {
                case AffectRangeType.EnemySingle:
                    return this.GetBeAttackPosition(targets.ToSingle());
                case AffectRangeType.EnemyAll:
                case AffectRangeType.EnemyAllRandom:
                    return this.GetBeAttackCenterPosition(affecter);

                case AffectRangeType.Myself:
                case AffectRangeType.MySideSingle:
                case AffectRangeType.MySideVanguard:
                case AffectRangeType.MySideRearguard:
                case AffectRangeType.BesideMeRandom:
                case AffectRangeType.MySideAllRandom:
                    return this.GetBattlerPosition(affecter);

                case AffectRangeType.BesidesMe:
                case AffectRangeType.MySideAll:
                    return this.GetBuffCenterPosition(affecter);

                default: throw new System.Exception($"[rangeType:{rangeType}] が設定されていません。");
            }
        }

        /// <summary>
        /// MAGIが行動パターンに従って次に移動するべき自陣のpositionIndexの値を返す
        /// positionIndex(9つあるPlacementPointerの場所)0～8までの値しかない
        /// </summary>
        public int GetNextPositionIndex(IBattler affecter)
        {
            // 行動順番情報がない場合は現在の位置情報を返す
            if (affecter.Unit.Entity.behaviorAI.Count.Equals(0)) return affecter.Unit.Entity.positionIndex;

            for(int i=0;i<affecter.Unit.Entity.behaviorAI.Count;i++)
            {
                if (affecter.Unit.Entity.positionIndex == affecter.Unit.Entity.behaviorAI[i])
                {
                    if (i + 1 >= affecter.Unit.Entity.behaviorAI.Count)// カウントは9までが限界
                    {
                        return affecter.Unit.Entity.behaviorAI[0]; // リスト内の最初の行動パターンの位置を返す
                    }
                    else
                    {
                        return affecter.Unit.Entity.behaviorAI[i + 1];// 次の行動パターンの位置を返す
                    }
                }
            }

            return affecter.Unit.Entity.positionIndex;
        }
    }
}