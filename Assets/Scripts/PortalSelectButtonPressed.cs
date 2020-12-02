using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PortalSelectButtonPressed : MonoBehaviour
    {
        [SerializeField] private Text _displayIdText;

        public void ButtonPressed()
        {
            MediaDisplayManager.instance.PortalSelect();
        }
    }
}