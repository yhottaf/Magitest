using Cysharp.Threading.Tasks;
using fantec.Common;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Debugger.BattleSetup
{
    public class DebugFotter : MonoBehaviour
    {
        [SerializeField] private Button m_PlayButton;

        private void Awake()
        {
            m_PlayButton.OnClickAsObservable().Subscribe(OnClickPlayButton).AddTo(this);
        }

        private void OnClickPlayButton(Unit unit)
        {
            var model = ExSceneManager.GetRootComponent<DebugModel>();

            if(model.GetIsMemberExist()==false)
            {
                Modal.Create()
                    .SetTitleText("エラー")
                    .SetContextText("メンバーが設定されていません。\n最低でも1キャラは設定する必要があります。")
                    .SetBackground()
                    .SetRightTopButton()
                    .AddButtonActionAndClose("閉じる");
            }
            else
            {
                UniTask.Void(async () =>
                {
                    Connecting.Open();
                    var result = await DummyServerForBattle.GetLotteryChoiseAsync(model.StageData.stageId);
                    Connecting.Close();

                    if (result.IsSuccess)
                    {
                        var data = BridgingDataProvider.Get;
                        data.SetTeamData(model.GetTeamData()); // チームデータ入れ込み
                        data.SetStageData(model.StageData);    //ステージデータの入れ込み
                        data.SetLotteryData(result);           // サーバー抽選結果の入れ込み

                        Fade.FadeOut(0.5f, async () =>
                        {
                            ExSceneManager.Instance.LoadScene(SceneIndex.BATTLE);

                            await UniTask.Delay(1000);

                            Fade.FadeIn(0.5f);
                        });
                    }
                    else
                    {
                        Modal.Create()
                        .SetTitleText("エラー")
                        .SetContextText("サーバーとの通信に失敗しました。")
                        .SetBackground()
                        .SetRightTopButton()
                        .AddButtonActionAndClose("閉じる");
                    }
                });
            }
        }
    }
}