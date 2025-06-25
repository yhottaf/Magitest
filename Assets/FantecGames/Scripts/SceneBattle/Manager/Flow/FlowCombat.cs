using fantec.Battle.Manager.Flow;
using fantec.Battle.Model;
using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Manager
{
    public partial class BattleFlowManager
    {
        public partial class FlowCombat : FlowBase
        {
            private IBattler m_TurnStartLastBattler;// 最後に行動するユニットの記憶 (ターン経過観測用)
            private ICombatSequence m_CombatSection;
            private bool m_IsInaction;  // 誰かが行動中か否か

            public override void OnEnter(BattleFlowManager manager, FlowBase prevFlow)
            {
                // 基本的に最初のターンにしか入ってこないが、カットイン後はOnEnterに入ってくる設計のため、
                // カットイン後に変数が初期化されるため、再度そのターンの最後に行動するユニットを取得している。
                if (prevFlow.ToString() == "FlowOverrideCutin")
                {
                    m_TurnStartLastBattler = Locator.Resolve<IBattleModelStage>().LastActingUnit;// 最後に行動するユニット情報の観測
                }
                else
                {
                    m_TurnStartLastBattler = Locator.Resolve<IBattleModelAdventSkill>().ReserveLast; // 最後に行動するユニット
                    Locator.Resolve<IBattleModelStage>().SetLastActingUnit(m_TurnStartLastBattler);
                    Debug.Log($"ターン : {Locator.Resolve<IBattleModelStage>().CurrentTurn}");
                }
                   // UIなどの制御が必要ならここに記載
            }

            // ターン終了時に false に変える
            private void OnTurnCompleted()
            {
                m_IsInaction = false;

                if (Locator.Resolve<IBattleModelOverrideSkill>().OnIsFook.Value)
                {

                }
                else // オーバードライブのフラグが立っていないとき、そのままターンエンド処理
                {


                    // 決着がついていた場合はターン経過処理を行わない
                    if (Locator.Resolve<IBattleModelUnits>().IsSettled == false
                       && Locator.Resolve<IBattleModelStage>().IsMaxTurn == false)
                    {
                        //  行動順の更新により行動順の最後尾に回ってきたのが、
                        // 「1ターンの始まり時に最後尾にいるユニットであった場合」
                        if (m_TurnStartLastBattler == Locator.Resolve<IBattleModelAdventSkill>().ReserveLast)
                        {
                            Locator.Resolve<IBattleModelStage>().NextTurn(); // ターンの経過
                        }
                    }

                }
            }

            // 主に行動ロジック
            public override void OnUpdate(BattleFlowManager manager)
            {
                if (m_IsInaction) // 行動中であれば
                {
                    // 何もしない
                    //  Debug.Log("現在行動中です。");
                }
                else if (Locator.Resolve<IBattleModelTime>().OnIsSystemPause.Value) // 停止中であれば
                {
                    // 何もしない
                    Debug.Log("現在停止中です。");
                }
                else if (manager.m_ReserveFlow != null)
                {
                    manager.ChangeFlow(manager.m_ReserveFlow);
                }
                else if (Locator.Resolve<IBattleModelUnits>().IsSettled == true// 決着がついていれば
                    || Locator.Resolve<IBattleModelStage>().IsMaxTurn == true) // 最大ターンに到達したら
                {
                    manager.ChangeFlow<FlowDefeat>();
                }
                else if(Locator.Resolve<IBattleModelOverrideSkill>().OnIsActive.Value)                      // オーバーライドフラグが有効であれば
                {
                    m_IsInaction = true;                                                                    // 行動中フラグの有効化
                    m_CombatSection = new CombatOverrideSkillSequence(manager.m_Sequence, OnTurnCompleted); // オーバーライドの発動
                    m_CombatSection.Execute();
                }
                else if  (Locator.Resolve<IBattleModelOverrideSkill>().OnIsFook.Value==true) // オーバーライドスキルが発動待ちであれば
                {
                    Locator.Resolve<IBattleModelStage>().SetLastActingUnit(m_TurnStartLastBattler); // 事前に今のターンの最後の行動者を保存しておく
                    manager.ChangeFlow<FlowOverrideCutin>();                                 // オーバーライドスキルカットインへ
                }
                else  // 何もなければ
                {
                    m_IsInaction = true;
                    m_CombatSection = new CombatAdventSkillSequence(manager.m_Sequence, OnTurnCompleted);
                    m_CombatSection.Execute();
                }

            }

            public override void OnExit(BattleFlowManager manager, FlowBase nextFlow)
            {
                m_CombatSection?.Dispose();
            }
        }
    }
}