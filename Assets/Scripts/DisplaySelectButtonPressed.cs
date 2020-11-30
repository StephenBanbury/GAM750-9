using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DisplaySelectButtonPressed : MonoBehaviour
    {
        [SerializeField] private Text _displayIdText;
        [SerializeField] private int _displayId;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                // _displayId is a composite of scene + _displayId
                // e.g. 814 = scene 8, display 14; 208 = scene 2, display 8

                var scenes = MediaDisplayManager.instance.Scenes;
                var sceneName = GetCurrentSceneFromParent();
                var sceneId = scenes.First(s => s.Name == sceneName).Id;
                var compositeId = sceneId * 100 + _displayId;

                Debug.Log($"Display select displayId:{_displayId}");
                //Debug.Log($"Display select sceneId:{sceneId}");
                //Debug.Log($"Display select compositeId:{compositeId}");


                var gameManager = GameObject.Find("GameManager");

                var displaySelect = gameManager.GetComponent<DisplaySelect>();
                //displaySelect.SetDisplayId(compositeId);
                displaySelect.KeepInSync(compositeId);

                _displayIdText.text = _displayId.ToString();
            }
        }

        private string GetCurrentSceneFromParent()
        {
            var parentScene = transform.parent.parent.parent.gameObject;
            return parentScene.name;
        }
    }
}