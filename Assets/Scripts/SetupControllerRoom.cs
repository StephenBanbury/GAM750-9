using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SetupControllerRoom : MonoBehaviour
    {
        private Realtime _realtime;

        [SerializeField] private Text _statusText;

        private void Awake()
        {
            _statusText.text = "";

            // Get the Realtime component on this game object
            _realtime = GetComponent<Realtime>();
            // Notify us when Realtime successfully connects to the room
            _realtime.didConnectToRoom += DidConnectToRoom;

            //_realtimeTransform.RequestOwnership();
        }

        private void DidConnectToRoom(Realtime realtime)
        {
            _statusText.text = "Controller room: connected";
            Debug.Log("NNE Controller realtime connected");
        }
    }
}