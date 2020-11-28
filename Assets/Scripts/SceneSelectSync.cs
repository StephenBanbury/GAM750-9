using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Normal.Realtime;
using UnityEngine;

namespace Assets.Scripts
{
    public class SceneSelectSync : RealtimeComponent
    {
        private SceneSelectSyncModel _model;
        private SceneSelect _sceneSelect;

        void Start()
        {
            _sceneSelect = GetComponent<SceneSelect>();
        }

        private SceneSelectSyncModel model
        {
            set
            {
                //Debug.Log("In SceneSelectSyncModel");

                if (_model != null)
                {
                    // Unregister from events
                    _model.sceneDidChange -= SceneDidChange;
                }

                // Store the model
                _model = value;

                if (_model != null)
                {
                    // Update scene to match the new model
                    UpdateScene();

                    // Register for events so we'll know if the value changes later
                    _model.sceneDidChange += SceneDidChange;
                }
            }
        }

        private void SceneDidChange(SceneSelectSyncModel model, Scene value)
        {
            UpdateScene();
        }

        private void UpdateScene()
        {
            if (_model != null && _model.scene > 0)
            {
                // Get the value from the model, display it 
                _sceneSelect.SetScene(_model.scene);

                //Debug.Log($"in UpdateScene: {_model.scene}");

                // TODO: this may not be needed, but if it is, maybe move it to its own manager class
                //MediaDisplayManager.instance.SelectedScreenFormation = (ScreenFormation) _model.formationId;
            }
        }

        public void SetScene(Scene scene)
        {
            // Set the value on the model
            // This will fire the valueChanged event on the model, which will update the value for both the local player and all remote players
            _model.scene = scene;
        }
    }
}
