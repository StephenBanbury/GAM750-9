//using Assets.Scripts.Enums;
//using UnityEngine;

//namespace Assets.Scripts
//{
//    public class StreamSelect : MonoBehaviour
//    {
//        //private int _streamId;
//        private int _previousId;
        
//        private StreamSelectSync _streamSelectSync;
        
//        void Start()
//        {
//            _streamSelectSync = gameObject.GetComponent<StreamSelectSync>();
//        }

//        public void SetStreamId(int id)
//        {
//            Debug.Log($"SetStreamId: {id}");

//            //_streamId = id;

//            if (id > 0 && id != _previousId)
//            {
//                MediaDisplayManager.instance.SelectedStream = id;
//                MediaDisplayManager.instance.SelectedMediaType = MediaType.VideoStream;
//            }
//        }

//        public void KeepInSync(int id)
//        {
//            // If the id has changed, call SetId on the sync component
//            if (id != _previousId)
//            {
//                Debug.Log($"streamId: {id}");

//                _streamSelectSync.SetId(id);
//                _previousId = id;
//            }
//        }
//    }
//}