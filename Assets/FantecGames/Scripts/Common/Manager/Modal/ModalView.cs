using System;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Common
{
    public class ModalView:OverlayObject
    {
        [SerializeField] private Transform m_TitleChunk;
        [SerializeField] private Text m_TitleText;
        [SerializeField] private Transform m_ContentChunk;
        [SerializeField] private Text m_ContentText;
        [SerializeField] private Transform m_ButtonsChunk;
        [SerializeField] private Button m_ButtonOriginal;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private Image m_BackImage;

        private void Awake()
        {
            m_TitleChunk.gameObject.SetActive(false);
            m_ContentChunk.gameObject.SetActive(false);
            m_ButtonsChunk.gameObject.SetActive(false);
            m_ButtonOriginal.gameObject.SetActive(false);
            m_CloseButton.gameObject.SetActive(false);
            m_BackImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// タイトルテキストを表示する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public ModalView SetTitleText(string text)
        {
            m_TitleChunk.gameObject.SetActive(true);
            m_TitleText.text = text;
            return this;
        }

        /// <summary>
        /// テキストを表示する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public ModalView SetContextText(string text)
        {
            m_ContentChunk.gameObject.SetActive(true);
            m_ContentText.text = text;
            return this;
        }

        /// <summary>
        /// 作成されたボタンクリック時に登録したイベントを実行する
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="onClick"></param>
        /// <returns></returns>
        public ModalView AddButtonAction(string buttonName,Action onClick)
        {
            m_ButtonsChunk.gameObject.SetActive(true);
            var button = this.CreateButton(buttonName);
            button.onClick.AddListener(onClick.Invoke);
            return this;
        }

        /// <summary>
        /// ボタンクリックと同時にモーダルを削除できる。イベント追加も可能
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="onClick"></param>
        /// <returns></returns>
        public ModalView AddButtonActionAndClose(string buttonName,Action onClick=null)
        {
            m_ButtonsChunk.gameObject.SetActive(true);
            var clone=this.CreateButton(buttonName);
            clone.onClick.AddListener(Close);
            if(onClick!=null)clone.onClick.AddListener(onClick.Invoke);
            return this;
        }

        /// <summary>
        /// 右上の閉じるボタンを有効化。イベント追加も可能
        /// </summary>
        /// <param name="onClick"></param>
        /// <returns></returns>
        public ModalView SetRightTopButton(Action onClick=null)
        {
            m_CloseButton.gameObject.SetActive(true);
            m_CloseButton.onClick.AddListener(Close);
            if(onClick!=null)m_CloseButton.onClick.AddListener(onClick.Invoke);
            return this;
        }

        /// <summary>
        /// 背景を有効化
        /// </summary>
        /// <returns></returns>
        public ModalView SetBackground()
        {
            m_BackImage.gameObject.SetActive(true);
            return this;
        }

        /// <summary>
        /// モーダルを閉じる(破棄)
        /// </summary>
        public void Close()
        {
            Destroy(this.gameObject);
        }

        private Button CreateButton(string buttonName)
        {
            var clone = Instantiate(m_ButtonOriginal);
            clone.transform.SetParent(m_ButtonOriginal.transform.parent);
            clone.transform.localScale = Vector3.one;
            clone.gameObject.SetActive(true);
            clone.GetComponentInChildren<Text>().text = buttonName;
            return clone;
        }
    }
}