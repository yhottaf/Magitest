using Cysharp.Threading.Tasks;
using fantec.Menu.Manager;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

namespace fantec.ModalDialog.NoContents
{
    public class NoContentsModal : MonoBehaviour
    {
        private ObservablePointerDownTrigger EventTrigger;
        void Start()
        {
            // イベントトリガー用のオブサーバークラスを取得(EventTriggerのUniRX版)
            EventTrigger= GetComponent<ObservablePointerDownTrigger>();

            // クリック時のイベント追加
            EventTrigger.OnPointerDownAsObservable().
                Subscribe(OnPointerDown).AddTo(this);
        }

        /// <summary>
        /// 画面押下時
        /// </summary>
        /// <param name="obj"></param>
        private void OnPointerDown(PointerEventData obj)
        {
            MenuWindowManager.Instance.Remove(MenuWindowManager.CreateType.NoContents);
        }
    }
}