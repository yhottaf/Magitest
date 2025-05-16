using fantec.Battle.Manager;
using UnityEngine;

namespace fantec.Battle.Ui.Animation
{
    public class AnimationEventSePlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] m_Clips;

        public void OnPlay(int index)
        {
            try
            {
                if(index==-1)
                {
                    Locator.Resolve<IBattleSoundManager>().PlaySe(m_Clips[Random.Range(0, m_Clips.Length)]);
                }
                else
                {
                    {
                        Locator.Resolve<IBattleSoundManager>().PlaySe(m_Clips[index]);
                    }
                }
            }
            catch { Debug.LogWarning($"[index : {index}] にクリップが割り当てられていません。"); }
        }
    }
}