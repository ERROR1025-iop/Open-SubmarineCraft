using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Scraft
{
    public class VersionCode : MonoBehaviour
    {


        void Start()
        {
            string versionText = "V" + Application.version;

            // Try TextMeshPro first
            var tmp = GetComponent<TMP_Text>();
            if (tmp != null)
            {
                tmp.text = versionText;
                return;
            }

            // Fallback to legacy UI Text
            var uiText = GetComponent<Text>();
            if (uiText != null)
            {
                uiText.text = versionText;
                return;
            }

            // Try children as a last resort
            tmp = GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                tmp.text = versionText;
                return;
            }

            uiText = GetComponentInChildren<Text>();
            if (uiText != null)
            {
                uiText.text = versionText;
                return;
            }

            Debug.LogWarning($"VersionCode: No Text or TMP_Text found on '{name}' or children.");
        }

    }

}