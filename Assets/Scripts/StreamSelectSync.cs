//using Normal.Realtime;
//using UnityEngine;

//namespace Assets.Scripts
//{
//    public class StreamSelectSync : RealtimeComponent
//    {
//        private StreamSelectSyncModel _model;
//        private StreamSelect _streamSelectDisplay;

//        void Start()
//        {
//            _streamSelectDisplay = GetComponent<StreamSelect>();
//        }

//        private StreamSelectSyncModel model
//        {
//            set
//            {
//                //Debug.Log("In StreamSelectSyncModel");

//                if (_model != null)
//                {
//                    // Unregister from events
//                    _model.streamIdDidChange -= StreamIdDidChange;
//                }

//                // Store the model
//                _model = value;

//                if (_model != null)
//                {
//                    // Update the stream id to match the new model
//                    UpdateStreamId();

//                    // Register for events so we'll know if the value changes later
//                    _model.streamIdDidChange += StreamIdDidChange;
//                }
//            }
//        }

//        private void StreamIdDidChange(StreamSelectSyncModel model, int value)
//        {
//            UpdateStreamId();
//        }

//        private void UpdateStreamId()
//        {
//            Debug.Log("UpdateStreamId");

//            if (_model != null && _model.streamId > 0)
//            {
//                // Get the value from the model, display it 
//                _streamSelectDisplay.SetStreamId(_model.streamId);

//                Debug.Log("Sync: Selected stream updated");

//                MediaDisplayManager.instance.SelectedStream = _model.streamId;
//            }
//        }

//        public void SetId(int id)
//        {
//            Debug.Log("StreamSelectSync: SetId");

//            // Set the value on the model
//            // This will fire the valueChanged event on the model, which will update the value for both the local player and all remote players
//            _model.streamId = id;
//        }
//    }
//}