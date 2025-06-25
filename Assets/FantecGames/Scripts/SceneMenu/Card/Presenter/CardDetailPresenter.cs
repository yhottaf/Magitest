using Cysharp.Threading.Tasks;
using fantec.Menu.Card.View;
using fantec.Menu.Manager;
using fantec.PlayFabClient;
using UniRx;
using UnityEngine;

namespace fantec.Menu.Card.Presenter
{
    public class CardDetailPresenter : MonoBehaviour
    {
        [Header("View")]
        [SerializeField]
        private CardDetailView m_View; // ビュー

        [SerializeField]
        private CardDetailContentView m_ContentView;

        private void Start()
        {
            // 所持Magi情報をリストに保存
            MenuManager.Instance.m_UserDataCardList = CardManager.CardDatas;

            // 閉じるボタンが押されたときの関数呼び出しを登録
            m_View.OnClickCloseButtonObservable.Subscribe(OnClickCloseButton).AddTo(this);

            // 画面情報の更新
            UpdateView();
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        private void UpdateView()
        {
            int cardId = MenuManager.Instance.SelectCardId;
            m_ContentView.Setup(cardId);
        }

        /// <summary>
        /// 閉じるボタンが押されたとき
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickCloseButton(Unit unit)
        {
            // SE再生


            // カード詳細画面を閉じる
            MenuWindowManager.Instance.Remove(MenuWindowManager.CreateType.CardDetail);
        }
    }
}