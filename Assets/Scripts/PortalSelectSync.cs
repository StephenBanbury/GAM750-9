//using Normal.Realtime;
//using UnityEngine;

//namespace Assets.Scripts
//{
//    public class PortalSelectSync : RealtimeComponent
//    {
//        private PortalSelectSyncModel _model;
//        private PortalSelect _portalSelect;

//        void Start()
//        {
//            _portalSelect = GetComponent<PortalSelect>();
//        }

//        private PortalSelectSyncModel model
//        {
//            set
//            {
//                if (_model != null)
//                {
//                    // Unregister from events
//                    _model.displayIdDidChange -= PortalDisplayIdDidChange;
//                }

//                // Store the model
//                _model = value;

//                if (_model != null)
//                {
//                    // Update the display id to match the new model
//                    UpdatePortalDisplayId();

//                    // Register for events so we'll know if the value changes later
//                    _model.displayIdDidChange += PortalDisplayIdDidChange;
//                }
//            }
//        }

//        private void PortalDisplayIdDidChange(PortalSelectSyncModel model, int value)
//        {
//            UpdatePortalDisplayId();
//        }

//        private void UpdatePortalDisplayId()
//        {
//            Debug.Log("UpdatePortalDisplayId");

//            if (_model != null && _model.displayId > 0)
//            {
//                // Get the value from the model, display it and update the manager
//                _portalSelect.SetPortalDisplayId(_model.displayId, true);
                
//                //Debug.Log("Sync: Selected display updated");
                
//                //MediaDisplayManager.instance.SelectedDisplay = _model.displayId;
//                //MediaDisplayManager.instance.StoreScreenDisplayState();
//                //MediaDisplayManager.instance.AssignMediaToDisplay();
//            }
//        }
        
//        public void SetId(int id)
//        {
//            Debug.Log("PortalSelectSync: SetId");

//            // Set the value on the model
//            // This will fire the valueChanged event on the model, which will update the value for both the local player and all remote players
//            _model.displayId = id;
//        }
//    }
//}