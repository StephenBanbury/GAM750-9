using System.IO;
using UnityEngine;

namespace Assets.Scripts
{
    public class OSCManager : MonoBehaviour
    {
        private VideoSelect _videoSelectDisplay;
        private StreamSelect _streamSelectDisplay;
        private DisplaySelect _displaySelectDisplay;
        private FormationSelect _formationSelectDisplay;

        void Start()
        {
            _videoSelectDisplay = gameObject.GetComponentInParent<VideoSelect>();
            _streamSelectDisplay = gameObject.GetComponentInParent<StreamSelect>();
            _displaySelectDisplay = gameObject.GetComponentInParent<DisplaySelect>();
            _formationSelectDisplay = gameObject.GetComponentInParent<FormationSelect>();
        }
        
        public void OnReceiveVideoId(int value)
        {
            Debug.Log($"VideoId: {value}");
           // _videoSelectDisplay.SetVideoId(value);
            _videoSelectDisplay.KeepInSync(value);
        }

        public void OnReceiveStreamId(int value)
        {
            Debug.Log($"StreamId: {value}");
           // _streamSelectDisplay.SetStreamId(value);
            _streamSelectDisplay.KeepInSync(value);
        }

        public void OnReceiveDisplayId(int value)
        {
            Debug.Log($"DisplayId: {value}");
            //_displaySelectDisplay.SetDisplayId(value);
            _displaySelectDisplay.KeepInSync(value);
        }

        public void OnReceiveFormationId(int value)
        {
            Debug.Log($"FormationId: {value}");
            //_formationSelectDisplay.SetFormationId(value, 10);
            _formationSelectDisplay.KeepInSync(value);
        }
    }
}
