using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class SlidingScaleSyncTest : MonoBehaviour
    {
        [SerializeField] private float _scaleValue;
        private float _previousScaleValue;

        private SlidingScaleSync _slidingScaleSync;

        void Start()
        {
            // Get a reference to the sliding scale sync component
            _slidingScaleSync = GetComponent<SlidingScaleSync>();
        }

        void Update()
        {
            // If the value has changed, call SetValue on the sliding scale sync component
            if (_scaleValue != _previousScaleValue)
            {
                _slidingScaleSync.SetValue(_scaleValue);
                _previousScaleValue = _scaleValue;
            }
        }
    }
}