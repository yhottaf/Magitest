using fantec.Battle.Manager;
using fantec.Battle.Model;

namespace fantec.Battle
{
    public partial class BattlerExtentions
    {
        /// <summary>
        /// ダメージ効果反映時のパーティクル再生
        /// </summary>
        public static void PlayTakeDamageParticle(this IBattler @this,TakeDamageInfo info)
        {
            var affectInfo = info.affectInfo;

            if(affectInfo.GetIsAffectable())
            {
                // TODO: エフェクト表示やらヒット音再生やらを書く
                // エフェクト表示
                Locator.Resolve<IBattlePoolManager>().Rent(PoolableSpotParticle.GetIndex(info.AttributeType)).SetPosition(@this.GetCenterPosition()).Setup();
                // 数値非表示が無効の場合
                if (affectInfo.IsHideView==false)
                {
                    // 数値表示
                    Locator.Resolve<IBattlePoolManager>().Rent(PoolableNumeral.Index.Numeral_Damage).SetPosition(@this.GetCenterPosition()).Setup(info.valueInfo.affectValue);
                }
            }
            else
            {
                // Miss or Guard 表示
               // Locator.Resolve<IBattlePoolManager>().Rent(PoolableSpotParticle.GetIndex(info.affectInfo.HitType)).SetPosition(@this.GetCenterPosition()).Setup();
            }
        }

        /// <summary>
        ///  回復効果反映時のパーティクル再生
        /// </summary>
        public static void PlayTakeHealParticle(this IBattler @this,TakeHealInfo info)
        {
            var affectInfo = info.affectInfo;
            
            // 効果が反映される場合
            if(affectInfo.GetIsAffectable())
            {
                // TODO: 効果音の再生

                // エフェクト表示
                Locator.Resolve<IBattlePoolManager>().Rent(PoolableSpotParticle.GetIndex(affectInfo.Command.categoryType)).SetPosition(@this.GetCenterPosition()).Setup();

                // 数値非表示が無効の場合
                if(affectInfo.IsHideView==false)
                {
                    // 数値表示
                    Locator.Resolve<IBattlePoolManager>().Rent(PoolableNumeral.Index.Numeral_Heal).SetPosition(@this.GetCenterPosition()).Setup(info.valueInfo.affectValue);
                }
            }
            else
            {
                // Miss 表記 (回復失敗)
            }
        }

        public static void PlayTakeBuffParticle(this IBattler @this,AffectInfo info)
        {
            // 効果が反映される場合
            if(info.GetIsAffectable())
            {
                var categoryType = info.Command.categoryType;

                var center = @this.GetCenterPosition();
                var target = @this.GetTargetPosition();

                // TODO: 効果音再生や、エフェクト表示
            }
            else
            {
                // Miss 表示
            }
        }
    }
}