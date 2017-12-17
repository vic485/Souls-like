using UnityEngine;
using UnityEngine.UI;

namespace Gazzotto.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        public string key;

        private void Start()
        {
            Text text = GetComponent<Text>();
            text.text = LocalizationManager.instance.GetLocalizedValue(key);
        }
    }
}