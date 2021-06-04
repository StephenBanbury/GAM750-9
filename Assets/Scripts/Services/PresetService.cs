using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class PresetService
    {
        private List<ScreenDisplayPreset> _screenDisplayPresets;

        public PresetService()
        {
            _screenDisplayPresets = new List<ScreenDisplayPreset>();
        }

        public List<ScreenDisplayPreset> Test()
        {
            var mediaScreenAssignStates = new List<MediaScreenAssignState>();

            // test preset
            for (int i = 1; i <= 16; i++)
            {
                mediaScreenAssignStates.Add(
                    new MediaScreenAssignState
                    {
                        MediaId = i,
                        MediaTypeId = (int)MediaType.VideoClip,
                        ScreenDisplayId = i
                    }
                );
            }

            var preset = new ScreenDisplayPreset
            {
                Id = 1,
                MediaScreenAssignStates = mediaScreenAssignStates
            };

            _screenDisplayPresets.Add(preset);

            Debug.Log($"test: {_screenDisplayPresets[0].MediaScreenAssignStates.Count}");

            return _screenDisplayPresets;
        }

    }
}
