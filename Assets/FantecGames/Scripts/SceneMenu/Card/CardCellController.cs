using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using fantec.Common;
using fantec.Menu.Manager;
using fantec.PlayFabClient;
using fantec;
using UnityEngine.Timeline;
using Cysharp.Threading.Tasks;

namespace Fantec.Menu
{
    public class CardCellController : MonoBehaviour
    {
        /// <summary>
        /// カード画像
        /// </summary>
        [SerializeField]
        private Image m_CharacterImage;

        /// <summary>
        /// レアリティフレーム画像
        /// </summary>
        [SerializeField]
        private Image m_RarityFrameImage;

        /// <summary>
        /// レアリティ背景画像
        /// </summary>
        //[SerializeField]
        //private Image m_RarityBgImage;

        ///// <summary>
        ///// ソート時のレアリティ下地画像
        ///// </summary>
        //[SerializeField]
        //private Image m_RaritySortBaseImage;


        //[SerializeField]
        //private GameObject[] m_BreakLimitOnImage = new GameObject[BreakLimitMax];

        [SerializeField]
        private Button m_Button;

        //[SerializeField]
        //private Text m_InfoText;

        //private const int BreakLimitMax = 6;

        public IObservable<Unit> OnClickButtonObservable => m_Button.OnClickAsObservable();
        public IObservable<Unit> OnLongClickButtonObservable => m_Button.OnLongTapAsObservable(1f);

        /// <summary>
        /// 見た目を更新する
        /// </summary>
        /// <param name="cardId">カードID</param>
        /// <param name="isInit">初期値を表示するか</param>
        public async UniTask UpdateView(int cardId, bool isInit = false)
        {
            SetNormalColor();

            // キャラクター画像
            if (MasterDataManager.Instance.PlayerCardMaster.IsExist(cardId))
            {
                int originId = MasterDataManager.Instance.PlayerCardMaster.GetData(cardId).originId;
                m_CharacterImage.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>($"{AssetPath.GetCharacterSpriteCapsulePath(originId)}");
            }

            CardData cardData = CardManager.GetCardData(cardId);
            if (cardData != null)
            {
                CardRarityType cardRarityType = isInit ? cardData.CardMasterData().rarityType : cardData.rarityType;
                // レアリティ背景
         //       m_RarityBgImage.sprite = AssetManager.Instance.LoadAsset<Sprite>(AssetPath.GetSpriteRarityBackgroundPath((int)cardRarityType + 1));
                // レアリティフレーム
                m_RarityFrameImage.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>(AssetPath.GetSpriteRarityFramePath((int)cardRarityType + 1));
                // レアリティデコレーション
         //       m_RaritySortBaseImage.sprite = AssetManager.Instance.LoadAsset<Sprite>(AssetPath.GetSpriteRaritySortBasePath((int)cardRarityType + 1));

                // クラスアイコン
          //      m_ClassIconImage.sprite = AssetManager.Instance.LoadAsset<Sprite>(AssetPath.GetSpriteClassIconPath(cardData.CardMasterData().classType));

            }
            else
            {
                fantec.Master.PlayerCardData masterData = MasterDataManager.Instance.PlayerCardMaster.GetData(cardId);
                CardRarityType cardRarityType = masterData.rarityType;
          //      m_RarityBgImage.sprite = AssetManager.Instance.LoadAsset<Sprite>(AssetPath.GetSpriteRarityBackgroundPath((int)cardRarityType + 1));
                // レアリティフレーム
                m_RarityFrameImage.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>(AssetPath.GetSpriteRarityFramePath((int)cardRarityType + 1));
                // レアリティデコレーション
             //   m_RaritySortBaseImage.sprite = AssetManager.Instance.LoadAsset<Sprite>(AssetPath.GetSpriteRaritySortBasePath((int)cardRarityType + 1));

                // クラスアイコン
            //    m_ClassIconImage.sprite = AssetManager.Instance.LoadAsset<Sprite>(AssetPath.GetSpriteClassIconPath(masterData.classType));
            }
        }

        /// <summary>
        /// ソート時に使用するカード下部の情報
        /// </summary>
        /// <param name="cardId"></param>
        public void UpdateCardInfo(int cardId)
        {
            CardData cardData = CardManager.GetCardData(cardId);
            if (cardData != null)
            {

                //switch (MenuManager.Instance.m_CardSortType)
                //{
                //    case CardSortFilterModal.Presenter.CardSortFilterPresenter.CardSortingbutton.Rarity:
                //        m_InfoText.text = $"{(int)cardData.rarityType + 1}".ConvertToFullWidth();
                //        break;
                //    case CardSortFilterModal.Presenter.CardSortFilterPresenter.CardSortingbutton.Level:
                //        m_InfoText.text = $"{cardData.CardMasterData().GetLevelByExp(cardData.totalExp)}".ConvertToFullWidth();
                //        break;
                //    case CardSortFilterModal.Presenter.CardSortFilterPresenter.CardSortingbutton.SPEC:
                //        m_InfoText.text = $"{cardData.GetSpec()}".ConvertToFullWidth();
                //        break;
                //    case CardSortFilterModal.Presenter.CardSortFilterPresenter.CardSortingbutton.HP:
                //        m_InfoText.text = $"{cardData.GetHp()}".ConvertToFullWidth();
                //        break;
                //    case CardSortFilterModal.Presenter.CardSortFilterPresenter.CardSortingbutton.ATK:
                //        m_InfoText.text = $"{cardData.GetAtk()}".ConvertToFullWidth();
                //        break;
                //    case CardSortFilterModal.Presenter.CardSortFilterPresenter.CardSortingbutton.BRK:
                //        m_InfoText.text = $"{cardData.GetBrk()}".ConvertToFullWidth();
                //        break;
                //    case CardSortFilterModal.Presenter.CardSortFilterPresenter.CardSortingbutton.DEF:
                //        m_InfoText.text = $"{cardData.GetDef()}".ConvertToFullWidth();
                //        break;
                //    case CardSortFilterModal.Presenter.CardSortFilterPresenter.CardSortingbutton.SPD:
                //        m_InfoText.text = $"{cardData.GetSpd()}".ConvertToFullWidth();
                //        break;
                //    default:
                //        m_InfoText.text = $"{(int)cardData.rarityType + 1}".ConvertToFullWidth();
                //        break;
                //}
            }
        }

        /// <summary>
        /// 通常の表示にする
        /// </summary>
        public void SetNormalColor()
        {
            m_CharacterImage.color = Color.white;
        }

        /// <summary>
        /// グレーアウトさせる
        /// </summary>
        public void SetGrayColor()
        {
            m_CharacterImage.color = Color.gray;
        }
    }
}
