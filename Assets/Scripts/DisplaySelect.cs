using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DisplaySelect : MonoBehaviour
    {
        private int _displayId;
        private int _previousId;

        private DisplaySelectSync _displaySelectSync;


        void Start()
        {
            _displaySelectSync = gameObject.GetComponent<DisplaySelectSync>();
        }

        public void SetDisplayId(int id)
        {
            Debug.Log($"SetDisplayeId: {id}");

            _displayId = id;
            
            if (_displayId > 0) // && _displayId != _previousId)
            {
                MediaDisplayManager.instance.SelectedDisplay = _displayId;
                MediaDisplayManager.instance.StoreRealtimeScreenMediaState();
            }
        }

        public void KeepInSync()
        {
            // If the id has changed, call SetId on the sync component
            //if (_displayId != _previousId)
            //{

            // TODO: check display plus media type and id have not changed

                Debug.Log($"Keep in sync: displayId: {_displayId}");

                _displaySelectSync.SetId(_displayId);
                _previousId = _displayId;
            //}
        }
    }
}