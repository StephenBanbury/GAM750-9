using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts
{
    public class VideoSelect : MonoBehaviour
    {
        private int _videoId;
        private int _previousId;
        
        private VideoSelectSync _videoSelectSync;

        void Start()
        {
            _videoSelectSync = gameObject.GetComponent<VideoSelectSync>();
        }

        public void SetVideoId(int id)
        {
            Debug.Log($"SetVideoId: {id}");

            _videoId = id;

            if (_videoId > 0 && _videoId != _previousId)
            {
                MediaDisplayManager.instance.SelectedVideo = _videoId;
                MediaDisplayManager.instance.SelectedMediaType = MediaType.VideoClip;
            }
        }

        public void KeepInSync()
        {
            // If the id has changed, call SetId on the sync component
            if (_videoId != _previousId)
            {
                Debug.Log($"videoId: {_videoId}");

                _videoSelectSync.SetId(_videoId);
                _previousId = _videoId;
            }
        }
    }
}