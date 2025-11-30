using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scraft
{
    public class LoginViaNull : MonoBehaviour
    {
        void Start()
        {            
            GetComponent<Button>().onClick.AddListener(() =>
            {
                OnLoginClick();
            });
        }

        void OnLoginClick()
        {
            LoginHandle.via = "null";
            IToast.instance.hide();
            SceneManager.LoadScene("Menu");
        }        
    }
}
