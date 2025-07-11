// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;

namespace fantec
{

    /// <summary>
    /// パーティクルを拡大縮小する。
    /// 拡大縮小に必要な設定に自動書き換えする
    /// </summary>
    [AddComponentMenu("fantec/Lib/Effect/ParticleAutomaticDestroyer")]
    public class ParticleAutomaticDestroyer : MonoBehaviour
    {
        bool isPlalyed = false;

        void Update()
        {
            if (CheckPlaying())
            {
                isPlalyed = true;
            }
            else if (isPlalyed)
            {
                Destroy(this.gameObject);
            }
        }

        bool CheckPlaying()
        {
            foreach (ParticleSystem particle in this.GetComponentsInChildren<ParticleSystem>(true))
            {
                if (particle.isPlaying) return true;
            }
            return false;
        }
    }
}
