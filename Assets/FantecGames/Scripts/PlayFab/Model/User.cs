using Cysharp.Threading.Tasks;
using fantec.PlayfabCilent;
using System.Collections.Generic;

namespace fantec.PlayFabClient
{
    /// <summary>
    /// PlayFab の UserData として記録するユーザー情報
    /// </summary>
    public class User
    {
        public List<PartyData> PartyList { get; set; }
        public int ProfileCardId { get; set; }

        public List<int> ClearQuestIdList { get; set; }
        public Dictionary<TutorialId, bool> TutorialDictionary { get; set; }
        //プロフィール写真情報やパーティーリスト情報など持たせたい場合はここに記載


        /// <summary>
        /// 新規ユーザーデータを作成する(新規ユーザーデータ作成の際必ず作成すべき処理は全てここで管理する)
        /// </summary>
        /// <returns></returns>
        public static User Create()
        {
            var user = new User()
            {
                // パーティーデータの作成
                PartyList=new List<PartyData>() // とりあえずパーティー編成の枠を5つ作れるように用意
                {
                    new PartyData(new List<int>(Define.PARTY_CAPACITY)),
                    new PartyData(new List<int>(Define.PARTY_CAPACITY)),
                    new PartyData(new List<int>(Define.PARTY_CAPACITY)),
                    new PartyData(new List<int>(Define.PARTY_CAPACITY)),
                    new PartyData(new List<int>(Define.PARTY_CAPACITY)),
                },

                ClearQuestIdList = new List<int>(),

                TutorialDictionary = new Dictionary<TutorialId, bool>()
                {
                    {TutorialId.InitialPresent,false}, 
                }
            };

            // ※パーティにいれるための初期キャラ3体をあらかじめ設定しておく
            List<int> cardIds = new List<int>() {10001100,10001101,10001102 };

            // プロフィールカードの設定
            user.ProfileCardId = cardIds[0];

            // 初期パーティの設定
            for(int i=0;i<user.PartyList.Count;i++) // 編成枠全てを同じ編成で埋める
            {
                for(int k=0;k<cardIds.Count;k++)
                {
                    user.PartyList[i].MemberList.Add(cardIds[k]);
                }
            }

            return user;
        }

        /// <summary>
        /// チュートリアルフラグを更新する
        /// </summary>
        /// <param name="tutorialId"></param>
        /// <returns></returns>
        public async UniTask UpdateTutorialFlag(TutorialId tutorialId)
        {
            UserDataManager.User.TutorialDictionary[tutorialId] = true;
            await UserDataManager.UpdatePlayFab();
        }
    }
}