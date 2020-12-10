//using Assets.Scripts.Enums;
//using UnityEngine;

//namespace Assets.Scripts
//{
//    public class VideoSelect : MonoBehaviour
//    {
//        //private int _videoId;
//        private int _previousId;
        
//        private VideoSelectSync _videoSelectSync;

//        void Start()
//        {
//            _videoSelectSync = gameObject.GetComponent<VideoSelectSync>();
//        }

//        public void SetVideoId(int id)
//        {
//            Debug.Log($"SetVideoId: {id}");

//            //_videoId = id;

//            if (id > 0 && id != _previousId)
//            {
//                MediaDisplayManager.instance.SelectedVideo = id;
//                MediaDisplayManager.instance.SelectedMediaType = MediaType.VideoClip;
//            }
//        }

//        public void KeepInSync(int id)
//        {
//            if (id != _previousId)
//            {
//                Debug.Log($"KeepInSync videoId: {id}");

//                _videoSelectSync.SetId(id);
//                _previousId = id;
//            }
//        }
//    }
//}