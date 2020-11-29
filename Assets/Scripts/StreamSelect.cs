using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts
{
    public class StreamSelect : MonoBehaviour
    {
        private int _streamId;
        private int _previousId;
        
        private StreamSelectSync _streamSelectSync;
        
        void Start()
        {
            _streamSelectSync = gameObject.GetComponent<StreamSelectSync>();
        }

        public void SetStreamId(int id)
        {
            Debug.Log($"SetStreamId: {id}");

            _streamId = id;

            if (_streamId > 0 && _streamId != _previousId)
            {
                MediaDisplayManager.instance.SelectedStream = _streamId;
                MediaDisplayManager.instance.SelectedMediaType = MediaType.VideoStream;
            }
        }

        public void KeepInSync()
        {
            // If the id has changed, call SetId on the sync component
            if (_streamId != _previousId)
            {
                Debug.Log($"streamId: {_streamId}");
                _previousId = _streamId;

                _streamSelectSync.SetId(_streamId);
            }
        }
    }
}