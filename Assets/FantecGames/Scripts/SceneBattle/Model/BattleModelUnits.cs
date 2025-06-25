using Cysharp.Threading.Tasks;
using fantec.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Model
{
    public class BattleModelUnits : MonoBehaviour,IBattleModelUnits,IRegistable
    {
        public IObservable<IBattleModelUnits> OnInitCompletedObservable => m_SetupCompletedSubject;
        public IObservable<Unit> OnSettledObservable => m_DefeatPlayerSubject.Merge(m_DefeatEnemySubject);

        public IObservable<Unit> OnDefeatPlayerObservable => m_DefeatPlayerSubject;

        public IObservable<Unit> OnDefeatEnemyObservable => m_DefeatEnemySubject;
        public IObservable<IEnumerable<IBattler>> OnUpdatePlayerMemberObservable => m_UpdatePlayerMemberSubject;
        public IObservable<IEnumerable<IBattler>> OnUpdateEnemyMemberObservable => m_UpdateEnemyMemberSubject;

        public IObservable<IBattler> OnReviveBattlerObservable => m_ReviveBattlerSubject;

        public IObservable<IBattler> OnDeadBattlerObservable => m_DeadBattlerSubject;
        public IObservable<AffectInfo> OnActionAdventObserveble => m_ActionAdventSubject;
        public IObservable<AffectInfo> OnActionOverrideSkillObservable => m_ActionOverrideSubject;
        public IObservable<AffectInfo> OnBuffedObservable => m_BuffedSubject;
        public IObservable<Unit> OnBreakObservable => m_BreakSubject;

        public IBattler BossData => m_EnemyDatas[BD.POSITION_INDEX_BOSS];// 仮で3体に設定
        public IBattler[] PlayerDatas => m_PlayerDatas;
        public IBattler[]EnemyDatas=> m_EnemyDatas;
        public bool IsPlayerDefeat => m_PlayerDatas.GetIsAllDead();
        public bool IsEnemyDefeat => m_EnemyDatas.GetIsAllDead();

        public float PlayerHPRatio => m_PlayerDatas.GetHPRatio();
        public float EnemyHPRatio => m_EnemyDatas.GetHPRatio();

        public bool IsSettled => IsPlayerDefeat || IsEnemyDefeat; // 決着がついたかどうか


        private readonly AsyncSubject<IBattleModelUnits> m_SetupCompletedSubject = new AsyncSubject<IBattleModelUnits>();
        private readonly Subject<IEnumerable<IBattler>> m_UpdatePlayerMemberSubject=new Subject<IEnumerable<IBattler>>();
        private readonly Subject<IEnumerable<IBattler>> m_UpdateEnemyMemberSubject=new Subject<IEnumerable<IBattler>>();
        private readonly Subject<Unit>m_DefeatEnemySubject=new Subject<Unit>();
        private readonly Subject<Unit>m_DefeatPlayerSubject=new Subject<Unit>();   
        private readonly Subject<IBattler> m_DeadBattlerSubject=new Subject<IBattler>();
        private readonly Subject<IBattler>m_ReviveBattlerSubject= new Subject<IBattler>();
        private readonly Subject<AffectInfo> m_ActionAdventSubject = new Subject<AffectInfo>();
        private readonly Subject<AffectInfo> m_ActionOverrideSubject = new Subject<AffectInfo>();
        private readonly Subject<AffectInfo> m_BuffedSubject=new Subject<AffectInfo>();
        private readonly Subject<Unit>m_BreakSubject=new Subject<Unit>();
        private IBattler[] m_PlayerDatas = new IBattler[Define.PARTY_CAPACITY];
        private IBattler[] m_EnemyDatas=new IBattler[Define.PARTY_CAPACITY];

        private readonly CompositeDisposable m_OnDestroyDisposable=new CompositeDisposable();


        public void Register()
        {
            Locator.Register<IBattleModelUnits>(this);

            for(int i=0;i<Define.PARTY_CAPACITY;i++)
            {
                var index = i;
                m_PlayerDatas[index] = new BattleUnitPlayer();
                m_EnemyDatas[index] = new BattleUnitEnemy();
            }

            //TODO エネミーデータの数分ボスを加えたりする

            foreach (var player in PlayerDatas)
            {
                player.State.Health.OndeadObservable.Subscribe(_ => OnDeadPlayer(player)).AddTo(m_OnDestroyDisposable);
                player.State.Health.OnRevivalObservable.Subscribe(_ => OnRevive(player)).AddTo(m_OnDestroyDisposable);
                player.State.OnTakeBuffObservable.Subscribe(m_BuffedSubject).AddTo(m_OnDestroyDisposable);
                player.AdventSkill.OnActivationObservable.Subscribe(m_ActionAdventSubject).AddTo(m_OnDestroyDisposable);
                player.OverrideSkill.OnActivationObservable.Subscribe(m_ActionOverrideSubject).AddTo(m_OnDestroyDisposable);
            }

            foreach (var enemy in EnemyDatas)
            {
                enemy.State.Health.OndeadObservable.Subscribe(_ => OnDeadEnemy(enemy)).AddTo(m_OnDestroyDisposable);
                enemy.State.Health.OnRevivalObservable.Subscribe(_ => OnRevive(enemy)).AddTo(m_OnDestroyDisposable);
                enemy.State.OnTakeBuffObservable.Subscribe(m_BuffedSubject).AddTo(m_OnDestroyDisposable);
                enemy.AdventSkill.OnActivationObservable.Subscribe(m_ActionAdventSubject).AddTo(m_OnDestroyDisposable);
                enemy.OverrideSkill.OnActivationObservable.Subscribe(m_ActionOverrideSubject).AddTo(m_OnDestroyDisposable);
            }
        }

        public void Dispose()
        {
            m_SetupCompletedSubject.Dispose();
            m_OnDestroyDisposable.Dispose();
            m_PlayerDatas.Dispose();
            m_EnemyDatas.Dispose();
            m_DeadBattlerSubject.Dispose();
            m_DefeatEnemySubject.Dispose();
            m_DefeatPlayerSubject.Dispose();
            m_ReviveBattlerSubject.Dispose();
            m_UpdateEnemyMemberSubject.Dispose();
            m_UpdatePlayerMemberSubject.Dispose();
        }

        #region Subscribe

        private void OnDeadPlayer(IBattler battler)
        {
            m_DeadBattlerSubject.OnNext(battler);

            // 全滅している場合
            if(IsPlayerDefeat)
            {
                m_DefeatPlayerSubject.OnNext(Unit.Default);
            }
        }

        private void OnDeadEnemy(IBattler battler)
        {
            m_DeadBattlerSubject.OnNext(battler);

            // 全滅している場合
            if(IsEnemyDefeat)
            {
                m_DefeatEnemySubject.OnNext(Unit.Default);
            }
        }

        private void OnRevive(IBattler battler)
        {
            m_ReviveBattlerSubject.OnNext(battler);
        }

        #endregion

        #region IBattleModelUnits

        public void Reload()
        {
            m_PlayerDatas.Reload();
            m_EnemyDatas.Reload();
        }

        public void ResetPlayerTeam()
        {
            m_PlayerDatas.Reset();
        }

        public void ResetEnemyTeam()
        {
            m_EnemyDatas.Reset();
        }

        /// <summary>
        /// 味方パーティーメンバーを戦闘監視対象に登録する
        /// </summary>
        public void SetPlayerTeam(TeamData team)
        {
            // 旧データの破棄
            m_PlayerDatas.Reset();

            // 新データの登録
            m_PlayerDatas.SetUnitData(team);

            // 存在するキャラを通知
            m_UpdatePlayerMemberSubject.OnNext(m_PlayerDatas.GetExistBattlers());
        }

        /// <summary>
        /// 敵パーティーメンバーを戦闘監視対象に登録する
        /// </summary>
        public void SetEnemyTeam(TeamData team)
        {
            // 旧データの破棄
            m_EnemyDatas.Reset();

            // 新データの登録
            m_EnemyDatas.SetUnitData(team);

            // 存在するキャラを通知
            m_UpdateEnemyMemberSubject.OnNext(m_EnemyDatas.GetExistBattlers());
        }
        #endregion
    }
}