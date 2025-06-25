using UnityEngine;

namespace fantec.Common
{
    public class ConnectingView : OverlayObject
    {
        public void Show()
        {
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}