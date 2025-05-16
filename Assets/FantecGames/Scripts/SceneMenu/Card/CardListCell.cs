using UnityEngine;
using UniRx;
using fantec.Common;
using FantecScrollView;
using Fantec.Menu;
using Cysharp.Threading.Tasks;
using fantec.Menu.Manager;

namespace fantec.Menu.Card
{
    class CardListCell : FantecGridViewCell<CardData, CardListContext>
    {
        [SerializeField]
        private CardCellController m_View;

        [SerializeField]
        private GameObject m_OrganizationObject;

        [SerializeField]
        private GameObject m_NewMarkObject;

     //  [SerializeField] お気に入りカードを設定するなら
    //    private GameObject m_FavoriteObject; 

        private bool isPartyInclude;   //このカードが編成中か判定

        public override void Initialize()
        {
            m_View.OnClickButtonObservable.Subscribe(OnClickButton).AddTo(this);
            m_View.OnLongClickButtonObservable.Subscribe(OnLongTapButton).AddTo(this);
        }

        public override void UpdateContent(CardData itemData)
        {
            bool selected = Context.SelectedIndex == Index;
            UniTask.Void(async() =>
            {
                await m_View.UpdateView(itemData.cardId);
                m_View.UpdateCardInfo(itemData.cardId);

                // TODO：お気に入りカードの設定をするならここに記載
                //if (LocalDataManager.Instance.LocalData.IsFavorite(itemData.cardId))
                //{
                //    m_FavoriteObject.SetActive(true);
                //}
                //else
                //{
                //    m_FavoriteObject.SetActive(false);
                //}

                // 編成できないキャラをまとめたIDリストを作成
                isPartyInclude = MenuManager.Instance.CardOrgamization(itemData);

                if (isPartyInclude)
                {
                    // 編成中であるならグレーで表示
                    m_View.SetGrayColor();
                }
                else
                {
                    // 編成していないのであれば通常カラーで表示
                    m_View.SetNormalColor();
                }

                // 編成中の表示
                m_OrganizationObject.SetActive(isPartyInclude);

                // NEWマークの表示
                m_NewMarkObject.SetActive(LocalDataManager.Instance.LocalData.IsNew(itemData.cardId));
                LocalDataManager.Instance.LocalData.AddCheckCard(itemData.cardId);
            });
        }

        protected override void UpdatePosition(float normalizedPosition, float localPosition)
        {
            base.UpdatePosition(normalizedPosition, localPosition);
        }

        /// <summary>
        /// ボタン押下時
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickButton(Unit unit)
        {
            Context.OnCellClicked?.Invoke(Index);
        }

        /// <summary>
        /// ボタンを長押しした時 (1秒間以上)
        /// </summary>
        /// <param name="unit"></param>
        private void OnLongTapButton(Unit unit)
        {
            Context.OnCellLongTaped?.Invoke(Index);
        }
    }
}
