using Normal.Realtime;
using UnityEngine;

namespace Assets.Scripts
{
    public class VideoSelectSync : RealtimeComponent<VideoSelectSyncModel>
    {
        //private VideoSelectSyncModel _model;
        private VideoSelect _videoSelectDisplay;
        private int _videoId;

        void Start()
        {
            _videoSelectDisplay = GetComponent<VideoSelect>();
        }

        protected override void OnRealtimeModelReplaced(VideoSelectSyncModel previousModel,
            VideoSelectSyncModel currentModel)
        {
            Debug.Log($"PreviousModel: {previousModel != null}");
            Debug.Log($"CurrentModel: {currentModel != null}");

            if (previousModel != null)
            {
                // Unregister from events
                previousModel.videoIdDidChange -= VideoIdDidChange;
            }

            if (currentModel != null)
            {
                // If this is a model that has no data set on it, populate it with the current value.
                if (currentModel.isFreshModel)
                    currentModel.videoId = _videoId;

                // Update the formationId to match the new model
                UpdateVideoId();

                // Register for events so we'll know if the value changes later
                currentModel.videoIdDidChange += VideoIdDidChange;
            }
        }

        //private VideoSelectSyncModel model
        //{
        //    set
        //    {
        //        //Debug.Log("In VideoSelectSyncModel");

        //        if (_model != null)
        //        {
        //            // Unregister from events
        //            _model.videoIdDidChange -= VideoIdDidChange;
        //        }

        //        // Store the model
        //        _model = value;

        //        if (_model != null)
        //        {
        //            //Debug.Log("in VideoSelectSyncModel");
        //            // Update the video id to match the new model
        //            UpdateVideoId();

        //            // Register for events so we'll know if the value changes later
        //            _model.videoIdDidChange += VideoIdDidChange;
        //        }
        //    }
        //}

        private void VideoIdDidChange(VideoSelectSyncModel model, int value)
        {
            UpdateVideoId();
        }

        private void UpdateVideoId()
        {
            Debug.Log("UpdateVideoId");

            if (model != null && model.videoId > 0)
            {
                // Get the value from the model, display it 
                _videoSelectDisplay.SetVideoId(model.videoId);

                Debug.Log("Sync: Selected video clip updated");

                MediaDisplayManager.instance.SelectedVideo = model.videoId;
            }
        }


        //private void UpdateVideoId()
        //{
        //    Debug.Log("UpdateVideoId");

        //    if (_model != null && _model.videoId > 0)
        //    {
        //        // Get the value from the model, display it 
        //        _videoSelectDisplay.SetVideoId(_model.videoId);

        //        Debug.Log("Sync: Selected video clip updated");

        //        MediaDisplayManager.instance.SelectedVideo = _model.videoId;
        //    }
        //}

        public void SetId(int id)
        {
            Debug.Log("VodeoSelectSync: SetId");

            // Set the value on the model
            // This will fire the valueChanged event on the model, which will update the value for both the local player and all remote players
            model.videoId = id;
        }
    }
}