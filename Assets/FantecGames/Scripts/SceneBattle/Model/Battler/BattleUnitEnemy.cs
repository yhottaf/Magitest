using Cysharp.Threading.Tasks;
using fantec.Battle.Manager;
using fantec.Common;
using System.Collections.Generic;
using System.Threading;

namespace fantec.Battle.Model
{
    public class BattleUnitEnemy : AbstructBattler,IEnemy
    {
        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Reload()
        {
            base.Reload();
        }

        public override void SetUp(TeamData.Unit unit, bool isTakeover = false, bool leader = false)
        {
            var cardData = Locator.Resolve<IBattleMasterManager>().EnemyCardMaster.GetData(unit.cardId);
            var normalEntity = NormalAttackEntity.GetEntity(cardData.attributeType);
            var overrideEntity = OverrideSkillEntityExtensions.ConvertToEntity(cardData.attributeType, new List<(int id, int level)>
            {
                (cardData.OverrideId,unit.OverrideLevel),            // リストの0番目にオーバーライド
                (cardData.ExsaOverrideId,unit.ExsaOverrideLevel),    // リストの1番目にエクサ
                (cardData.SectaOverrideId,unit.SectaOverrideLevel),  // リストの2番目にゼタ
                (cardData.QuetaOverrideId, unit.QuetaOverrideLevel), // リストの3番目にクエタを入れる
            });

            var unitEntity = cardData.ToInfoEntity(unit.rarityType, unit.positionIndex);
            var stateEntity = cardData.ToStateEntity(unit.cardLevel,leader);
            m_Unit.Setup(unitEntity);
            m_State.Setup(stateEntity);

            m_OverrideSkill.Setup(overrideEntity, normalEntity);
            m_AdventSkill.Setup(normalEntity);

            // Unitの情報が必要なので最後にセットアップ
            m_Transform.Setup(this.GetBattlerPosition());

            base.SetupCompleted();
        }
    }
}