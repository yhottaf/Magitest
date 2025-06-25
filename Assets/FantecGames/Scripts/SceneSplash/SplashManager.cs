using Cysharp.Threading.Tasks;
using fantec.Common;
using UnityEngine;

namespace fantec.Splash.Manager
{
    public class SplashManager : MonoBehaviour
    {
        private void Start()
        {
            Fade.FadeIn(0.5f);
        }
    }
}