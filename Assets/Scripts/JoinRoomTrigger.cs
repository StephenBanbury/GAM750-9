using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{

    public class JoinRoomTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                Debug.Log("JoinRoomTrigger");
                AgoraController.instance.JoinRoom();
            }
        }
    }

}