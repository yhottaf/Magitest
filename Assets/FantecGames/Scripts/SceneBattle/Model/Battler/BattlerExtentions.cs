using Cysharp.Threading.Tasks;
using fantec.Battle.Model;
using fantec.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fantec.Battle
{
    public static partial class BattlerExtentions
    {
        public static bool GetIsPlayer(this IBattler battler) => battler is IPlayer;
        public static bool GetIsEnemy(this IBattler battler) => battler is IEnemy;
        public static bool GetIsBoss(this IBattler battler) => battler is IBoss;

        /// <summary> 一番上の横一列であるか否か
        public static bool GetIsHorizontalUp(this IBattler battler) =>
            battler.Unit.Entity.positionIndex == 0 ||
            battler.Unit.Entity.positionIndex == 4 ||
            battler.Unit.Entity.positionIndex == 1;

        /// <summary> 真ん中の横一列であるか否か
        public static bool GetIsHorizontalCenter(this IBattler battler) =>
            battler.Unit.Entity.positionIndex == 7 ||
            battler.Unit.Entity.positionIndex == 8 ||
            battler.Unit.Entity.positionIndex == 5;

        /// <summary> 一番下の横一列であるか否か
        public static bool GetIsHorizontalDown(this IBattler battler)=>
            battler.Unit.Entity.positionIndex == 3 ||
            battler.Unit.Entity.positionIndex == 6 ||
            battler.Unit.Entity.positionIndex == 2;

        /// <summary> 一番後ろの縦一列であるか否か
        public static bool GetIsVirticalBack(this IBattler battler)=>
            battler.Unit.Entity.positionIndex == 0 ||
            battler.Unit.Entity.positionIndex == 7 ||
            battler.Unit.Entity.positionIndex == 3;

        /// <summary> 真ん中の縦一列であるか否か
        public static bool GetIsVirticalCenter(this IBattler battler)=>
            battler.Unit.Entity.positionIndex == 4 ||
            battler.Unit.Entity.positionIndex == 8 ||
            battler.Unit.Entity.positionIndex == 6;

        /// <summary> 一番前の縦一列であるか否か
        public static bool GetIsVirticalFront(this IBattler battler)=>
            battler.Unit.Entity.positionIndex == 1 ||
            battler.Unit.Entity.positionIndex == 5 ||
            battler.Unit.Entity.positionIndex == 2;

        /// <summary> 味方であるか否かを返す
        public static bool GetIsPlayer(this IEnumerable<IBattler> battlers) => battlers.Any(battler => battler.GetIsPlayer());

        /// <summary> 敵であるか否かを返す
        public static bool GetIsEnemy(this IEnumerable<IBattler> battlers) => battlers.Any(battler => battler.GetIsEnemy());

        /// <summary> ボスであるか否かを返す
        public static bool GetIsBoss(this IEnumerable<IBattler> battlers) => battlers.Any(battler => battler.GetIsBoss());


        /// <summary> リストの内、誰かが死亡していれば true
        public static bool GetIsAnyDead(this IEnumerable<IBattler> battlers) => battlers.Any(battler => battler.State.Health.IsDead);
        /// <summary> リストの内、全員が死亡していれば true
        public static bool GetIsAllDead(this IEnumerable<IBattler> battlers) => battlers.All(battler => battler.State.Health.IsDead);
        /// <summary> スペック合計値取得 
        public static int GetSpec(this IEnumerable<IBattler> battlers) => battlers.Sum(battler => battler.State.Spec);

        /// <summary> パーティーのHPの割合の取得　MEMO: これのパーティー数で割ったものを相手のものと比較して高い方が勝利となる
        public static float GetHPRatio(this IEnumerable<IBattler> battlers)
        {
            var existBattlers = battlers.Where(battler => battler.Unit.IsExist);
            return existBattlers.Any()
                ? existBattlers.Sum(battler => battler.State.RatioHP) / existBattlers.Count()
                : 0f; // 0体なら0%とする
        }

        /// <summary>
        /// プレイヤーに変換
        /// </summary>
        public static IPlayer ToPlayer(this IBattler battler)
        {
            try { return (IPlayer)battler; }
            catch { throw new InvalidCastException($"{nameof(IPlayer)} を継承していません。"); }
        }

        /// <summary>
        /// 敵に変換
        /// </summary>
        public static IEnemy ToEnemy(this IBattler battler)
        {
            try { return (IEnemy)battler; }
            catch { throw new InvalidCastException($"{nameof(IEnemy)} を継承していません。"); }
        }

        /// <summary>
        /// ボスに変換
        /// </summary>
        public static IBoss ToBoss(this IBattler battler)
        {
            try { return (IBoss)battler; }
            catch { throw new InvalidCastException($"{nameof(IBoss)} を継承していません。"); }
        }

        /// <summary>
        /// 要素が1つしかない列挙型を単体に変換する
        /// </summary>
        public static IBattler ToSingle(this IEnumerable<IBattler>battlers)
        {
            try { return battlers.Single(); }
            catch { throw new InvalidCastException($"[関数 : {nameof(ToSingle)}] は要素の数が１つである必要があります。[Count : {battlers.Count()}]"); }
        }

        /// <summary>
        /// IBattler を IEnumerable<IBattler> に変換する
        /// </summary>
        public static IEnumerable<IBattler>ToEnumerable(this IBattler battler)
        {
            return new IBattler[] { battler };
        }

        /// <summary>
        /// パーティーデータを受け取り対象の Battler データに入れ込み、返す
        /// </summary>
        /// <param name="battlers">データを入れ込む対象 Butler の配列</param>
        /// <param name="teamData">入れ込みたいデータのパーティーデータ</param>
        public static IBattler[] SetUnitData(this IBattler[] battlers,TeamData teamData,bool isTakeover=false)
        {
            int i = 0;
            foreach(var unitData in teamData.UnitList)
            {
                var battler = battlers[i];
                if (i == 0)// 先頭をリーダー指定にする
                {
                    battler.SetUp(unitData, isTakeover, true);
                }
                else {
                    battler.SetUp(unitData, isTakeover,false);
                }
                battler.AdventSkill.AddLottery();
                i++;
            }
            return battlers;
        }

        /// <summary>
        /// Battler をまとめてリロード
        /// </summary>
        public static void Reload(this IEnumerable<IBattler>battlers)
        {
            foreach(var battler in battlers)battler.Reload();
        }

        /// <summary>
        /// Battler をまとめてリセット
        /// </summary>
        /// <param name="battlers"></param>
        public static void Reset(this IEnumerable<IBattler>battlers)
        {
            foreach (var battler in battlers) battler.Reset();
        }

        /// <summary>
        /// Battler をまとめて破棄
        /// </summary>
        public static void Dispose(this IEnumerable<IBattler>battlers)
        {
            foreach (var battler in battlers) battler.Dispose();
        }

        //---------------------------------------------------------------------------------------------------------
        // Utils
        //---------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Battler に関する情報を JSONで取得する
        /// </summary>
        private static string GetBattlerLog(IEnumerable<IBattler>battlers)
        {
            if (battlers.Count() == 0)
                return "要素が存在しません";
            else 
                return JsonHelper.ToJson(battlers);
        }
    }
}