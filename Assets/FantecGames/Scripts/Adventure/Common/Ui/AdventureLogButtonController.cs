using UnityEngine;

namespace fantec.Adventure
{
    public class AdventureLogButtonController : MonoBehaviour
    {
        [SerializeField] AdventureMenuView m_View;

        public void SetLogToggleOff()
        {
            m_View.SetLogtoggleOff();
        }
    }
}