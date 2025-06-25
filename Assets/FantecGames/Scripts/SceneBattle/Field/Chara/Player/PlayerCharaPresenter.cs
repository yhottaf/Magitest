using fantec.Battle.Manager;
using fantec.Battle.Model;
using fantec.Battle.Utiles;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Field.Chara
{
    public class PlayerCharaPresenter : ICharaPresenter
    {
        public IBattler Battler =>m_Battler;
        private PlayerView m_View;
        private IBattler m_Battler;
        public PlayerCharaPresenter(PlayerView view)
        {
            m_View = view;
        }

        public void OnInitialize(IBattler battler)
        {
            m_Battler = battler;
            m_View.Initialize();
            m_View.SpineView.SetFlipX(battler.GetIsEnemy());
            m_View.SortingView.SetSortingOrder(BD.SortingLayer.FieldOrder.CharaSd);


            var overrideSkillStream = Locator.Resolve<IBattleModelOverrideSkill>().OnIsFook.DistinctUntilChanged();

            // false のとき
            overrideSkillStream
                .Where(x => !x)
                .Subscribe(_ =>
                {
                    var overrideModel = Locator.Resolve<IBattleModelOverrideSkill>();
                    if (!overrideModel.TryGetReserveHeadBattler(out var m_Battler)) return;

                    var entity = overrideModel.GetReserveHeadBattler()?.OverrideSkill?.Entity;
                    if (overrideModel.GetReserveHeadBattler() == m_Battler && entity != null)
                    {
                        m_View.MovementView.StopBlurTrail(); // 停止処理
                    }
                });
        }

        public void OnBreakRevival(Unit unit)
        {
            m_View.SpineView.SetColor(Color.white);
        }

        public void OnDead(AffectInfo info)
        {
            m_View.MovementView.StartFadeBody(1.2f, () => m_View.Hide());
        }

        public void OnOverrideCutin(OverrideSkillEntity data)
        {
            m_View.SetThroughPause(true);             // Pauseに設定
            m_View.SortingView.SetSortingNameBlack(); // カットイン以外の背景を薄暗く(黒の透過画像を出す)
        }

        public void OnOverrideEnd(Unit unit)
        {
            m_View.SetThroughPause(false); // Pauseを戻す
            m_View.SortingView.SetSortingNameDefault(); // ソート順を元に戻す
            m_View.MovementView.StopBlurTrail();
        }

        public void OnMove(Vector3 targetPosition)
        {
            m_View.MovementView.PlayMoveTo(targetPosition,0.4f,()=>
            {
                m_Battler.Transform.MoveCompleted();// 移動の完了通知を飛ばす
            });
        }

        public void OnReload(IBattler battler)
        {
            if(battler.Unit.IsExist&&battler.State.Health.IsDead==false)
            {
                m_View.Show();
                m_View.MovementView.PlayAdmission(m_Battler.GetBattlerPosition());
            }
            else
            {
                m_View.Hide();
            }
        }

        public void OnRevival(AffectInfo info)
        {
            m_View.Show();
            m_View.Reload();
            m_View.SpineView.PlayIdle();
            m_View.MovementView.SetPosition(m_Battler.GetOffScreenPosition());
            m_View.MovementView.PlayAdmission(m_Battler.GetBattlerPosition());
        }

        public void OnSkillAction(AffectInfo info)
        { 
            if(info.SkillType==SkillType.Dummy)
            {
                m_View.SpineView.PlayAction(info,
                 () => info.GetNotify().ActionCompleted());
                return;
            }

            if(info.IsRangedAttackSkill||
                (info.IsRangedAttacker&&info.IsRangedAttackable))
            {
                m_View.SpineView.PlayAction(info,
                    () => HomingLuncher.Lunch(info,
                    () => info.GetNotify().ActionCompleted()));
            }
            else
            {
                m_View.MovementView.PlayMoveTo(Locator.Resolve<IBattlePlacementManager>().GetDestination(info));
                m_View.SpineView.PlayAction(info,
                    ()=>info.GetNotify().ActionCompleted());
            }
        }

        public void OnTakeBuff(AffectInfo info)
        {
            // TODO: パーティクルの実装を書く
            Battler.PlayTakeBuffParticle(info);

            if(info.SkillType==SkillType.Style)
            {

            }
        }

        public void OnTakeDamage(TakeDamageInfo info)
        {
            var damage = info.valueInfo.affectValue;
            Debug.Log($"味方の {Battler.State.Entity.CharaName} に {damage} のダメージ!!");
            Debug.Log($"{Battler.State.Entity.CharaName} : {Battler.State.CurrentHP}/{Battler.State.OriginalMaxHP}");
            m_View.SpineView.PlayDamage(); // ダメージを受けたモーションの再生
            m_View.MovementView.PlayKnockBack(m_Battler.GetKnockBackPosition()); // 後方に飛ばす
            Locator.Resolve<IBattleCameraManager>().PlayShake(0.2f);
            Battler.PlayTakeDamageParticle(info);
        }

        public void OnTakeHeal(TakeHealInfo info)
        {
            Battler.PlayTakeHealParticle(info);
        }

        public void OnUpdateUnitData(IBattler battler)
        {
            if(battler.Unit.IsExist)
            {
                m_View.Reload();
                m_View.Show();
                m_View.SpineView.SetSkeletonDataAsset(Locator.Resolve<IBattleResourceManager>().GetCharaSpine(battler));
                
                m_View.SpineView.SetupFix();
                m_View.SpineView.PlayIdle();
                m_View.MovementView.SetPosition(m_Battler.GetOffScreenPosition());
                m_View.MovementView.PlayAdmission(m_Battler.GetBattlerPosition());
             //   m_View.SpineView.LoadShader();
            }
            else
            {
                m_View.Hide();
            }
        }

        public void OnMoveCompleted(Unit unit)
        {
            //Debug.Log($"プレイヤー側 :{Battler.State.Entity.CharaName} の移動完了を通知");
        }

        public void OnActivateBlur(Unit unit)
        {
            var overrideModel = Locator.Resolve<IBattleModelOverrideSkill>();
            var entity = overrideModel.GetReserveHeadBattler()?.OverrideSkill?.Entity;

            if ( entity != null)
            {
                m_View.MovementView.StartBlurTrail();
            }
        }
    }
}