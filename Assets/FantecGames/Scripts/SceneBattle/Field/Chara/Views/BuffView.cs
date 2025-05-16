using UnityEngine;

namespace fantec.Battle.Field.Chara
{
    public class BuffView : MonoBehaviour,IInitializable,IReloadable,ISpeedable
    {
        [SerializeField] GameObject m_BuffObj;
        [SerializeField] GameObject m_DebuffObj;

        public void Initialize()
        {
            Reload();
        }

        public void Reload()
        {
            SetActiveDebuff(true);
            SetActiveDebuff(false);
        }

        public void SetTimeScale(float timeScale)
        {

        }

        public void SetActiveBuff(bool enabled)
        {
            m_BuffObj.gameObject.SetActive(enabled);
        }

        public void SetActiveDebuff(bool enabled)
        {
            m_DebuffObj.gameObject.SetActive(enabled);
        }
    }
}