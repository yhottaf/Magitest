using UnityEngine;

namespace fantec.Battle.Ui
{
    public interface IHudInputGuardView:ILocatable
    {
        void Show();
        void Hide();
    }
}

namespace fantec.Battle.Ui.Hud.InputGuard
{
    public class HudInputGuardView : MonoBehaviour,IHudInputGuardView,IRegistable
    {
        public void Register()
        {
            Locator.Register<IHudInputGuardView>(this);
        }

        public void Show()=>this.gameObject.SetActive(true);
        public void Hide() => this.gameObject.SetActive(false);
    }
}