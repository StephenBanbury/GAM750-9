using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BoxEntered : MonoBehaviour
    {
        [SerializeField] private Text _scaleText;

        private float _scaleValue;
        private float _previousScaleValue;

        private SlidingScaleSync _slidingScaleSync;

        public void SetScaleValue(float scaleValue)
        {
            //Debug.Log($"SetScaleValue: {scaleValue}");
            _scaleValue = scaleValue;
            _scaleText.text = _scaleValue.ToString();
        }

        void Start()
        {
            // Get a reference to the sliding scale sync component
            _slidingScaleSync = GetComponent<SlidingScaleSync>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                Debug.Log("Hand entered!");
            }
        }

        // OnTriggerStay is called once per frame for every Collider other that is touching the trigger
        private void OnTriggerStay(Collider other)
        {
            //if (other.CompareTag("Hand"))
            //{
            //    // Round to nearest 2 decimal places
            //    _scaleValue = Mathf.Round(other.transform.position.y * 100f) / 10f;
            //    //Debug.Log($"_handHeight: {_scaleValue}");
            //    _scaleText.text = _scaleValue.ToString();


            //    // If the value has changed, call SetValue on the sliding scale sync component
            //    if (_scaleValue != _previousScaleValue)
            //    {
            //        //Debug.Log($"_scaleValue: {_scaleValue}, _previousScaleValue: {_previousScaleValue}");

            //        _slidingScaleSync.SetValue(_scaleValue);
            //        _previousScaleValue = _scaleValue;
            //    }
            //}
        }
    }
}