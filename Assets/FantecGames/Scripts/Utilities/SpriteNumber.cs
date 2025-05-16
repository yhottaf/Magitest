using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace fantec.Utilities
{
    public class SpriteNumber : MonoBehaviour
    {
        [Header("参照")]
        [SerializeField]private Sprite[] m_NumberSprites=new Sprite[10];
        [SerializeField] private GameObject m_NumberOriginalObject;

        [Header("設定")]
        [SerializeField] private int m_LimitValue = 999999;
        [SerializeField] private float m_Size = 1;
        [SerializeField] private float m_Spacing = 1;

        [Header("状態")]
        [SerializeField] private int m_Value;

        private List<SpriteRenderer> m_NumberRendererList;

        // 値
        private int Value => m_Value;

        // 桁数
        public int LimitDigit => GetDigit(m_LimitValue);

        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// アルファ値設定
        /// </summary>
        /// <param name="value"></param>
        public void SetAlpha(float value)
        {
            foreach(var renderer in m_NumberRendererList)
            {
                renderer.color = new Color(
                    renderer.color.r,
                    renderer.color.g,
                    renderer.color.b,value);
            }
        }

        /// <summary>
        /// サイズ設定
        /// </summary>
        public void SetScale(float scale)
        {
            m_Size = scale;
        }

        /// <summary>
        /// 数値画像を表示
        /// </summary>
        /// <param name="value">数値</param>
        /// <param name="ratio">最上位親オブジェクトの倍率</param>
        public void Show(int value)
        {
            m_Value = value;

            // 初期化
            Init();

            // 桁数取得
            var digit = GetDigit(value);

            // 桁数制限
            if(digit>LimitDigit)
            {
                value = m_LimitValue;
                digit = LimitDigit;
            }

            for(int i=0;i<digit;i++)
            {
                var numRenderer = m_NumberRendererList[i];

                // 画像を表示
                numRenderer.gameObject.SetActive(true);

                // 画像サイズ設定
                numRenderer.transform.localScale = Vector3.one * m_Size;

                // 中心へ合わせる
                var width = numRenderer.bounds.size.x;

                // 位置調整
                var adjustX = ((width) * i - (width * digit) / 2) + width / 2;

                // 拡大率に対する実際のサイズとの比率
                var ratio=numRenderer.transform.lossyScale.magnitude/numRenderer.transform.localScale.magnitude;

                // ターゲット位置取得
                var position=new Vector3(
                    numRenderer.transform.localPosition.x-(adjustX*m_Spacing)/ratio,
                    numRenderer.transform.localPosition.y,
                    numRenderer.transform.localPosition.z);

                // 位置設定
                numRenderer.transform.localPosition= position;

                // 現在チェック中の数値を取得
                var digitNum = GetPointDigit(value,i+1);

                // 数値に合わせた画像に差し替え
                numRenderer.sprite = GetNumberSprite(digitNum);
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            // 未初期化の場合
            if(m_NumberRendererList==null)
            {
                m_NumberRendererList = new List<SpriteRenderer>();

                // 複製用の為隠す
                m_NumberOriginalObject.gameObject.SetActive(false);

                for(int i=0;i<LimitDigit;i++)
                {
                    var numRenderer = Instantiate(m_NumberOriginalObject).GetComponent<SpriteRenderer>();

                    numRenderer.gameObject.SetActive(true);           　// 表示
                    numRenderer.transform.parent = this.transform;  　  // 子として登録
                    numRenderer.transform.localPosition = Vector3.zero; // 位置設定

                    // 数値オブジェクトリストに登録
                    m_NumberRendererList.Add(numRenderer);
                }
            }
            else // 初期化されていれば
            {
                // 全て
                foreach(var numObj in m_NumberRendererList)
                {
                    numObj.transform.localPosition=Vector3.zero;       // 位置初期化
                    numObj.gameObject.SetActive(false);                // 隠す
                }
            }
        }

        /// <summary>
        /// 整数の桁数を返す
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private int GetDigit(int num)
        {
            if (num == 0) return 1;
            return (num == 0) ? 1 : ((int)Mathf.Log10(num) + 1);
        }

        /// <summary>
        /// 整数の中から指定した桁数の値を返す
        /// </summary>
        /// <param name="num"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        private int GetPointDigit(int num,int digit)
        {
            int res = 0;
            int powDigit=(int)Mathf.Pow(10, digit);
            if(digit==1)
            {
                res = num - (num / powDigit) * powDigit;
            }
            else
            {
                res = (num - (num / powDigit) * powDigit) / (int)Mathf.Pow(10, (digit - 1));
            }

            return res;
        }

        /// <summary>
        /// 一桁分の数値画像を取得
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private Sprite GetNumberSprite(int number)
        {
            try { return m_NumberSprites[number]; }
            catch (System.IndexOutOfRangeException) { throw new System.IndexOutOfRangeException($"[number : {number}]"); }
        }
    }
}