using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DisplaySelect : MonoBehaviour
    {
        //private int _displayId;
        private int _previousId;

        private DisplaySelectSync _displaySelectSync;


        void Start()
        {
            _displaySelectSync = gameObject.GetComponent<DisplaySelectSync>();
        }

        public void SetDisplayId(int id)
        {
            Debug.Log($"SetDisplayeId: {id}");
            Debug.Log($"_previousId: {_previousId}");

            //_displayId = id;
            
            if (id > 0 && id != _previousId)
            {
                MediaDisplayManager.instance.SelectedDisplay = id;
                MediaDisplayManager.instance.StoreRealtimeScreenMediaState();
            }
        }

        public void KeepInSync(int id)
        {
            // If the id has changed, call SetId on the sync component
            //if (_displayId != _previousId)
            //{

            // TODO: check display plus media type and id have not changed

            Debug.Log($"Keep in sync: displayId: {id}");

            _displaySelectSync.SetId(id);
            _previousId = id;
            //}
        }
    }
}