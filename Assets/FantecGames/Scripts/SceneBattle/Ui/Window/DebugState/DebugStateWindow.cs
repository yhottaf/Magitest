using fantec.Battle.Manager;
using fantec.Battle.Model;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace fantec.Battle
{
    public interface IDebugStateWindow:IWindow<IDebugStateWindow>
    {

    }
}

namespace fantec.Battle.Ui.Window
{
    public class DebugStateWindow : WindowBase,IDebugStateWindow
    {
        public CompositeDisposable ClosedDisposable { get;private set; }=new CompositeDisposable();

        [SerializeField] private DebugStateTextView m_TextViewOriginal;

        private Dictionary<int,DebugStateTextView>m_PlayerTextViewDict=new Dictionary<int,DebugStateTextView>();
        private Dictionary<int,DebugStateTextView>m_EnemyTextViewDict=new Dictionary<int,DebugStateTextView>();

        public IDebugStateWindow OnCreate()
        {
            m_TextViewOriginal.gameObject.SetActive(false);

            SetStateText(Locator.Resolve<IBattleModelUnits>().PlayerDatas.GetExistBattlers());
            SetStateText(Locator.Resolve<IBattleModelUnits>().EnemyDatas.GetExistBattlers());

            Locator.Resolve<IBattleModelUnits>().OnUpdatePlayerMemberObservable
                .Subscribe(SetStateText).AddTo(ClosedDisposable);
            Locator.Resolve<IBattleModelUnits>().OnUpdateEnemyMemberObservable
                .Subscribe(SetStateText).AddTo(ClosedDisposable);

            return this;
        }

        public void Close()
        {
            ClosedDisposable.Dispose();
            Destroy(this.gameObject);
        }

        private void SetStateText(IEnumerable<IBattler>battlers)
        {
            if (battlers.GetIsPlayer())
                foreach (var text in m_PlayerTextViewDict.Values) text.gameObject.SetActive(false);
            else
                if (battlers.GetIsEnemy())
                foreach (var text in m_EnemyTextViewDict.Values) text.gameObject.SetActive(false);

            foreach (var battler in battlers)
            {
                var text = GetTextView(battler);
                text.gameObject.SetActive(true);
                text.SetStateText(battler);

                battler.State.OnUpdateTokenCellList.Subscribe(_ => text.SetStateText(battler)).AddTo(ClosedDisposable);
                battler.State.Health.OnHealthChangeObservable.Subscribe(_ => text.SetStateText(battler)).AddTo(ClosedDisposable);
                battler.Transform.OnMoveCompletedObservable.Subscribe(_ =>
                     {
                         text.SetStateText(battler);

                         // battler の位置を取得し、スクリーン座標に変換して表示位置に反映
                         var offset = new Vector3(0, -0.5f, 0); // 少し下方向に表示（好みに応じて調整）
                         Vector3 worldPosition = battler.GetCenterPosition() + offset;
                         Vector2 screenPos = Locator.Resolve<IBattleCanvasManager>().GetWorldToScreenPointForOver(worldPosition);
                         text.GetComponent<RectTransform>().anchoredPosition = screenPos;
                     }).AddTo(ClosedDisposable);
                battler.State.Health.OndeadObservable.Subscribe(_=>
                {
                    // テキスト非表示＋破棄
                    if (text != null)
                    {
                        text.gameObject.SetActive(false);
                    }

                    // Dictionary から削除（あれば）
                    int posIndex = battler.Unit.Entity.positionIndex;
                    if (battler.GetIsPlayer())
                    {
                        m_PlayerTextViewDict.Remove(posIndex);
                    }
                    else
                    {
                        m_EnemyTextViewDict.Remove(posIndex);
                    }
                }).AddTo(ClosedDisposable);
            }
        }

        private DebugStateTextView GetTextView(IBattler battler)
        {
            var positionIndex = battler.Unit.Entity.positionIndex;

            if(battler.GetIsEnemy())
            {
                if (m_EnemyTextViewDict.ContainsKey(positionIndex))
                    return m_EnemyTextViewDict[positionIndex];
                else
                    return CreateTextView(battler);
            }
            else
            {
                if(m_PlayerTextViewDict.ContainsKey(positionIndex))
                    return m_PlayerTextViewDict[positionIndex];
                else
                    return CreateTextView(battler);
            }
        }

        private DebugStateTextView CreateTextView(IBattler battler)
        {
            var text = Instantiate(m_TextViewOriginal);
            text.transform.SetParent(m_TextViewOriginal.transform.parent);
            text.transform.localScale = Vector3.one;
            var offset = Vector3.zero;
            offset *= battler.GetIsEnemy() ? -1 : 1;
            text.GetComponent<RectTransform>().anchoredPosition = Locator.Resolve<IBattleCanvasManager>().GetWorldToScreenPointForOver(battler.GetCenterPosition() + offset);

            if (battler.GetIsEnemy())
            {
                m_EnemyTextViewDict.Add(battler.Unit.Entity.positionIndex, text);
            }
            else
            {
                m_PlayerTextViewDict.Add(battler.Unit.Entity.positionIndex, text);
            }

            return text;
        }
    }
}