using System;
using System.Collections;
using Assets.Scripts.Models;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Services;
using DG.Tweening;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MediaDisplayManager : RealtimeComponent<MediaScreenDisplayModel>
    {
        public static MediaDisplayManager instance;

        [SerializeField] private VideoClip[] _videoClips = new VideoClip[5];
        [SerializeField] private Button _button1;
        [SerializeField] private GameObject _screen;
        [SerializeField] private GameObject _screenVariant;
        [SerializeField] private AudioSource _sceneAudio;
        [SerializeField] private GameObject _sceneLights;
        //[SerializeField] private GameObject _selectionPanels;
        //[SerializeField] private Text _lobbyStatusInfoText;
        //[SerializeField] private GameObject _startButton;
        [SerializeField] private Text _debugText;
        //[SerializeField] private Text _hudText;
        [SerializeField] private Text _bufferText;
        [SerializeField] private Text _sceneValue;
        [SerializeField] private Text _formationValue;
        [SerializeField] private Text _videoStreamValue;
        [SerializeField] private Text _videoClipValue;
        [SerializeField] private Text _screenValue;
        [SerializeField] private Material _skybox1;
        [SerializeField] private Material _skybox2;

        private int _sceneIndex;
        //private int _lastSelectedVideoId;
        //private int _lastSelectedStreamId;
        //private int _lastSelectedDisplayId;
        //private MediaType _lastSelectedMediaType;
        private float _floorAdjust = 1.25f;
        private List<ScreenPortalBufferState> _screenPortalBuffer;
        private List<MediaScreenAssignState> _mediaStateBuffer;
        private List<MediaScreenAssignState> _mediaStatePreparationBuffer;
        private MediaScreenAssignState _mediaStatePreparation;
        private int _currentSceneId;
        private int _currentVideoClip;
        private int _currentVideoStream;
        private int _compositeScreenId = 0;
        private float _buttonOffset = 31f;
        private bool _interfaceIsOn = true;

        private List<VideoPlayer> videoPlayers = new List<VideoPlayer>();

        // _lastSelectionSelected = Scene=1; Formation=2; Stream=3; Clip=4; Screen=5; Portal=6
        private int _lastSelectionSelected; 

        //public int SelectedVideo { set => _lastSelectedVideoId = value; }
        //public int SelectedStream { set => _lastSelectedStreamId = value; }
        //public int SelectedDisplay { set => _lastSelectedDisplayId = value; }
        //public MediaType SelectedMediaType { set => _lastSelectedMediaType = value; }

        public List<SceneDetail> Scenes { get; private set; }
        public List<Scene> CanTransformScene { get; set; }
        public Scene MyCurrentScene { get; set; }
        public List<MediaDetail> Videos { get; private set; }
        public List<ScreenActionModel> ScreenActions { get; private set; }
        //public List<int> ScreensAsPortal { get; set; }

        private ScreenPortalStateModel _screenPortalStateModel;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _currentSceneId = 1;
            SetSkybox(false);
            StartCoroutine(AwaitVideosFromApiBeforeStart());
        }

        private void MediaAssignedToDisplay(RealtimeArray<MediaScreenDisplayStateModel> mediaScreenDisplayStates,
            MediaScreenDisplayStateModel mediaScreenDisplayState, bool remote)
        {
            Debug.Log("MediaAssignedToDisplay: -");
            foreach (var modelMediaScreenDisplayState in model.mediaScreenDisplayStates)
            {
                Debug.Log($"RealtimeArray: {(MediaType)modelMediaScreenDisplayState.mediaTypeId} to {modelMediaScreenDisplayState.screenDisplayId}");
            }

            AssignMediaToDisplaysFromArray();
        }
        
        private void PortalAssignedToDisplay(RealtimeArray<ScreenPortalStateModel> screenPortalStates,
            ScreenPortalStateModel screenPortalState, bool remote)
        {
            Debug.Log("PortalAssignedToDisplay: -");
            foreach (var modelScreenPortalState in model.screenPortalStates)
            {
                Debug.Log($"RealtimeArray: {modelScreenPortalState.screenId} is portal: {modelScreenPortalState.isPortal}");
            }

            AssignPortalToDisplaysFromArray();
        }

        protected override void OnRealtimeModelReplaced(MediaScreenDisplayModel previousModel,
            MediaScreenDisplayModel currentModel)
        {
            Debug.Log("OnRealtimeModelReplaced");

            if (previousModel != null)
            {
                Debug.Log("previousModel != null");

                // Unregister from events
                previousModel.mediaScreenDisplayStates.modelAdded -= MediaAssignedToDisplay;
                previousModel.screenPortalStates.modelAdded -= PortalAssignedToDisplay;
            }


            if (currentModel != null)
            {
                Debug.Log($"currentModel != null. Models: {currentModel.mediaScreenDisplayStates.Count}");

                AssignMediaToDisplaysFromArray();
                AssignPortalToDisplaysFromArray();

                // Let us know when a new screen has changed 
                currentModel.mediaScreenDisplayStates.modelAdded += MediaAssignedToDisplay;
                currentModel.screenPortalStates.modelAdded += PortalAssignedToDisplay;
            }
        }


        private ScreenPortalStateModel portalStateModel
        {
            set
            {
                if (_screenPortalStateModel != null)
                {
                    _screenPortalStateModel.isPortalDidChange -= IsPortalDidChange;
                    _screenPortalStateModel.screenIdDidChange -= PortalScreenIdDidChange;
                }

                _screenPortalStateModel = value;

                if (_screenPortalStateModel != null)
                {
                    // Update value
                    _screenPortalStateModel.isPortalDidChange += IsPortalDidChange;
                    _screenPortalStateModel.screenIdDidChange += PortalScreenIdDidChange;
                }
            }
        }

        private void IsPortalDidChange(ScreenPortalStateModel model,  bool value)
        {
            Debug.Log($"IsPortalDidChange :{value}");
        }
        private void PortalScreenIdDidChange(ScreenPortalStateModel model,  int value)
        {
            Debug.Log($"PortalScreenIdDidChange :{value}");
        }


        private void AssignPortalToDisplaysFromArray()
        {
            // If this is a new player joining the room then they may realtime and local buffer arrays may differ
            //Debug.Log("AssignPortalToDisplaysFromArray");

            foreach (var portalState in model.screenPortalStates)
            {
                var existingBufferRecord = _screenPortalBuffer.FirstOrDefault(p => p.ScreenId == portalState.screenId);
                Debug.Log($"Existing portal buffer record: {existingBufferRecord != null}");

                if (existingBufferRecord != null)
                {
                    if(existingBufferRecord.IsPortal == portalState.isPortal && existingBufferRecord.DestinationSceneId == portalState.destinationSceneId) 
                        Debug.Log($"Portal on screen {portalState.screenId} is already set to {portalState.isPortal} and destination scene {portalState.destinationSceneId}");
                    else if (existingBufferRecord.IsPortal == portalState.isPortal)
                    {
                        Debug.Log($"Portal on screen {portalState.screenId} will change to scene {portalState.destinationSceneId}");
                        existingBufferRecord.DestinationSceneId = portalState.destinationSceneId;
                    }
                    else
                    {
                        Debug.Log($"Screen {portalState.screenId} will become a portal to scene {portalState.destinationSceneId}");
                        existingBufferRecord.IsPortal = portalState.isPortal;
                        AssignPortalToScreen(portalState.screenId, portalState.isPortal);
                    }
                }
                else
                {
                    Debug.Log($"Assigning portal: screen: {portalState.screenId}; destination scene: {portalState.destinationSceneId}; isPortal: {portalState.isPortal}");
                    _screenPortalBuffer.Add(new ScreenPortalBufferState
                    {
                        ScreenId = portalState.screenId,
                        DestinationSceneId = portalState.destinationSceneId,
                        IsPortal = portalState.isPortal
                    });
                    AssignPortalToScreen(portalState.screenId, portalState.isPortal);
                }
            }
        }
    //private void AssignPortalToDisplaysFromArray()
    //    {
    //        // If this is a new player joining the room then they may realtime and local buffer arrays may differ
    //        //Debug.Log("AssignPortalToDisplaysFromArray");

    //        foreach (var portalState in model.screenPortalStates)
    //        {
    //            var existingBufferRecord = _screenPortalBuffer.FirstOrDefault(p => p.ScreenId == portalState.screenId);

    //            Debug.Log($"Existing portal buffer record: {existingBufferRecord != null}");

    //            if (existingBufferRecord != null)
    //            {
    //                if(existingBufferRecord.IsPortal == portalState.isPortal) 
    //                    Debug.Log($"Portal on screenId {portalState.screenId} is already set to {portalState.isPortal}");
    //                else
    //                {
    //                    existingBufferRecord.IsPortal = portalState.isPortal;
    //                    AssignPortalToScreen(portalState.screenId, portalState.isPortal);
    //                }
    //            }
    //            else
    //            {
    //                _screenPortalBuffer.Add(new ScreenPortalBufferState
    //                {
    //                    ScreenId = portalState.screenId,
    //                    IsPortal = portalState.isPortal
    //                });

    //                AssignPortalToScreen(portalState.screenId, portalState.isPortal);
    //            }
    //        }
    //    }

        public void AssignMediaToDisplaysFromArray()
        {
            Debug.Log("In AssignMediaToDisplaysFromArray");

            foreach (var mediaInfo in model.mediaScreenDisplayStates)
            {
                var exists = _mediaStateBuffer.FirstOrDefault(m =>
                    m.ScreenDisplayId == mediaInfo.screenDisplayId && m.MediaTypeId == mediaInfo.mediaTypeId &&
                    m.MediaId == mediaInfo.mediaId);

                Debug.Log($"AssignMediaToDisplaysFromArray - exists: {exists}");

                if (exists != null)
                {
                    Debug.Log($"MediaId {mediaInfo.mediaId} already exists on screen {mediaInfo.screenDisplayId}");
                }
                else
                {
                    bool assigned = false;
                    //videoPlayers = new List<VideoPlayer>();

                    switch (mediaInfo.mediaTypeId)
                    {
                        case (int) MediaType.VideoClip:
                            Debug.Log($"Assign video clip {mediaInfo.mediaId} to display {mediaInfo.screenDisplayId}");
                            assigned = AssignVideoToDisplay(mediaInfo.mediaId, mediaInfo.screenDisplayId);
                            break;

                        case (int) MediaType.VideoStream:
                            Debug.Log(
                                $"Assign video stream {mediaInfo.mediaId} to display {mediaInfo.screenDisplayId}");
                            assigned = AssignStreamToDisplay(mediaInfo.mediaId, mediaInfo.screenDisplayId);
                            break;
                    }

                    if (assigned)
                    {
                        _mediaStateBuffer.Add(new MediaScreenAssignState
                        {
                            MediaTypeId = mediaInfo.mediaTypeId,
                            MediaId = mediaInfo.mediaId,
                            ScreenDisplayId = mediaInfo.screenDisplayId
                        });
                    }
                }

                //Debug.Log($"Assign portal to display {mediaInfo.screenDisplayId}?: {mediaInfo.isPortal}");
                //AssignPortalToScreen(mediaInfo.screenDisplayId, mediaInfo.isPortal);
            }
            
            Debug.Log("Preparing VideoPlayers");

            //foreach (VideoPlayer videoPlayer in videoPlayers)
            //{
            //    StartCoroutine(PrepareVideo(videoPlayer));
            //}
        }



        #region Controller UI

        public void SceneSelect(int id)
        {
            Debug.Log($"SceneSelect: {id}; _lastSelectionSelected: {_lastSelectionSelected}");
            
            // If the last button pressed was 'portal' then use previously selected scene and screen to set portal
            if (_lastSelectionSelected == 6)
            {
                StoreRealtimeScreenPortalState(id);
                _lastSelectionSelected = 1;
            }
            else
            {
                _lastSelectionSelected = 1;
                _sceneValue.text = id.ToString();
                _currentSceneId = id;
            }
            EnablePortalButton(false);
        }

        public void FormationSelect(int id)
        {
            Debug.Log($"FormationId: {id}");

            bool canDoFormation = CanTransformScene.Contains((Scene) _currentSceneId);
            
            if (canDoFormation)
            {
                _lastSelectionSelected = 2;

                _formationValue.text = id.ToString();

                var formationSyncScript = gameObject.GetComponent<FormationSelectSync>();
                int compoundId = CompoundFormationId(id);
                formationSyncScript.SetId(compoundId);
            }

            EnablePortalButton(false);
        }

        public void StreamSelect(int id)
        {
            _lastSelectionSelected = 3;

            _videoStreamValue.text = id.ToString();

            _currentVideoClip = 0;
            _currentVideoStream = id;

            _mediaStatePreparation = new MediaScreenAssignState
            {
                MediaTypeId = (int)MediaType.VideoStream,
                MediaId = id
            };

            Debug.Log(
                $"Media state preparation: MediaTypeId = {_mediaStatePreparation.MediaTypeId}; MediaId = {_mediaStatePreparation.MediaId}");
            EnablePortalButton(false);
        }

        private int _hundreds;
        private int _tens;
        private int _ones;

        public void VideoSelect(int id, string buttonName)
        {
            Debug.Log($"VideoSelect id:{id}, buttonName:{buttonName}");
            
            _lastSelectionSelected = 4;

            int temp = _currentVideoClip;

            if (id == 0)
            {
                if (buttonName.Contains("100"))
                    _hundreds = 0;
                else if (buttonName.Contains("10"))
                    _tens = 0;
                else _ones = 0;
            }
            else
            {
                if (id % 100 == 0)
                {
                    _hundreds = id;
                }
                else if (id % 10 == 0)
                {
                    _tens = id;
                }
                else
                {
                    _ones = id;
                }
            }

            _currentVideoClip = _hundreds + _tens + _ones;

            _videoClipValue.text = _currentVideoClip.ToString();

            _mediaStatePreparation = new MediaScreenAssignState
            {
                MediaTypeId = (int) MediaType.VideoClip,
                MediaId = id
            };

            _currentVideoStream = 0;


            Debug.Log($"VideoSelect _mediaStatePreparation:{_mediaStatePreparation.MediaId}");

            EnablePortalButton(false);
        }

        public void ScreenSelect(int id)
        {
            Debug.Log($"ScreenSelect id:{id}");

            int compositeId = CompoundScreenId(id);

            // Last selected = scene
            if (_lastSelectionSelected == 1)
                EnablePortalButton(true);

            // Last selected = video clip/stream
            if (_lastSelectionSelected == 3 || _lastSelectionSelected == 4 || _lastSelectionSelected == 5)
            {

                if (_currentVideoClip > 0 && Videos.All(v => v.Id != _currentVideoClip))
                {
                    Debug.Log($"Video clip {_currentVideoClip} does not exist");
                    _screenValue.text = "";
                    _videoClipValue.text = "";
                    _currentVideoClip = 0;
                    _hundreds = 0;
                    _tens = 0;
                    _ones = 0;
                }
                else
                {
                    _screenValue.text = id.ToString();

                    if (_mediaStatePreparation != null)
                    {
                        var currentScreenState =
                            _mediaStatePreparationBuffer.FirstOrDefault(s => s.ScreenDisplayId == compositeId);

                        if (currentScreenState != null)
                        {
                            currentScreenState.MediaTypeId = _mediaStatePreparation.MediaTypeId;
                            currentScreenState.MediaId = _mediaStatePreparation.MediaId;
                        }
                        else
                        {
                            _mediaStatePreparationBuffer.Add(new MediaScreenAssignState
                            {
                                MediaTypeId =
                                    (int) (_currentVideoClip > 0 ? MediaType.VideoClip : MediaType.VideoStream),
                                MediaId = _currentVideoClip > 0 ? _currentVideoClip : _currentVideoStream,
                                ScreenDisplayId = compositeId
                            });
                        }

                        DisplayBufferText();

                        _lastSelectionSelected = 5;
                    }
                }
            }

            _compositeScreenId = compositeId;
            //ShowPortalButtonState();
        }

        public void PortalSelect()
        {
            // At this point we want both scene and screen, then we can select the scene to teleport to

            if (_lastSelectionSelected == 5 && _currentSceneId > 0 && _compositeScreenId > 0)
            {
                //ShowPortalButtonState();
                Clear();
                _lastSelectionSelected = 6;
            }else if (_lastSelectionSelected != 5)
            {
                EnablePortalButton(false);
                _lastSelectionSelected = 6;
            }

        }

        public void PresetDisplay()
        {

        }

        private void EnablePortalButton(bool enable)
        {
            Button button = GameObject.Find("PortalSelectButton").GetComponent<Button>();
            button.enabled = enable;
            Text portalButtonText = GameObject.Find("PortalSelectButtonText").GetComponent<Text>();
            if (portalButtonText != null)
            {
                if (enable)
                {
                    portalButtonText.text = "Set";
                }
                else
                {
                    portalButtonText.text = "";
                }
            }
        }

        // Get current portal state and show on button
        private void ShowPortalButtonState()
        {
            ScreenPortalStateModel currentPortalState = GetPortalStatus(_compositeScreenId);
            var buttonText = currentPortalState != null && currentPortalState.isPortal ? "On" : "Off";
            Debug.Log($"ScreenId {_compositeScreenId} current portal state: {buttonText}");
            Text portalButtonText = GameObject.Find("PortalSelectButtonText").GetComponent<Text>();
            if (portalButtonText != null)
                portalButtonText.text = buttonText;
            
        }

        public void Apply()
        {
            StoreRealtimeScreenMediaState();
            StoreBufferScreenMediaState();

            //AssignMediaToDisplaysFromArray();

            Clear();
        }

        public void Clear()
        {
            _hundreds = 0;
            _tens = 0;
            _ones = 0;
            //_currentVideoClip = 0;
            //_currentVideoStream = 0;
            _mediaStatePreparationBuffer.Clear();
            DisplayBufferText();
        }

        private void DisplayBufferText()
        {
            _bufferText.text = "";
            foreach (var state in _mediaStatePreparationBuffer)
            {
                var sceneId = SceneFromScreenId(state.ScreenDisplayId);
                int displaySceneId = state.ScreenDisplayId - sceneId * 100;
                _bufferText.text +=
                    $"{(MediaType)state.MediaTypeId} {state.MediaId} --> Scene {sceneId} / Screen {displaySceneId}\n";
            }
        }

        public void DoExitGame()
        {
            Application.Quit();
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

#endregion



        private IEnumerator AwaitVideosFromApiBeforeStart()
        {
            Videos = new List<MediaDetail>();
            //ScreensAsPortal = new List<int>();
            _mediaStatePreparationBuffer = new List<MediaScreenAssignState>();
            _mediaStateBuffer = new List<MediaScreenAssignState>();
            _screenPortalBuffer = new List<ScreenPortalBufferState>();

            GetLocalVideosDetails();

            //GetVideoLinksFromTextFile();

            yield return StartCoroutine(GetVideosFromApi());

            Debug.Log(
                $"Number of videos: " +
                $"Local={Videos.Count(v => v.Source == Source.LocalFile)}; " +
                $"External={Videos.Count(v => v.Source == Source.Url)}");

            yield return StartCoroutine(DownloadVideoFiles(Videos));

            //DownloadVideoFilesSynchronous(Videos);

            Scenes = new List<SceneDetail>();
            ScreenActions = new List<ScreenActionModel>();

            _sceneIndex = 1;

            CanTransformScene = new List<Scene> { Scene.Scene1 };

            SpawnScene(Scene.Scene1, ScreenFormation.LargeSquare);
            SpawnScene(Scene.Scene2, ScreenFormation.SmallSquare);
            SpawnScene(Scene.Scene3, ScreenFormation.Circle);
            SpawnScene(Scene.Scene4, ScreenFormation.Cross);
            SpawnScene(Scene.Scene5, ScreenFormation.ShortRectangle);
            SpawnScene(Scene.Scene6, ScreenFormation.LargeStar);
            SpawnScene(Scene.Scene7, ScreenFormation.Triangle);
            SpawnScene(Scene.Scene8, ScreenFormation.LongRectangle);

            SpawnSceneSelectButtons();
            SpawnFormationSelectButtons();
            SpawnVideoClipSelectButtons();
            SpawnVideoStreamSelectButtons();
            SpawnScreenSelectButtons();

            // Test: set preset screen displays
            SetPresetScreenDisplays();

            EnablePortalButton(false);
            MyCurrentScene = Scene.Scene1;
            _sceneValue.text = "1";
            _lastSelectionSelected = 1;
        }

        private void SetPresetScreenDisplays()
        {
            Debug.Log("set preset screen displays");

            var presetService = new PresetService();

            presetService.Test();

        }

        //public void SetNextScreenAction(int screenId)
        //{
        //    ScreenAction newAction;
        //    ScreenActionModel screenAction = ScreenActions.FirstOrDefault(a => a.ScreenId == screenId);
        //    ScreenAction lastAction = screenAction.NextAction;
        //    Scene scene = GetSceneFromScreenId(screenId);

        //    if (lastAction == ScreenAction.CreatePortal)
        //    {
        //        newAction = ScreenAction.DoTeleport;
        //    }
        //    else
        //    {
        //        bool canDoFormation = CanTransformScene.Contains(scene);
        //        bool hasVideoStreams = AgoraController.instance.AgoraUsers.Count > 0;
        //        int numberOfActions = Enum.GetValues(typeof(ScreenAction)).Cast<int>().Max();
        //        do
        //        {
        //            newAction = (ScreenAction)Math.Ceiling(Random.value * numberOfActions);
        //        } while (newAction == lastAction
        //                 || newAction == ScreenAction.ChangeFormation && !canDoFormation
        //                 || newAction == ScreenAction.DoTeleport && lastAction != ScreenAction.CreatePortal
        //                 || newAction == ScreenAction.ChangeVideoStream && !hasVideoStreams);
        //    }

        //    screenAction.NextAction = newAction;
        //}

        public Scene GetSceneFromScreenId(int screenId)
        {
            int sceneId = GetSceneIdFromScreenId(screenId);
            Scene scene = Scenes.FirstOrDefault(s => s.Id == sceneId).Scene;
            return scene;
        }

        public int GetSceneIdFromScreenId(int screenId)
        {
            string sceneIdString = screenId.ToString().Substring(0, 1);
            int sceneId = int.Parse(sceneIdString);
            return sceneId;
        }

        public int TargetedTeleportation(int screenId)
        {
            var buffer = _screenPortalBuffer.FirstOrDefault(s => s.ScreenId == screenId && s.IsPortal);
            int destinationSceneId = _screenPortalBuffer.First(s => s.ScreenId == screenId && s.IsPortal).DestinationSceneId;

            StartCoroutine(DoTeleportation(destinationSceneId));
            return destinationSceneId;
        }

        public IEnumerator DoTeleportation(int sceneId, bool scatter = false)
        {
            string spawnPointName = $"Spawn Point {sceneId}";
            Transform spawnPoint = GameObject.Find(spawnPointName).transform;
            Transform player = GameObject.Find("Player").transform;
            var playerController = player.GetComponent<OVRPlayerController>();
            var sceneSampleController = player.GetComponent<OVRSceneSampleController>();

            //Debug.Log($"Teleporting to {spawnPointName}");
            //Debug.Log($"SpawnPoint position: {spawnPoint.position}");

            playerController.enabled = false;
            sceneSampleController.enabled = false;

            PlayerAudioManager.instance.PlayAudioClip("Teleport 3_1");

            yield return new WaitForSeconds(2f);

            PlayerAudioManager.instance.PlayAudioClip("Teleport 3_2");

            Vector3 newPosition = spawnPoint.position;

            if (scatter)
            {
                float x = newPosition.x + Random.Range(-1f, 1f);
                float y = newPosition.y + Random.Range(-1f, 1f);
                float z = newPosition.z;
                newPosition = new Vector3(x, y, z);
            }

            player.position = newPosition;

            if (sceneId == 9) MediaDisplayManager.instance.SetSkybox(true);

            yield return new WaitForSeconds(0.5f);

            playerController.enabled = true;
            sceneSampleController.enabled = true;

            MyCurrentScene = (Scene)sceneId;
        }

        public void Targeted360CameraPosition(int sceneId)
        {
            StartCoroutine(GoTo360CameraPosition(sceneId));
        }

        private IEnumerator GoTo360CameraPosition(int sceneId)
        {
            string spawnPointName = $"360 Spawn Point {sceneId}";
            Transform spawnPoint = GameObject.Find(spawnPointName).transform;

            Transform player = GameObject.Find("Main Camera").transform;

            Debug.Log($"Teleporting to {spawnPointName}");
            Debug.Log($"SpawnPoint position: {spawnPoint.position}");

            //PlayerAudioManager.instance.PlayAudioClip("Teleport 3_1");

            yield return new WaitForSeconds(2f);

            //PlayerAudioManager.instance.PlayAudioClip("Teleport 3_2");

            Debug.Log($"Teleporting...");

            Vector3 newPosition = spawnPoint.position;

            player.position = newPosition;

            player.eulerAngles = new Vector3(0f, 0f, 0f);


            if (sceneId == 9) MediaDisplayManager.instance.SetSkybox(true);

            yield return new WaitForSeconds(0.5f);

            MyCurrentScene = (Scene)sceneId;
        }

        private IEnumerator DownloadVideoFiles(List<MediaDetail> mediaDetails)
        {
            Debug.Log("Downloading video files: -");
            //_lobbyStatusInfoText.text = "Downloading video files: -";
            _debugText.text = "Saving video file to: ";

            foreach (var mediaDetail in mediaDetails.Where(m => m.Source == Source.Url))
            {
                string savePath;
                if (Application.platform == RuntimePlatform.Android)
                {
                    string rootPath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android", StringComparison.Ordinal));
                    savePath = Path.Combine(Path.Combine(rootPath, "Android/Data/com.MachineAppStudios.GAM7506/files"), mediaDetail.Filename);
                }
                else
                {
                    savePath = $"{Application.persistentDataPath}/{mediaDetail.Filename}";
                }

                Debug.Log($"Save path: {savePath}");

                mediaDetail.LocalPath = savePath;

                _debugText.text += $"\n{savePath}";
                //_lobbyStatusInfoText.text += $"\n{mediaDetail.Filename}";

                if (File.Exists(savePath))
                {
                    //_lobbyStatusInfoText.text += " - exists.";
                }
                else
                {
                    //_lobbyStatusInfoText.text += " - downloading.";
                    string url = mediaDetail.Url;
                    using (UnityWebRequest www = UnityWebRequest.Get(url))
                    {
                        yield return www.Send();
                        if (www.isNetworkError || www.isHttpError)
                        {
                            Debug.Log(www.error);
                        }
                        else
                        {
                            Debug.Log($"Saving video file to: {savePath}");
                            System.IO.File.WriteAllBytes(savePath, www.downloadHandler.data);
                        }

                        if (File.Exists(savePath))
                            _debugText.text += " - Saved!";
                        else
                            _debugText.text += " - Not saved!";
                    }
                }
            }

            //_lobbyStatusInfoText.text += "\nFinished.";
            //_startButton.SetActive(true);
        }

        private Transform RemoveGameObjectsFromContainer(string containerName, string objectTag)
        {
            Transform container = GameObject.Find(containerName).transform;

            foreach (Transform child in container)
            {
                if (child.gameObject.CompareTag(objectTag))
                    Destroy(child.gameObject);
            }

            return container;
        }

        private void SpawnSceneSelectButtons()
        {
            Debug.Log("SpawnSceneSelectButtons");

            Transform container = RemoveGameObjectsFromContainer("SceneSelectButtons", "Button");

            float xLeft = 50;
            float yPos = 50;

            for (int i = 1; i <= 8; i++)
            {
                var xPos = xLeft + (i - 1) * _buttonOffset;

                var button = Instantiate(_button1);
                button.name = $"Button{i}";

                button.transform.SetParent(container);
                button.transform.localPosition = new Vector2(xPos, yPos);

                Text buttonText = button.GetComponentInChildren<Text>();
                buttonText.text = i.ToString();
                buttonText.fontStyle = FontStyle.Bold;

                int param = i;
                button.onClick.AddListener(delegate { SceneSelect(param); });
                //button.onClick.AddListener(() => SceneSelect(i));
            }
        }

        private void SpawnFormationSelectButtons()
        {
            Debug.Log("SpawnFormationSelectButtons");

            Transform container = RemoveGameObjectsFromContainer("FormationSelectButtons", "Button");

            float xLeft = 50;
            float yPos = 50;

            for (int i = 1; i <= 8; i++)
            {
                var xPos = xLeft + (i - 1) * _buttonOffset;

                var button = Instantiate(_button1);
                button.name = $"Button{i}";

                button.transform.SetParent(container);
                button.transform.localPosition = new Vector2(xPos, yPos);

                Text buttonText = button.GetComponentInChildren<Text>();
                buttonText.text = i.ToString();
                buttonText.fontStyle = FontStyle.Bold;

                int param = i;
                button.onClick.AddListener(delegate { FormationSelect(param); });
                //button.onClick.AddListener(() => SceneSelect(i));
            }
        }


        private void SpawnVideoClipSelectButtons()
        {
            Debug.Log("SpawnVideoClipSelectButtons");

            Transform container = RemoveGameObjectsFromContainer("VideoClipSelectButtons", "Button");

            var nVideos = Videos.Count;

            Debug.Log($"nVideos: {nVideos}");

            // TODO: remove
            //nVideos = 20;

            if (nVideos > 0)
            {
                float xLeft = 50;

                // 100s

                float yPos = 50;

                for (int i = 0; i <= 9; i++)
                {
                    var x = i * 100;

                    var xPos = xLeft + i * _buttonOffset;

                    var button = Instantiate(_button1);
                    string buttonName = $"Button{x} 100";
                    button.name = buttonName;

                    button.transform.SetParent(container);
                    button.transform.localPosition = new Vector2(xPos, yPos);

                    Text buttonText = button.GetComponentInChildren<Text>();
                    buttonText.text = x.ToString();
                    buttonText.fontStyle = FontStyle.Bold;

                    int val = x;
                    //button.onClick.AddListener(delegate { VideoSelect(i); });
                    button.onClick.AddListener(() => VideoSelect(val, buttonName));
                }

                // 10s

                yPos = yPos - _buttonOffset;

                for (int i = 0; i <= 9; i++)
                {
                    var x = i * 10;

                    var xPos = xLeft + i * _buttonOffset;

                    var button = Instantiate(_button1);
                    string buttonName = $"Button{x} 10";
                    button.name = buttonName;

                    button.transform.SetParent(container);
                    button.transform.localPosition = new Vector2(xPos, yPos);

                    Text buttonText = button.GetComponentInChildren<Text>();
                    buttonText.text = x.ToString();
                    buttonText.fontStyle = FontStyle.Bold;

                    int val = x;
                    //button.onClick.AddListener(delegate { VideoSelect(i); });
                    button.onClick.AddListener(() => VideoSelect(val, buttonName));
                }


                // 1s

                yPos = yPos - _buttonOffset;

                for (int i = 0; i <= 9; i++)
                {
                    var xPos = xLeft + i * _buttonOffset;

                    var button = Instantiate(_button1);
                    string buttonName = $"Button{i} 1";
                    button.name = buttonName;

                    button.transform.SetParent(container);
                    button.transform.localPosition = new Vector2(xPos, yPos);

                    Text buttonText = button.GetComponentInChildren<Text>();
                    buttonText.text = i.ToString();
                    buttonText.fontStyle = FontStyle.Bold;

                    int val = i;
                    //button.onClick.AddListener(delegate { VideoSelect(i); });
                    button.onClick.AddListener(() => VideoSelect(val, buttonName));
                }
            }
        }

        public void SpawnVideoStreamSelectButtons()
        {
            Debug.Log("SpawnVideoStreamSelectButtons");

            Transform container = RemoveGameObjectsFromContainer("VideoStreamSelectButtons", "Button");

            var agoraUsers = AgoraController.instance.AgoraUsers;

            //var agoraUsers = AgoraController.instance.AgoraUsers;
            Debug.Log($"agoraUsers: {agoraUsers.Count}");

            if (agoraUsers != null)
            {
                float xLeft = 50;
                float yPos = 50;

                var joinedUsers = agoraUsers.Where(u => !(u.IsLocal || u.LeftRoom)).ToList();
                Debug.Log($"Non-local agora users: {joinedUsers.Count}");

                int i = 1;

                foreach (var joinedUser in joinedUsers)
                {
                    var xPos = xLeft + (i - 1) * _buttonOffset;

                    var button = Instantiate(_button1);
                    button.name = $"Button{i}";

                    button.transform.SetParent(container);
                    button.transform.localPosition = new Vector2(xPos, yPos);

                    Text buttonText = button.GetComponentInChildren<Text>();
                    buttonText.text = joinedUser.Uid.ToString();
                    buttonText.fontStyle = FontStyle.Bold;

                    button.onClick.AddListener(delegate { StreamSelect(joinedUser.Id); });
                    //button.onClick.AddListener(() => StreamSelect(x));

                    i++;
                }
            }
        }

        private void SpawnScreenSelectButtons()
        {
            Debug.Log("SpawnScreenSelectButtons");

            Transform container = RemoveGameObjectsFromContainer("ScreenSelectButtons", "Button");

            float xLeft = 50;
            float yPos = 50;

            for (int i = 1; i <= 8; i++)
            {
                var xPos = xLeft + (i - 1) * _buttonOffset;

                var button = Instantiate(_button1);
                button.name = $"Button{i}";

                button.transform.SetParent(container);
                button.transform.localPosition = new Vector2(xPos, yPos);

                Text buttonText = button.GetComponentInChildren<Text>();
                buttonText.text = i.ToString();
                buttonText.fontStyle = FontStyle.Bold;

                int param = i;
                button.onClick.AddListener(delegate { ScreenSelect(param); });
                //button.onClick.AddListener(() => VideoSelect(i));
            }

            for (int i = 1; i <= 8; i++)
            {
                var xPos = xLeft + (i - 1) * _buttonOffset;

                var button = Instantiate(_button1);
                button.name = $"Button{i + 8}";

                button.transform.SetParent(container);
                button.transform.localPosition = new Vector2(xPos, yPos - _buttonOffset);

                Text buttonText = button.GetComponentInChildren<Text>();
                buttonText.text = (i + 8).ToString();
                buttonText.fontStyle = FontStyle.Bold;

                int param = i + 8;
                button.onClick.AddListener(delegate { ScreenSelect(param); });
                //button.onClick.AddListener(() => VideoSelect(i));
            }
        }


        private void GetLocalVideosDetails()
        {
            Debug.Log("Get Videos from local storage");
            var videoService = new VideoService();
            Videos = videoService.GetLocalVideos();
        }

        private void GetVideoLinksFromTextFile()
        {
            // Get external video URLs from text file
            Debug.Log("Get Videos from text file");

            var textLines = GetVideosExternal.GetFromTextFile();

            var i = 1;
            foreach (var textLine in textLines)
            {
                var video = new MediaDetail
                {
                    Id = i,
                    Title = $"Video {Videos.Count + 1}",
                    MediaType = MediaType.VideoClip,
                    Source = Source.Url,
                    Url = textLine
                };

                Videos.Add(video);

                i++;
            }
        }

        public IEnumerator GetVideosFromApi()
        {
            // Get external video URLs from database
            var apiService = new ApiService();
            var videosFromApi = apiService.VideosGet();

            yield return new WaitUntil(() => videosFromApi.Count > 0);

            Debug.Log($"GetVideosFromApi - done: {videosFromApi.Count}");
            Videos.AddRange(videosFromApi);
        }


        public void StoreBufferScreenMediaState()
        {
            Debug.Log("In StoreBufferScreenMediaState");

            foreach (var prepBuffer in _mediaStatePreparationBuffer)
            {
                var existing =
                    _mediaStateBuffer.FirstOrDefault(s =>
                        s.ScreenDisplayId == prepBuffer.ScreenDisplayId &&
                        s.MediaTypeId == prepBuffer.MediaTypeId &&
                        s.MediaId == prepBuffer.MediaId
                    );

                Debug.Log($"StoreBufferScreenMediaState. Exists: {existing != null}");

                if (existing != null)
                {
                    existing.MediaTypeId = prepBuffer.MediaTypeId;
                    existing.MediaId = prepBuffer.MediaId;
                }
                else
                {
                    MediaScreenAssignState mediaScreenDisplayBufferState = new MediaScreenAssignState
                    {
                        ScreenDisplayId = prepBuffer.ScreenDisplayId,
                        MediaTypeId = prepBuffer.MediaTypeId,
                        MediaId = prepBuffer.MediaId
                    };

                    _mediaStateBuffer.Add(mediaScreenDisplayBufferState);
                }
            }
        }

        public void StoreRealtimeScreenMediaState()
        {
            Debug.Log("In StoreRealtimeScreenMediaState");

            foreach (var buffer in _mediaStatePreparationBuffer)
            {
                var existing =
                    model.mediaScreenDisplayStates.FirstOrDefault(s => s.screenDisplayId == buffer.ScreenDisplayId);

                Debug.Log($"StoreRealtimeScreenMediaState. Exists: {existing != null}");
                Debug.Log($"buffer.Screen: {buffer.ScreenDisplayId}; buffer.MediaType: {buffer.MediaTypeId}; buffer.MediaId: {buffer.MediaId}");

                if (buffer.MediaTypeId == (int) MediaType.VideoClip)
                {
                    Debug.Log($"Video title: {Videos.FirstOrDefault(v => v.Id == buffer.MediaId).Title}");
                }

                if (existing != null)
                {
                    existing.mediaTypeId = buffer.MediaTypeId;
                    existing.mediaId = buffer.MediaId;
                }
                else
                {
                    MediaScreenDisplayStateModel mediaScreenDisplayState = new MediaScreenDisplayStateModel
                    {
                        screenDisplayId = buffer.ScreenDisplayId,
                        mediaTypeId = buffer.MediaTypeId,
                        mediaId = buffer.MediaId
                    };

                    model.mediaScreenDisplayStates.Add(mediaScreenDisplayState);
                }
            }
        }

        public void StoreBufferScreenPortalState()
        {
            var existing =
                _screenPortalBuffer.FirstOrDefault(p => p.ScreenId == _compositeScreenId);

            Debug.Log($"StoreBufferScreenPortalState. Exists: {existing != null}");

            if (existing != null)
            {
                existing.IsPortal = !existing.IsPortal;
            }
            else
            {
                ScreenPortalBufferState bufferState = new ScreenPortalBufferState
                {
                    ScreenId = _compositeScreenId,
                    IsPortal = true
                };

                _screenPortalBuffer.Add(bufferState);
            }

            Debug.Log("StoreBufferScreenPortalState: -");
            foreach (var model in _screenPortalBuffer)
            {
                Debug.Log($"ScreenId {model.ScreenId} is portal: {model.IsPortal}");
            }
        }

        public void StoreRealtimeScreenPortalState(int destinationSceneId)
        {
            var existingRealtimeState =
                model.screenPortalStates.FirstOrDefault(p => p.screenId == _compositeScreenId);

            Debug.Log($"StoreRealtimeScreenPortalState. Exists: {existingRealtimeState != null}");

            if (existingRealtimeState != null)
            {
                // TODO find a way of making this into an event change

                if (existingRealtimeState.destinationSceneId == destinationSceneId)
                {
                    Debug.Log($"Change existing portal state on screen {_compositeScreenId} from {existingRealtimeState.isPortal} to {!existingRealtimeState.isPortal}");
                    existingRealtimeState.isPortal = !existingRealtimeState.isPortal;
                }
                else
                {
                    Debug.Log($"Change existing portal on screen {_compositeScreenId} destination from scene {existingRealtimeState.destinationSceneId} to scene {destinationSceneId}");
                    existingRealtimeState.destinationSceneId = destinationSceneId;
                }

                // Buffer state
                var existingBufferState = _screenPortalBuffer.FirstOrDefault(p => p.ScreenId == _compositeScreenId);
                if (existingBufferState.DestinationSceneId == destinationSceneId)
                {
                    Debug.Log($"Change existing portal state on screen {_compositeScreenId} from {existingBufferState.IsPortal} to {!existingBufferState.IsPortal}");
                    existingBufferState.IsPortal = !existingBufferState.IsPortal;
                }
                else
                {
                    Debug.Log($"Change existing portal destination on screen {_compositeScreenId} from scene {existingBufferState.DestinationSceneId} to scene {destinationSceneId}");
                    existingBufferState.DestinationSceneId = destinationSceneId;
                }
                
                if (!existingBufferState.IsPortal)
                {
                    Transform screenObject = GetScreenObjectFromScreenId(_compositeScreenId);
                    if (screenObject != null)
                    {
                        Transform portal = screenObject.Find("Portal");
                        portal.gameObject.SetActive(false);
                    }

                    //ScreenActionModel screenAction = ScreenActions.FirstOrDefault(a => a.ScreenId == _compositeScreenId);
                    //screenAction.NextAction = ScreenAction.ChangeVideoClip;
                }
            }
            else
            {
                ScreenPortalStateModel state = new ScreenPortalStateModel
                {
                    screenId = _compositeScreenId,
                    destinationSceneId = destinationSceneId,
                    isPortal = true
                };

                model.screenPortalStates.Add(state);
            }

            //Debug.Log("StoreRealtimeScreenPortalState: -");
            //foreach (var model in model.screenPortalStates)
            //{
            //    Debug.Log($"ScreenId {model.screenId} is portal: {model.isPortal}");
            //}
        }

        /*
        public void StoreRealtimeScreenPortalState()
        {
            var existingRealtimeState =
                model.screenPortalStates.FirstOrDefault(p => p.screenId == _compositeScreenId);

            Debug.Log($"StoreRealtimeScreenPortalState. Exists: {existingRealtimeState != null}");

            if (existingRealtimeState != null)
            {
                // TODO find a way of making this into an event change

                Debug.Log($"Change existing portal state from {existingRealtimeState.isPortal} to {!existingRealtimeState.isPortal}");
                existingRealtimeState.isPortal = !existingRealtimeState.isPortal;

                var existingBufferState = _screenPortalBuffer.FirstOrDefault(p => p.ScreenId == _compositeScreenId);
                existingBufferState.IsPortal = !existingBufferState.IsPortal;

                if (!existingBufferState.IsPortal)
                {
                    Transform screenObject = GetScreenObjectFromScreenId(_compositeScreenId);
                    if (screenObject != null)
                    {
                        Transform portal = screenObject.Find("Portal");
                        portal.gameObject.SetActive(false);
                    }

                    //ScreenActionModel screenAction = ScreenActions.FirstOrDefault(a => a.ScreenId == _compositeScreenId);
                    //screenAction.NextAction = ScreenAction.ChangeVideoClip;
                }
            }
            else
            {
                ScreenPortalStateModel state = new ScreenPortalStateModel
                {
                    screenId = _compositeScreenId,
                    isPortal = true
                };

                model.screenPortalStates.Add(state);
            }

            //Debug.Log("StoreRealtimeScreenPortalState: -");
            //foreach (var model in model.screenPortalStates)
            //{
            //    Debug.Log($"ScreenId {model.screenId} is portal: {model.isPortal}");
            //}
        }
        */

        private ScreenPortalStateModel GetPortalStatus(int screenId)
        {
            ScreenPortalStateModel state = model.screenPortalStates.FirstOrDefault(p => p.screenId == screenId);
            return state;
        }

        //public void StoreRealtimeScreenPortalState(int screenId, bool isActive)
        //{

        //    var isPortal = ScreensAsPortal.IndexOf(screenId) != -1;

        //    if (!isPortal && isActive)
        //        ScreensAsPortal.Add(screenId);

        //    if (isPortal && !isActive)
        //        ScreensAsPortal.RemoveAll(id => id == screenId);

        //    var existing =
        //        model.mediaScreenDisplayStates.FirstOrDefault(s => s.screenDisplayId == screenId);

        //    if (existing != null)
        //    {
        //        existing.isPortal = isActive;
        //    }
        //    else
        //    {
        //        MediaScreenDisplayStateModel mediaScreenDisplayState = new MediaScreenDisplayStateModel
        //        {
        //            screenDisplayId = screenId,
        //            isPortal = isActive
        //        };

        //        model.mediaScreenDisplayStates.Add(mediaScreenDisplayState);
        //    }
        //}

        private Transform GetScreenObjectFromScreenId(int screenId)
        {
            var sceneId = int.Parse(screenId.ToString().Substring(0, 1));
            var screensContainerName = $"Screens {sceneId}";
            var screenName = $"Screen {screenId}";
            var screenVariantName = $"Screen Variant {screenId}";
            var sceneName = Scenes.First(s => s.Id == sceneId).Name;
            var scene = GameObject.Find(sceneName);
            var screensContainer = scene.transform.Find(screensContainerName);
            var screenObject = screensContainer.transform.Find(screenName);
            if (screenObject == null) screenObject = screensContainer.transform.Find(screenVariantName);
            return screenObject;
        }


        private bool AssignVideoToDisplay(int videoId, int screenId)
        {
            try
            {
                //Debug.Log("AssignVideoToDisplay");
                //Debug.Log($"videoId: {videoId}");
                //Debug.Log($"displayId: {displayId}");

                var displaySuffix = "Tall";

                var canvasDisplayName = $"CanvasDisplay{displaySuffix}";
                var videoDisplayName = $"VideoDisplay{displaySuffix}";

                if (videoId > 0 && screenId > 0) // && _displayVideo[localDisplayId].Id != videoId)
                {
                    Transform screenObject = GetScreenObjectFromScreenId(screenId);

                    var thisVideoClip = Videos.First(v => v.Id == videoId);

                    Debug.Log($"Show video '{thisVideoClip.Title}' on display {screenObject.name}");

                    Transform videoDisplay = screenObject.transform.Find(videoDisplayName);
                    Transform canvasDisplay = screenObject.transform.Find(canvasDisplayName);

                    videoDisplay.gameObject.SetActive(true);
                    canvasDisplay.gameObject.SetActive(false);

                    VideoPlayer videoPlayer = videoDisplay.GetComponentInChildren<VideoPlayer>();

                    //Add AudioSource
                    AudioSource audioSource = gameObject.AddComponent<AudioSource>();

                    bool isPlaying = false;

                    if (thisVideoClip.Source == Source.Url)
                    {
                        //Debug.Log($"new path: {thisVideoClip.LocalPath}; current path on {videoPlayer.name}: {videoPlayer.url}");

                        if (thisVideoClip.LocalPath == videoPlayer.url)
                        {
                            Debug.Log($"Video {videoId} is already playing on screen {screenId}");
                            isPlaying = true;
                        }
                        else
                        {
                            // Video clip from Url
                            Debug.Log("URL video clip");

                            videoPlayer.source = VideoSource.Url;

                            // Set mode to Audio Source.
                            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

                            // We want to control one audio track with the video player
                            videoPlayer.controlledAudioTrackCount = 1;

                            // We enable the first track, which has the id zero
                            videoPlayer.EnableAudioTrack(0, true);

                            // ...and we set the audio source for this track
                            videoPlayer.SetTargetAudioSource(0, audioSource);

                            videoPlayer.url = thisVideoClip.LocalPath;
                            
                            videoPlayers.Add(videoPlayer);

                            StartCoroutine(PrepareVideo(videoPlayer));
                        }
                    }
                    else
                    {
                        // Video clip from local storage
                        Debug.Log("Local video clip");
                        var vc = _videoClips[videoId - 1];
                        videoPlayer.clip = vc;
                    }

                    return true;
                }

                return false;

            }
            catch (Exception exception)
            {
                Debug.Log(exception);
                return false;
            }
        }
        
        IEnumerator PrepareVideo(VideoPlayer videoPlayer)
        {
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                Debug.Log($"Preparing {videoPlayer.name}");
                yield return new WaitForEndOfFrame();
            }
             
            videoPlayer.Play();

        }

        private bool AssignStreamToDisplay(int streamId, int displayId)
        {
            try
            {
                if (streamId > 0 && displayId > 0)
                {
                    var agoraUsers = AgoraController.instance.AgoraUsers;

                    if (agoraUsers.Any())
                    {
                        Debug.Log("agoraUsers: -");
                        foreach (var user in agoraUsers)
                        {
                            Debug.Log(
                                $" - {user.Uid} (isLocal: {user.IsLocal}, leftRoom: {user.LeftRoom}, id: {user.Id})");
                        }

                        var agoraUser = agoraUsers.FirstOrDefault(u => u.Id == streamId);

                        Debug.Log($"agoraUser exists: {agoraUser != null}");

                        if (agoraUser != null)
                        {
                            if (agoraUser.IsLocal || agoraUser.LeftRoom)
                            {
                                Debug.Log(
                                    $"Something has gone wrong - is local: {agoraUser.IsLocal}, left room: {agoraUser.LeftRoom}.");
                            }
                            else
                            {
                                agoraUser.DisplayId = displayId;
                                AgoraController.instance.AssignStreamToDisplay(agoraUser);
                            }
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                Debug.Log(exception);
                return false;
            }
        }

        private void AssignPortalToScreen(int screenId, bool isActive)
        {
            if (_screenPortalBuffer.Any(p => p.ScreenId == screenId))
            {
                var screenAction = ScreenActions.FirstOrDefault(a => a.ScreenId == screenId);

                if (isActive)
                {
                    Debug.Log($"Assigning NextAction to screen {screenId}");
                    screenAction.NextAction = ScreenAction.DoTeleport;
                }
                else
                {
                    Debug.Log($"Removing portal on screen {screenId}");
                    screenAction.NextAction = ScreenAction.ChangeVideoClip;
                }

                Transform screenObject = GetScreenObjectFromScreenId(screenId);
                if (screenObject != null)
                {
                    Transform portal = screenObject.Find("Portal");
                    portal.gameObject.SetActive(isActive);
                }
            }
        }

        private void SpawnScene(Scene scene, ScreenFormation formation)
        {
            var thisFormation = new List<ScreenPosition>();
            var screenFormationService = new ScreenFormationService(scene);

            switch (formation)
            {
                case ScreenFormation.LargeSquare:
                    thisFormation = screenFormationService.LargeSquare();
                    break;
                case ScreenFormation.SmallSquare:
                    thisFormation = screenFormationService.SmallSquare();
                    break;
                case ScreenFormation.Cross:
                    thisFormation = screenFormationService.Cross();
                    break;
                //case ScreenFormation.SmallStar:
                //    thisFormation = screenFormationService.SmallStar();
                //    break;
                case ScreenFormation.LargeStar:
                    thisFormation = screenFormationService.LargeStar();
                    break;
                case ScreenFormation.Circle:
                    thisFormation = screenFormationService.Circle();
                    break;
                case ScreenFormation.Triangle:
                    thisFormation = screenFormationService.Triangle();
                    break;
                case ScreenFormation.ShortRectangle:
                    thisFormation = screenFormationService.ShortRectangle();
                    break;
                case ScreenFormation.LongRectangle:
                    thisFormation = screenFormationService.LongRectangle();
                    break;
            }


            var scenePosition = screenFormationService.ScenePosition;
            //var sceneObject = Instantiate(_sceneObject, scenePosition, Quaternion.identity);

            var sceneName = $"Scene {_sceneIndex}";
            var sceneObject = GameObject.Find(sceneName);

            if (sceneObject == null)
            {
                //Debug.Log($"{sceneName} not found");
                sceneObject = new GameObject(sceneName);
            }


            // Instantiate selection panels, audio source and lighting as part of scene object

            //var selectionPanelsTrans = sceneObject.transform.Find($"Selection Panel {_sceneIndex}");

            //if (selectionPanelsTrans == null)
            //{
            //    //GameObject selectionPanels = Instantiate(_selectionPanels, _selectionPanels.transform.position + scenePosition, Quaternion.identity);
            //    //selectionPanels.transform.SetParent(sceneObject.transform);
            //    //selectionPanels.name = $"Selection Panel {_sceneIndex}";

            //    //Text indicator = selectionPanels.transform.Find("SceneSelectorView/Canvas/SceneText").GetComponent<Text>();
            //    //if (indicator != null)
            //    //{
            //    //    indicator.text = sceneName;
            //    //}

            //    //foreach (Transform child in selectionPanels.transform)
            //    //{
            //    //    Debug.Log($"In selection panel: {child.name}");
            //    //}

            //    //var indicator = texts.FirstOrDefault(t => t.name == "SceneIndicator");
            //    //if (indicator != null) indicator.text = sceneName;
            //}

            var sceneAudioTrans = sceneObject.transform.Find($"Scene Audio {_sceneIndex}");
            if (sceneAudioTrans == null)
            {
                AudioSource sceneAudio = Instantiate(_sceneAudio, _sceneAudio.transform.position + scenePosition, Quaternion.identity);
                sceneAudio.transform.SetParent(sceneObject.transform);
                sceneAudio.name = $"Scene Audio {_sceneIndex}";
            }

            var sceneLightsTrans = sceneObject.transform.Find($"Scene Lights {_sceneIndex}");
            if (sceneLightsTrans == null)
            {
                GameObject sceneLights = Instantiate(_sceneLights, _sceneLights.transform.position + scenePosition, Quaternion.identity);
                sceneLights.transform.SetParent(sceneObject.transform);
                sceneLights.name = $"Scene Lights {_sceneIndex}";
            }

            Scenes.Add(new SceneDetail
            {
                Id = _sceneIndex,
                Scene = scene,
                Name = sceneName,
                ScreenFormation = formation,
                ScenePosition = scenePosition,
                CurrentScreens = new List<GameObject>()
            });

            GameObject screensContainer = GameObject.Find($"Screens {_sceneIndex}");

            if (screensContainer == null)
            {
                //Debug.Log($"Screens {_sceneIndex} not found");
                screensContainer = new GameObject($"Screens {_sceneIndex}");
                screensContainer.transform.SetParent(sceneObject.transform);
            }

            var currentScene = Scenes.First(s => s.Id == _sceneIndex);


            // TODO: these could be elsewhere
            const bool showHiddenScreens = false;
            const bool showNumbers = false;

            foreach (var screenPosition in thisFormation)
            {
                var screenId = _sceneIndex * 100 + screenPosition.Id;

                //if (showHiddenScreens || (!showHiddenScreens && screenPosition.Hide))
                //{
                var vector3 = screenPosition.Vector3;
                vector3.y += _floorAdjust;

                GameObject screen;

                GameObject thisScreen;
                string screenName;

                if (screenPosition.Id % 2 != 0)
                {
                    screenName = $"Screen {screenId}";
                    thisScreen = _screen;
                }
                else
                {
                    screenName = $"Screen Variant {screenId}";
                    thisScreen = _screenVariant;
                }

                screen = GameObject.Find(screenName);

                if (screen == null)
                {
                    screen = Instantiate(thisScreen, vector3, Quaternion.identity);
                    //screen = Realtime.Instantiate(screenName, vector3, Quaternion.identity);
                    screen.transform.Rotate(0, screenPosition.Rotation, 0);
                    screen.transform.SetParent(screensContainer.transform);
                    if (!showHiddenScreens && screenPosition.Hide)
                    {
                        screen.SetActive(false);
                    }
                }
                //else
                //{
                //    Debug.Log($"{screenName} exists");
                //}

                screen.name = screenName;

                var screenNumber = screen.GetComponentInChildren<Text>();
                if (screenNumber != null)
                {
                    screenNumber.text = screenPosition.Id.ToString();
                    screenNumber.enabled = showNumbers;
                }


                var screenCamera = screen.GetComponentInChildren<Camera>();
                if (screenCamera != null)
                {
                    CameraSetup(screenCamera, $"Camera {screenId}", false);
                }

                currentScene.CurrentScreens.Add(screen);

                //SetNextScreenAction(screenId);
                //}

                ScreenActions.Add(new ScreenActionModel
                {
                    ScreenId = screenId
                });
            }

            _sceneIndex++;
        }

        private void CameraSetup(Camera camera, string name, bool isEnabled)
        {
            camera.name = name;
            camera.enabled = isEnabled;
            camera.GetComponent<AudioListener>().enabled = isEnabled;
            Debug.Log($"{camera.name} is enabled: {camera.isActiveAndEnabled}");
        }

        private void CameraSwitch(string cameraName)
        {

        }

        public void TweenScreens(ScreenFormation newFormation, int tweenTimeSeconds)
        {
            TweenScreens(MyCurrentScene, newFormation, tweenTimeSeconds);
        }

        public void TweenScreens(Scene scene, ScreenFormation newFormation, int tweenTimeSeconds)
        {
            var thisFormation = new List<ScreenPosition>();
            var screenFormationService = new ScreenFormationService(scene);

            switch (newFormation)
            {
                case ScreenFormation.LargeSquare:
                    thisFormation = screenFormationService.LargeSquare();
                    break;
                case ScreenFormation.SmallSquare:
                    thisFormation = screenFormationService.SmallSquare();
                    break;
                case ScreenFormation.Cross:
                    thisFormation = screenFormationService.Cross();
                    break;
                //case ScreenFormation.SmallStar:
                //    thisFormation = screenFormationService.SmallStar();
                //    break;
                case ScreenFormation.LargeStar:
                    thisFormation = screenFormationService.LargeStar();
                    break;
                case ScreenFormation.Circle:
                    thisFormation = screenFormationService.Circle();
                    break;
                case ScreenFormation.Triangle:
                    thisFormation = screenFormationService.Triangle();
                    break;
                case ScreenFormation.ShortRectangle:
                    thisFormation = screenFormationService.ShortRectangle();
                    break;
                case ScreenFormation.LongRectangle:
                    thisFormation = screenFormationService.LongRectangle();
                    break;
            }

            var thisScene = Scenes.FirstOrDefault(s => s.Scene == scene);

            if (thisScene != null)
            {
                var audioSource =
                    GameObject.Find(thisScene.Name)
                        .transform.Find($"Scene Audio {thisScene.Id}")
                        .GetComponent<AudioSource>();

                audioSource.Play();

                foreach (var screenPosition in thisFormation)
                {
                    var screenPositionPrev = thisScene.CurrentScreens[screenPosition.Id - 1];

                    var vector3To = screenPosition.Vector3;
                    vector3To.y += _floorAdjust;

                    screenPositionPrev.transform.DOMove(vector3To, tweenTimeSeconds).SetEase(Ease.Linear);
                    screenPositionPrev.transform.DORotate(new Vector3(0, screenPosition.Rotation, 0), 3)
                        .SetEase(Ease.Linear);
                }

            }
        }

        public void GrandFinale()
        {
            var gameManager = GameObject.Find("GameManager");
            var teleportToScene = gameManager.GetComponent<TeleportToSceneSync>();
            teleportToScene.SetNewScene(9);
        }

        public void SetSkybox(bool useFinale)
        {
            if (!useFinale)
            {
                RenderSettings.skybox = _skybox1;
            }
            else
            {
                RenderSettings.skybox = _skybox2;

                GameObject vpContainer = GameObject.Find("SkyboxVideoPlayer");
                if (vpContainer != null)
                {
                    VideoPlayer vp = vpContainer.GetComponent<VideoPlayer>();
                    vp.Play();
                }
            }
        }

        public void toggleInterface()
        {
            _interfaceIsOn = !_interfaceIsOn;
            GameObject controllerUi = GameObject.FindGameObjectWithTag("ControllerUI");
            var canvas = controllerUi.GetComponent<Canvas>();
            canvas.enabled = _interfaceIsOn;
        }
    }
}
