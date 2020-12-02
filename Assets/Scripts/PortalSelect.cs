using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class  PortalSelect : MonoBehaviour
    {
        //private int _portalDisplayId;
        private int _previousId;

        private PortalSelectSync _portalSelectSync;


        void Start()
        {
            _portalSelectSync = gameObject.GetComponent<PortalSelectSync>();
        }

        public void SetPortalDisplayId(int id, bool isActive)
        {
            Debug.Log($"SetPortalDisplayId: {id}");

            //_portalDisplayId = id;
            
            if (id > 0 && id != _previousId)
            {
                //MediaDisplayManager.instance.SelectedDisplay = _portalDisplayId;
                MediaDisplayManager.instance.CreatePortal(id, isActive);
            }
        }

        public void KeepInSync(int id)
        {
            Debug.Log($"Keep in sync portalDisplayId: {id}");

            _portalSelectSync.SetId(id);
            _previousId = id;
        }
    }
}