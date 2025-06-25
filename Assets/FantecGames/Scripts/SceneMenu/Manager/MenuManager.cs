using fantec.Common;
using UnityEngine;
using System;
using System.Collections.Generic;
using fantec.Utilities;
using Cysharp.Threading.Tasks;
using fantec.PlayFabClient;
using System.Linq;
using UniRx;
using PlayFab;
using PlayFab.ClientModels;
using fantec.Master;


namespace fantec.Menu.Manager
{
    public class MenuManager : Singleton<MenuManager>
    {
        /// <summary>
        /// パーティ編成で入れ替え前のID
        /// </summary>
        [NonSerialized]
        public int PartyEditCardId;

        /// <summary>
        /// カードリスト内で選択したID
        /// </summary>
        [NonSerialized]
        public int SelectCardId;

        /// <summary>
        /// カード入れ替え画面で選択したID
        /// </summary>
        [NonSerialized]
        public int CardChangeSelectId;

        /// <summary>
        /// ユーザーの所持カードを記憶
        /// </summary>
        [NonSerialized]
        public List<CardData> m_UserDataCardList;

        /// <summary>
        /// 選択したクエストカテゴリ
        /// </summary>
        [NonSerialized]
        public int SelectStageGroupId;

        /// <summary>
        /// 選択したクエストID
        /// </summary>
        [NonSerialized]
        public int SelectStageId= 1000001; // 仮のステージIDを設定

        //[NonSerialized] // スクロール形式のクエスト選択画面を作るなら使用する
        //public int CenterStageId;

        /// <summary>
        /// 閲覧するアイテムID
        /// </summary>
        [NonSerialized]
        public int ItemDetailId;

        /// <summary>
        /// 閲覧する敵情報
        /// int:敵ID int:レベル
        /// </summary>
        [NonSerialized]
        public int EnemyDetailId;

        /// <summary>
        /// クエスト選択系の画面から遷移してきたか
        /// </summary>
        [NonSerialized]
        public bool MoveQuestSelect = false;

        /// <summary>
        /// 編成状態に変更を加えたかどうか
        /// </summary>
        [NonSerialized]
        public bool ChangePartyData = false;

        /// <summary>
        /// Live2DのActive状態をコントロール 通知するタイミングで非アクティブなどにする
        /// </summary>
        [NonSerialized]
        public readonly Subject<Unit>onLive2DState=new Subject<Unit>();

        protected override void Awake()
        {
            base.Awake();

            Loading.Hide(0.5f);
        }

        /// <summary>
        /// 対象の所持カードがパーティに編成中であるかの判定
        /// </summary>
        /// <param name="CardId"></param>
        public bool CardOrgamization(CardData itemData)
        {
            // 全パーティに編成されているIDリスト
            List<int> partyIdList = UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex].MemberList;

            return partyIdList.Contains(itemData.cardId);
        }

        /// <summary>
        /// 編成データの中が何も配置されていない状態ならtrue,
        /// 1体でもMAGIが配置されているならfalseを返す
        /// </summary>
        /// <param name="partyData"></param>
        /// <returns></returns>
        public bool IsEmptyMember(PartyData partyData)
        {
            // 全てが 0 または -1 なら true、それ以外が含まれていれば false
            return partyData.MemberList.All(id => id == -1 || id == 0);
        }

        /// <summary>
        /// キャッシュに保存されているLive2Dデータを確認するための関数
        /// </summary>
        public void LoadedCheckLive2DData()
        {
            // 現在ロード中のアドレス一覧を確認(デバッグ)
            var loading = AssetManager.Instance.GetLoadedLive2DModels();
            foreach (var addr in loading)
            {
                Debug.Log($"現在キャッシュに残っているLive2Dデータ: {addr}");
            }
        }

        // UTCを日本時間にして文字列にして返す
        public string GetJSTScheduleTime(string now)
        {
            // UTCとして解析
            DateTime utcTime = DateTime.Parse(now, null, System.Globalization.DateTimeStyles.AdjustToUniversal); ;

            // 日本時間（JST = UTC+9）に変換
            DateTime jstTime = utcTime.AddHours(9);

            // 西暦下2桁を取得
            string year2Digit = (jstTime.Year % 100).ToString("D2");

            // フォーマットして出力
            string formatted = $"{year2Digit}/{jstTime.Month:D2}/{jstTime.Day:D2} {jstTime.Hour:D2}:{jstTime.Minute:D2}";

            // 25/05/20 12:11 の形式にして返す
            return formatted;
        }
    }
}