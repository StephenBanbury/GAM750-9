//using System.Collections.Generic;
//using System.Linq;
//using Assets.Scripts.Enums;
//using Normal.Realtime;
//using Normal.Realtime.Serialization;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Assets.Scripts
//{
//    public class ControlPanel : RealtimeComponent<MediaScreenDisplayModel>
//    {
//        public class MediaScreenDisplayBufferState
//        {
//            public int MediaTypeId;
//            public int MediaId;
//            public int ScreenDisplayId;
//            public bool IsPortal;
//        }

//        [SerializeField] private Text _bufferText;

//        private List<MediaScreenDisplayBufferState> _preparedStateBuffer;
//        private MediaScreenDisplayBufferState _preparedState;

//        private int _currentSceneId;
//        private int _currentVideoClip;
//        private int _currentVideoStream;


//        void Start()
//        {
//            _currentSceneId = 1;
//            _preparedStateBuffer = new List<MediaScreenDisplayBufferState>();
//        }


//        private void MediaAssignedToDisplay(RealtimeArray<MediaScreenDisplayStateModel> mediaScreenDisplayStates,
//            MediaScreenDisplayStateModel mediaScreenDisplayState, bool remote)
//        {
//            Debug.Log("MediaAssignedToDisplay: -");
//            foreach (var modelMediaScreenDisplayState in model.mediaScreenDisplayStates)
//            {
//                Debug.Log($"{(MediaType)modelMediaScreenDisplayState.mediaTypeId} to {modelMediaScreenDisplayState.screenDisplayId}");
//            }

//            AssignMediaToDisplaysFromArray();
//        }

//        protected override void OnRealtimeModelReplaced(MediaScreenDisplayModel previousModel,
//            MediaScreenDisplayModel currentModel)
//        {
//            // Clear Mesh
//            //_mesh.ClearRibbon();

//            // TODO: Clear screens

//            Debug.Log("OnRealtimeModelReplaced");

//            if (previousModel != null)
//            {
//                Debug.Log("previousModel != null");

//                // Unregister from events
//                previousModel.mediaScreenDisplayStates.modelAdded -= MediaAssignedToDisplay;
//            }


//            if (currentModel != null)
//            {
//                Debug.Log($"currentModel != null. Models: {currentModel.mediaScreenDisplayStates.Count}");
//                AssignMediaToDisplaysFromArray();

//                // Let us know when a new screen has changed 
//                currentModel.mediaScreenDisplayStates.modelAdded += MediaAssignedToDisplay;
//            }

//        }

//        public void AssignMediaToDisplaysFromArray()
//        {

//            Debug.Log("AssignMediaToDisplaysFromArray: -");
//            foreach (var modelMediaScreenDisplayState in model.mediaScreenDisplayStates)
//            {
//                Debug.Log($"{(MediaType)modelMediaScreenDisplayState.mediaTypeId} to {modelMediaScreenDisplayState.screenDisplayId}");
//            }

//            foreach (var mediaInfo in model.mediaScreenDisplayStates)
//            {
//                Debug.Log($"AssignMediaToDisplaysFromArray. mediaInfo: {mediaInfo.screenDisplayId}");

//                switch (mediaInfo.mediaTypeId)
//                {
//                    case (int)MediaType.VideoClip:
//                        Debug.Log($"Assign video clip {mediaInfo.mediaId} to display {mediaInfo.screenDisplayId}");
//                        AssignVideoToDisplay(mediaInfo.mediaId, mediaInfo.screenDisplayId);
//                        break;

//                    case (int)MediaType.VideoStream:
//                        Debug.Log($"Assign video stream {mediaInfo.mediaId} to display {mediaInfo.screenDisplayId}");
//                        AssignStreamToDisplay(mediaInfo.mediaId, mediaInfo.screenDisplayId);
//                        break;
//                }

//                Debug.Log($"Assign portal to display {mediaInfo.screenDisplayId}?: {mediaInfo.isPortal}");
//                AssignPortalToScreen(mediaInfo.screenDisplayId, mediaInfo.isPortal);
//            }

//            Debug.Log("AssignMediaToDisplaysFromArray: -");
//            foreach (var modelMediaScreenDisplayState in model.mediaScreenDisplayStates)
//            {
//                Debug.Log($"{(MediaType)modelMediaScreenDisplayState.mediaTypeId} to {modelMediaScreenDisplayState.screenDisplayId}");
//            }
//        }




//        public void SceneSelect(int id)
//        {
//            _currentSceneId = id;
//        }

//        public void FormationSelect(int id)
//        {
//            Debug.Log($"FormationId: {id}");

//            var formationSyncScript = gameObject.GetComponent<FormationSelectSync>();
//            int compoundId = CompoundFormationId(id);
//            formationSyncScript.SetId(compoundId);
//        }

//        public void VideoSelect(int id)
//        {
//            _currentVideoClip = id;
//            _currentVideoStream = 0;

//            _preparedState = new MediaScreenDisplayBufferState
//            {
//                MediaTypeId = (int) MediaType.VideoClip,
//                MediaId = id
//            };
//        }

//        public void StreamSelect(int id)
//        {
//            _currentVideoClip = 0;
//            _currentVideoStream = id;

//            _preparedState = new MediaScreenDisplayBufferState
//            {
//                MediaTypeId = (int)MediaType.VideoStream,
//                MediaId = id
//            };
//        }

//        public void DisplaySelect(int id)
//        {
//            int compoundId = CompoundScreenId(id);

//            if (_preparedState != null)
//            {
//                var currentScreenState =
//                    _preparedStateBuffer.FirstOrDefault(s => s.ScreenDisplayId == compoundId);

//                if (currentScreenState != null)
//                {
//                    currentScreenState.MediaTypeId = _preparedState.MediaTypeId;
//                    currentScreenState.MediaId = _preparedState.MediaId;
//                }
//                else
//                {
//                    _preparedStateBuffer.Add(new MediaScreenDisplayBufferState
//                    {
//                        MediaTypeId = (int)(_currentVideoClip > 0 ? MediaType.VideoClip : MediaType.VideoStream),
//                        MediaId = _currentVideoClip > 0 ? _currentVideoClip : _currentVideoStream,
//                        ScreenDisplayId = compoundId
//                    });
//                }

//                DisplayBuffer();
//            }
           
//        }

//        public void Apply()
//        {
//            foreach (var buffer in _preparedStateBuffer)
//            {
//                //var gameManager = GameObject.Find("GameManager");

//                //var videoSelect = gameManager.GetComponent<VideoSelect>();
//                //var streamSelect = gameManager.GetComponent<StreamSelect>();
//                //var displaySelect = gameManager.GetComponent<DisplaySelect>();

//                //if (buffer.MediaTypeId == (int) MediaType.VideoClip)
//                //{
//                //    //videoSelect.SetVideoId(buffer.MediaId);
//                //    videoSelect.KeepInSync(buffer.MediaId);
//                //}
//                //else
//                //{
//                //    //streamSelect.SetStreamId(buffer.MediaId);
//                //    streamSelect.KeepInSync(buffer.MediaId);
//                //}

//                ////displaySelect.SetDisplayId(buffer.ScreenDisplayId);
//                //displaySelect.KeepInSync(buffer.ScreenDisplayId);


//                var existing =
//                    model.mediaScreenDisplayStates.FirstOrDefault(s => s.screenDisplayId == buffer.ScreenDisplayId);

//                Debug.Log($"Apply. Exists: {existing != null}");

//                if (existing != null)
//                {
//                    existing.mediaTypeId = buffer.MediaTypeId;
//                    existing.mediaId = buffer.MediaId;

//                    //existing.isPortal = isPortal;
//                }
//                else
//                {
//                    MediaScreenDisplayStateModel mediaScreenDisplayState = new MediaScreenDisplayStateModel
//                    {
//                        screenDisplayId = buffer.ScreenDisplayId,
//                        mediaTypeId = buffer.MediaTypeId,
//                        mediaId = buffer.MediaId

//                        //isPortal = isPortal
//                    };

//                    model.mediaScreenDisplayStates.Add(mediaScreenDisplayState);

//                }
                
//                Debug.Log("Apply: -");
//                foreach (var modelMediaScreenDisplayState in model.mediaScreenDisplayStates)
//                {
//                    Debug.Log($"{(MediaType)modelMediaScreenDisplayState.mediaTypeId} to {modelMediaScreenDisplayState.screenDisplayId}");
//                }
//            }

//            _preparedStateBuffer.Clear();
//        }

//        public void Clear()
//        {
//            _preparedStateBuffer.Clear();
//            DisplayBuffer();
//        }

//        private void DisplayBuffer()
//        {
//            _bufferText.text = "";
//            foreach (var state in _preparedStateBuffer)
//            {
//                var sceneId = SceneFromScreenId(state.ScreenDisplayId);
//                int displaySceneId = state.ScreenDisplayId - sceneId * 100;
//                _bufferText.text +=
//                    $"\n{(MediaType)state.MediaTypeId} {state.MediaId} --> Scene {sceneId} / Screen {displaySceneId}";
//            }
//        }

//        private int CompoundFormationId(int formationId)
//        {
//            // create id in 'composite' form, e.g. 12 = scene 1, formation 2.
//            string scenePlusFormation = $"{_currentSceneId}{formationId}";
//            int compoundId = int.Parse(scenePlusFormation);
//            return compoundId;
//        }

//        private int CompoundScreenId(int screenId)
//        {
//            var compoundId = _currentSceneId * 100 + screenId;
//            return compoundId;
//        }

//        private int SceneFromScreenId(int sceneId)
//        {
//            var scene = sceneId.ToString().Substring(0, 1);
//            return int.Parse(scene);
//        }
//    }
//}