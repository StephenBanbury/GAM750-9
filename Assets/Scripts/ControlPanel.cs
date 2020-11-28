using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Assets.Scripts
{
    public class ControlPanel : RealtimeComponent<MediaScreenDisplayModel>
    {
        public class MediaScreenDisplayBufferState
        {
            public int MediaTypeId;
            public int MediaId;
            public int ScreenDisplayId;
            public bool IsPortal;
        }

        [SerializeField] private Text _bufferText;


        private int _currentSceneId;

        private List<MediaScreenDisplayBufferState> _preparedStateBuffer;
        private MediaScreenDisplayBufferState _preparedState;

        private int _currentVideoClip;
        private int _currentVideoStream;


        void Start()
        {
            _currentSceneId = 1;
            _preparedStateBuffer = new List<MediaScreenDisplayBufferState>();
        }

        private void MediaAssignedToDisplay(RealtimeArray<MediaScreenDisplayStateModel> mediaScreenDisplayStates, MediaScreenDisplayStateModel mediaScreenDisplayState, bool remote)
        {
            Debug.Log("MediaAssignedToDisplay!!");
            //AssignMediaToDisplay();
        }

        protected override void OnRealtimeModelReplaced(MediaScreenDisplayModel previousModel, MediaScreenDisplayModel currentModel)
        {
            Debug.Log("OnRealtimeModelReplaced");

            if (previousModel != null)
            {
                Debug.Log("previousModel != null");

                // Unregister from events
                previousModel.mediaScreenDisplayStates.modelAdded -= MediaAssignedToDisplay;
            }


            if (currentModel != null)
            {
                Debug.Log($"currentModel != null. Models: {currentModel.mediaScreenDisplayStates.Count}");
                //AssignMediaToDisplaysFromArray();

                // Let us know when a new screen has changed 
                currentModel.mediaScreenDisplayStates.modelAdded += MediaAssignedToDisplay;
            }
        }

        public void SceneSelect(int id)
        {
            _currentSceneId = id;
        }

        public void VideoSelect(int id)
        {
            _currentVideoClip = id;
            _currentVideoStream = 0;

            _preparedState = new MediaScreenDisplayBufferState
            {
                MediaTypeId = (int) MediaType.VideoClip,
                MediaId = id
            };
        }

        public void StreamSelect(int id)
        {
            _currentVideoClip = 0;
            _currentVideoStream = id;

            _preparedState = new MediaScreenDisplayBufferState
            {
                MediaTypeId = (int)MediaType.VideoStream,
                MediaId = id
            };
        }

        public void DisplaySelect(int id)
        {
            int compoundId = CompoundScreenId(id);

            if (_preparedState != null)
            {
                var currentScreenState =
                    _preparedStateBuffer.FirstOrDefault(s => s.ScreenDisplayId == compoundId);

                if (currentScreenState != null)
                {
                    currentScreenState.MediaTypeId = _preparedState.MediaTypeId;
                    currentScreenState.MediaId = _preparedState.MediaId;
                }
                else
                {
                    _preparedStateBuffer.Add(new MediaScreenDisplayBufferState
                    {
                        MediaTypeId = (int)(_currentVideoClip > 0 ? MediaType.VideoClip : MediaType.VideoStream),
                        MediaId = _currentVideoClip > 0 ? _currentVideoClip : _currentVideoStream,
                        ScreenDisplayId = compoundId
                    });
                }

                DisplayBuffer();
            }
           
        }

        public void FormationSelect(int id)
        {
            Debug.Log($"FormationId: {id}");

            var  formationSyncScript = gameObject.GetComponent<FormationSelectSync>();
            int compoundId = CompoundFormationId(id);
            formationSyncScript.SetId(compoundId);
        }

        public void Apply()
        {
            foreach (var buffer in _preparedStateBuffer)
            {
                var existing =
                    model.mediaScreenDisplayStates.FirstOrDefault(s => s.screenDisplayId == buffer.ScreenDisplayId);

                if (existing != null)
                {
                    existing.mediaTypeId = buffer.MediaTypeId;
                    existing.mediaId = buffer.MediaId;
                }
                else
                {
                    var mediaState = new MediaScreenDisplayStateModel
                    {
                        mediaTypeId = buffer.MediaTypeId,
                        mediaId = buffer.MediaId,
                        screenDisplayId = buffer.ScreenDisplayId
                    };

                    model.mediaScreenDisplayStates.Add(mediaState);
                }
            }

            _preparedStateBuffer.Clear();
        }

        public void Clear()
        {
            _preparedStateBuffer.Clear();
            DisplayBuffer();
        }

        private void DisplayBuffer()
        {
            _bufferText.text = "";
            foreach (var state in _preparedStateBuffer)
            {
                var sceneId = SceneFromScreenId(state.ScreenDisplayId);
                int displaySceneId = state.ScreenDisplayId - sceneId * 100;
                _bufferText.text +=
                    $"\n{(MediaType)state.MediaTypeId} {state.MediaId} --> Scene {sceneId} / Screen {displaySceneId}";
            }
        }

        private int CompoundFormationId(int formationId)
        {
            // create id in 'composite' form, e.g. 12 = scene 1, formation 2.
            string scenePlusFormation = $"{_currentSceneId}{formationId}";
            int compoundId = int.Parse(scenePlusFormation);
            return compoundId;
        }

        private int CompoundScreenId(int screenId)
        {
            var compoundId = _currentSceneId * 100 + screenId;
            return compoundId;
        }

        private int SceneFromScreenId(int sceneId)
        {
            var scene = sceneId.ToString().Substring(0, 1);
            return int.Parse(scene);
        }
    }
}