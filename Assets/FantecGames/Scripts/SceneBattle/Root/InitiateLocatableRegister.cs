using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace fantec.Battle.Root
{
    // オブジェクトの登録処理に使用する
    // ゲーム開始時や、シーン遷移時の初期化処理
    // ・例えば、キャラクター、アイテム、UIなどのオブジェクトを自動的に登録する
    // イベントリスナー、管理リストへの登録、シングルトン管理への追加など

    // 2. バトルシステムの初期化にも使用する
    // IRegistable を戦闘参加キャラクターに実装し、キャラクターをバトルマネージャーへ登録
    // 例 : 敵や味方のキャラクターをリストアップし、ターン管理システムへ登録

    // 3. オンラインゲームのネットワーク同期など
    // IRegistable をネットワークオブジェクトに実装し、サーバーへ登録
    // 例：Photon や Mirrorのようなネットワークライブラリを使用する場合など


    public class InitiateLocatableRegister : MonoBehaviour, IRootInitiater
    {
        [SerializeField] GameObject[] m_SearchObjects;
        public async UniTask InitializeAsync(CancellationTokenSource cts)
        {
            foreach(var searchObject in m_SearchObjects)
            {
                foreach(var locatable in searchObject.GetComponentsInChildren<IRegistable>())
                {
                    locatable.Register();
                }
            }

            // 処理負荷を分散したり、他の非同期処理と適切に同期させる
            await UniTask.DelayFrame(1); // 1フレーム待つ
        }
    }
}