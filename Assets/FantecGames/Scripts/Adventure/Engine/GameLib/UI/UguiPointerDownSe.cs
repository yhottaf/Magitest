using UnityEngine;
using UnityEngine.EventSystems;


namespace fantec
{

    /// <summary>
    /// イベントを受け取ってSEを鳴らす
    /// </summary>
    [AddComponentMenu("fantec/Lib/UI/UguiPointerDownSe")]
    public class UguiPointerDownSe : MonoBehaviour, IPointerDownHandler
    {
        //ポインターダウン時のSE
        public AudioClip se;

        //同じSEが鳴っていたら鳴らさないとか前のを止めるとか
        public SoundPlayMode playMode = SoundPlayMode.Add;

        // ポインターダウンでイベントでSEを鳴らす
        public void OnPointerDown(PointerEventData data)
        {
            //左クリックのみに反応
            if (data.button != PointerEventData.InputButton.Left) return;

            PlaySe(playMode, se);
        }

        void PlaySe(SoundPlayMode mode, AudioClip clip)
        {
            if (clip != null)
            {
                SoundManager soundManager = SoundManager.GetInstance();

                if (soundManager)
                {
                    soundManager.PlaySe(clip, clip.name, mode);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(clip, Vector3.zero);
                }
            }
        }
    }
}
