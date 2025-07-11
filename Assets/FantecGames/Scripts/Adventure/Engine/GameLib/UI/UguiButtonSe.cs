using fantecExtensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace fantec
{

    /// <summary>
    /// イベントを受け取ってSEを鳴らす
    /// </summary>
    [AddComponentMenu("fantec/Lib/UI/UguiButtonSe")]
    public class UguiButtonSe : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, ISubmitHandler, IMoveHandler
    {
        Selectable Selectable { get { return this.GetComponentCache(ref selectable); } }
        Selectable selectable;

        //クリック時のSE
        public AudioClip clicked;

        //ハイライト時のSE
        public AudioClip highlited;

        //同じSEが鳴っていたら鳴らさないとか前のを止めるとか
        public SoundPlayMode clickedPlayMode = SoundPlayMode.Add;
        //同じSEが鳴っていたら鳴らさないとか前のを止めるとか
        public SoundPlayMode highlitedPlayMode = SoundPlayMode.Add;

        // クリックイベントでSEを鳴らす
        public void OnPointerClick(PointerEventData data)
        {
            //左クリックのみに反応
            if (data.button != PointerEventData.InputButton.Left) return;

            PlayeSe(clickedPlayMode, clicked);
        }

        // ハイライトでSEを鳴らす
        public void OnPointerEnter(PointerEventData data)
        {
            PlayeSe(highlitedPlayMode, highlited);
        }

        // 決定でSEを鳴らす
        public void OnSubmit(BaseEventData eventData)
        {
            PlayeSe(clickedPlayMode, clicked);
        }

        // キー移動でSE鳴らす
        public void OnMove(AxisEventData eventData)
        {
            if (eventData.selectedObject == this.gameObject) return;
            PlayeSe(highlitedPlayMode, highlited);
        }

        void PlayeSe(SoundPlayMode playMode, AudioClip clip)
        {
            if (Selectable == null) return;
            if (!Selectable.interactable) return;

            if (clip != null)
            {
                SoundManager soundManager = SoundManager.GetInstance();

                if (soundManager)
                {
                    soundManager.PlaySe(clip, clip.name, playMode);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(clip, Vector3.zero);
                }
            }
        }
    }
}
