using fantec.Common;
using fantec.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace fantec.Menu.Manager
{
    public class MenuWindowManager:Singleton<MenuWindowManager>
    {
        [SerializeField] private GameObject m_Header;
        [SerializeField] private GameObject m_Footer;
        [SerializeField] private Transform m_ContentParent;

        //Overlayでのウィンドウ
        [SerializeField] private OverlayObject m_Home;
        [SerializeField] private OverlayObject m_Gacha;
        [SerializeField] private OverlayObject m_PartySelect;
        [SerializeField] private OverlayObject m_MenuModal;
        [SerializeField] private OverlayObject m_Profile;
        [SerializeField] private OverlayObject m_Notice;

        //モーダル
        [SerializeField] private OverlayObject m_NameChangeModal;
        [SerializeField] private OverlayObject m_PlayerNameEditModal;
        [SerializeField] private OverlayObject m_EnemyDetailModal;
        [SerializeField] private OverlayObject m_ItemDetailModal;
        [SerializeField] private OverlayObject m_StaminaItemDetailModal;
        [SerializeField] private OverlayObject m_NoContentsModal;

        private Dictionary<CreateType,OverlayObject>m_OverlayObjectDictionary= new Dictionary<CreateType,OverlayObject>(); 

        public enum CreateType
        {
            Home,
            Gacha,
            Profile,
            Menu,
            PlayerNameEdit,
            PartySelect,
            EnemyDetail,
            ItemDetail,
            StaminaItemDetail,
            NameChange,
            Notice,
            NoContents,//未実装モーダル
        }

        public CreateType NowWindow { get; private set; } = CreateType.Home;
        public CreateType PrevWindow { get; private set; } = CreateType.Home;

        private readonly int BackSiblingIndex = 0;
        private readonly int CenterSiblingIndex = 1;
        private readonly int FrontSiblingIndex = 2;

        protected override void OnDestroy()
        {
            Clear();
            base.OnDestroy();
        }

        /// <summary>
        /// 画面を作成
        /// </summary>
        /// <param name="type">作成したいプレゼンターの種別</param>
        /// <returns>作成できたか否か</returns>
        public bool Create(CreateType type)
        {
            //同じ画面を複数表示することはさせない
            if(m_OverlayObjectDictionary.ContainsKey(type))
            {
                return false;
            }

            OverlayObject CreateWindow(OverlayObject overlayObject)
            {
                PrevWindow = NowWindow;
                NowWindow = type;

                OverlayObject entity=Instantiate(overlayObject,m_ContentParent);
                m_OverlayObjectDictionary.Add(type, entity);

                return entity;
            }
            OverlayObject CreateModal(OverlayObject overlayObject)
            {
                OverlayObject entity = Instantiate(overlayObject, m_ContentParent);
                OverlayCanvasManager.Instance.Add(entity);
                m_OverlayObjectDictionary.Add(type, entity);

                return entity;
            }

            switch(type)
            {
                //-----------------------------------------------------------------------
                // 画面系
                //-----------------------------------------------------------------------
                //ホーム
                case CreateType.Home:
                    IndicateAll();
                    CreateWindow(m_Home);
                    return true;

                    //ガチャ
                case CreateType.Gacha:
                    CreateWindow(m_Gacha);
                    return true;

                    // 出撃パーティ選択
                case CreateType.PartySelect:
                    IndicateContent();
                    CreateWindow(m_PartySelect);
                    return true;

                    //プロフィール
                case CreateType.Profile:
                    CreateWindow(m_Profile);
                    return true;

                    // お知らせ
                case CreateType.Notice:
                    IndicateContent();
                    CreateWindow(m_Notice);
                    return true;

                    //--------------------------------------------------------------------
                    // モーダル系
                    //--------------------------------------------------------------------

                    //メニュー
                case CreateType.Menu:
                    CreateModal(m_MenuModal);
                    return true;

                    // ディレクトリのパーティ名変更
                case CreateType.NameChange:
                    CreateModal(m_NameChangeModal);
                    return true;

                    //プレイヤー名入力
                case CreateType.PlayerNameEdit:
                    CreateModal(m_PlayerNameEditModal);
                    return true;

                    // 敵の詳細情報
                case CreateType.EnemyDetail:
                    CreateModal(m_NoContentsModal);
                    return true;

                    // アイテムの詳細情報
                case CreateType.ItemDetail:
                    CreateModal(m_NoContentsModal);
                    return true;

                    //未実装モーダル
                case CreateType.NoContents:
                    CreateModal(m_NoContentsModal);
                    return true;

                default:
                    Debug.LogError("存在しないタイプが呼ばれました： " + type);
                    return false;
            }
        }

        /// <summary>
        /// 画面を破棄 (ウィンドウを破棄するときはこれを必ず呼ぶこと)
        /// </summary>
        /// <param name="type">破棄したい画面の種別</param>
        /// <returns>破棄できたか否か</returns>
        public bool Remove(CreateType type)
        {
            // 存在しない画面は破棄できない
            if (!m_OverlayObjectDictionary.ContainsKey(type)) return false;

            // 画面の破棄
            Destroy(m_OverlayObjectDictionary[type].gameObject);

            // Dictionary から取り除く
            m_OverlayObjectDictionary.Remove(type);

            // 破棄成功
            return true;
        }

        /// <summary>
        /// 画面を全て破棄
        /// </summary>
        public void RemoveAll()
        {
            // 画面の破棄
            foreach(var createType in m_OverlayObjectDictionary.Keys)
            {
                Destroy(m_OverlayObjectDictionary[createType].gameObject);
            }
            m_OverlayObjectDictionary.Clear();
        }

        /// <summary>
        /// 表示中の画面をすべて取り除く
        /// </summary>
        public void Clear()
        {
            foreach(var overlayDic in m_OverlayObjectDictionary)
            {
                // 中身が存在していれば
                if(overlayDic.Value)
                {
                    Destroy(overlayDic.Value.gameObject);
                }
            }

            // 中身も初期化
            m_OverlayObjectDictionary = new Dictionary<CreateType, OverlayObject>();
        }

        /// <summary>
        /// ヘッダー&フッター&コンテンツの順番を遷移先によって整理する
        /// </summary>
        /// <param name="footerType"></param>
        public void OrganizeSibling(FooterType footerType)
        {
            switch (footerType)
            {
                case FooterType.NONE:
                    break;
                case FooterType.Home:
                    IndicateAll();
                    break;
                case FooterType.Quest:
                    IndicateContent();
                    break;
            }
        }

        /// <summary>
        /// ヘッダー&フッター / コンテンツ の順番で表示する
        /// </summary>
        private void IndicateAll()
        {
            m_ContentParent.transform.SetSiblingIndex(BackSiblingIndex);
            m_Header.transform.SetSiblingIndex(CenterSiblingIndex);
            m_Footer.transform.SetSiblingIndex(FrontSiblingIndex);
        }

        /// <summary>
        /// コンテンツ / ヘッダー&フッター の順番で表示する
        /// </summary>
        private void IndicateContent()
        {
            m_Footer.transform.SetSiblingIndex(BackSiblingIndex);
            m_Header.transform.SetSiblingIndex(CenterSiblingIndex);
            m_ContentParent.transform.SetSiblingIndex(FrontSiblingIndex);
        }


        /// <summary>
        /// ヘッダー / コンテンツ / フッター の順番で表示する
        /// </summary>
        private void IndicateContentAndHeader()
        {
            m_Footer.transform.SetSiblingIndex(BackSiblingIndex);
            m_ContentParent.SetSiblingIndex(CenterSiblingIndex);
            m_Header.transform.SetSiblingIndex(FrontSiblingIndex);
        }

        /// <summary>
        /// フッター / コンテンツ / ヘッダー の順番で表示する
        /// </summary>
        private void IndicateContentAndFooter()
        {
            m_Header.transform.SetSiblingIndex(BackSiblingIndex);
            m_ContentParent.SetSiblingIndex(CenterSiblingIndex);
            m_Footer.transform.SetSiblingIndex(FrontSiblingIndex);
        }


        /// <summary>
        /// フッターを最前面にする
        /// </summary>
        public void FotterSetAsLastSibiling()
        {
            m_Footer.transform.SetAsLastSibling();
        }

        /// <summary>
        /// フッターを最後面にする
        /// </summary>
        public void FotterSetAsFirstSibling()
        {
            m_Footer.transform.SetAsFirstSibling();
        }
    }
}