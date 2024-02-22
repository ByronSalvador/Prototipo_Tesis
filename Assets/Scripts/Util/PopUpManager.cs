using UnityEngine;

namespace Util
{
    public class PopUpManager : MonoBehaviour
    {
        public GameObject popup;
        
        public void ShowPopUp(string message)
        {
            
            popup.SetActive(true);
            popup.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;
        }
    }
}
