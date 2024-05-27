using TMPro;
using UnityEngine;

namespace Script.Controller.UI
{
    public class SubtitleController : MonoBehaviour
    {
        // textMeshPro
        public TextMeshProUGUI subtitleText;

        public bool UpdateSubtitleText(string text)
        {
            if (subtitleText == null)
            {
                Debug.LogError("Subtitle Text is null");
                return false;
            }

            subtitleText.text = text;
            return true;
        }
        
        
    }
}