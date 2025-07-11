using UnityEngine;

using fantec.Common;

namespace fantec.Adventure
{
    public class AdventureEngineEnder : MonoBehaviour
    {
        public void OnEnd()
        {
            Loading.Show(0.5f, 1f, () => { ExSceneManager.Instance.LoadScene(SceneIndex.MENU); });
        }
    }
}
