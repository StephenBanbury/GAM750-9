using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class  PortalSelect : MonoBehaviour
    {
        private int _portalDisplayId;
        private int _previousId;

        private PortalSelectSync _portalSelectSync;


        void Start()
        {
            _portalSelectSync = gameObject.GetComponent<PortalSelectSync>();
        }

        public void SetPortalDisplayId(int id, bool isActive)
        {
            Debug.Log($"SetPortalDisplayId: {id}");

            _portalDisplayId = id;
            
            if (_portalDisplayId > 0) // && _displayId != _previousId)
            {
                //MediaDisplayManager.instance.SelectedDisplay = _portalDisplayId;
                MediaDisplayManager.instance.CreatePortal(id, isActive);
            }
        }

        public void KeepInSync()
        {
            // If the id has changed, call SetId on the sync component
            //if (_displayId != _previousId)
            //{

            // TODO: check display plus media type and id have not changed

                Debug.Log($"Keep in sync: portalDisplayId: {_portalDisplayId}");

                _portalSelectSync.SetId(_portalDisplayId);
                _previousId = _portalDisplayId;
            //}
        }
    }
}