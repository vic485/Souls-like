using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Gazzotto.Localization
{
    public class StartupManager : MonoBehaviour
    {
        // TODO: Have this work behind the scenes in splash screens,
        // and load menu when both localization and splash are done.
        private IEnumerator Start()
        {
            while (!LocalizationManager.instance.GetIsReady())
            {
                yield return null;
            }

            SceneManager.LoadScene("MenuScreen");
        }
    }
}