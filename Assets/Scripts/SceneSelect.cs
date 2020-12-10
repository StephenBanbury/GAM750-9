//using System.Collections;
//using System.Collections.Generic;
//using Assets.Scripts;
//using Assets.Scripts.Enums;
//using UnityEngine;

//public class SceneSelect : MonoBehaviour
//{

//    private Scene _scene;
//    private Scene _previousScene;

//    private SceneSelectSync _sceneSelectSync;

//    void Start()
//    {
//        _sceneSelectSync = gameObject.GetComponent<SceneSelectSync>();
//    }


//    public void SetScene(Scene scene)
//    {
//        _scene = scene;

//        if (_scene != Scene.None && _scene != _previousScene)
//        {
//            //_formationIdText.text = _formationId.ToString();
//            //MediaDisplayManager.instance.TweenScreens((ScreenFormation)_scene);
//            MediaDisplayManager.instance.MyCurrentScene = _scene;
//        }
//    }
//    public void KeepInSync()
//    {
//        // If the id has changed, call SetScene on the sync component
//        if (_scene != _previousScene)
//        {
//            _sceneSelectSync.SetScene(_scene);
//            _previousScene = _scene;
//        }
//    }
//}
