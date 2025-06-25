using Cysharp.Threading.Tasks;
using fantec.Common;
using fantec.Master;
using fantec.Menu.Manager;
using UniRx;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace fantec.Menu.PartySelect.View
{
    public class EnemyInfoCell : MonoBehaviour
    {
        [SerializeField]
        private Text m_WaveText;

        [SerializeField]
        private GameObject[] m_EnemyInfoCells;

        [SerializeField]
        private Image[] m_CharacterImages;

        [SerializeField]
        private Button[] m_DetailButton;

        public async UniTask Setup(Data data, CancellationToken cts)
        {
            m_WaveText.text = data.labelText;

            // 全て非表示にする
            for(int i=0;i<m_EnemyInfoCells.Length;i++)
            {
                m_EnemyInfoCells[i].SetActive(false);
            }

            AbstructCardData enemyData = null;

            // データ反映
            for(int i=0;i<data.enemyInfoList.Count;i++)
            {
                enemyData = null;
                //通常の敵
                if (MasterDataManager.Instance.EnemyCardMaster.IsExist(data.enemyInfoList[i]))
                {
                    enemyData = MasterDataManager.Instance.EnemyCardMaster.GetData(data.enemyInfoList[i]);

                    
                }
                // ボス作る？ TODO:まだ未定

                if(enemyData!=null)
                {
                    // アイコン
                    m_CharacterImages[i].sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>(AssetPath.GetCharacterSpriteSpherePath(enemyData.originId));

                    // 属性アイコンなど表示させるならここに追加する　後々追加する？ TODO

                    int enemyInfo = data.enemyInfoList[i];
                    m_DetailButton[i].OnClickAsObservable().Subscribe(_=>OnTapDetailButton(enemyInfo)).AddTo(this);
                    m_EnemyInfoCells[i].SetActive(true);
                }
            }
        }

        /// <summary>
        /// 詳細ボタン押下時
        /// </summary>
        private void OnTapDetailButton(int enemyId)
        {
            // SE再生させる
            MenuManager.Instance.EnemyDetailId = enemyId;

            MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.NoContents);
        }

        public struct Data
        {
            public Data(string text,List<int>enemyInfoList)
            {
                labelText = text;
                this.enemyInfoList = enemyInfoList;
            }

            public readonly string labelText;
            public readonly List<int> enemyInfoList;
        }
    }
}