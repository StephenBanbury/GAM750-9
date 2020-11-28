using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class WristButtonController : MonoBehaviour
    {
        [SerializeField] private Text _messageText;
        private string[] _messages;
        private bool _show;

        void Start()
        {
            _messages = new[] {"Show", "Hide" };
            _show = true;
            ShowCurrentMessage();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                ShowCurrentMessage();
            }
        }

        private void ShowCurrentMessage()
        {
            _messageText.text = _messages[_show ? 1 : 0];

            foreach (var panel in GameObject.FindGameObjectsWithTag("SelectionPanel"))
            {
                foreach (Transform child in panel.transform)
                {
                    child.gameObject.SetActive(_show);
                }
            }

            _show = !_show;
        }
    }
}
