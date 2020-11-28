using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class SceneService
    {
        private Scene _scene;
        public SceneService(Scene scene)
        {
            _scene = scene;
        }

        public Vector3 GetScenePosition()
        {
            Vector3 position = new Vector3();

            switch (_scene)
            {
                case Scene.Scene1:
                    position = new Vector3(15f, 0f, -15f);
                    break;
                case Scene.Scene2:
                    position = new Vector3(15f, 0f, 0f);
                    break;
                case Scene.Scene3:
                    position = new Vector3(15f, 0f, 15f);
                    break;
                case Scene.Scene4:
                    position = new Vector3(0f, 0f, 15f);
                    break;
                case Scene.Scene5:
                    position = new Vector3(-15f, 0f, 0f);
                    break;
                case Scene.Scene6:
                    position = new Vector3(-15f, 0f, 15f);
                    break;
                case Scene.Scene7:
                    position = new Vector3(0f, 0f, -15f);
                    break;
                case Scene.Scene8:
                    position = new Vector3(-15f, 0f, -15f);
                    break;
            }

            return position;
        }

        //public Vector3 GetSelectionPanelPosition()
        //{
        //    Vector3 position = new Vector3(-1.38f, -1f, -2.76f);
        //    return position;
        //}

        //public Vector3 GetSceneLightsPosition()
        //{
        //    Vector3 position = new Vector3(0f, 0f, 0f);
        //    return position;
        //}
        //public Vector3 GetSceneAudioPosition()
        //{
        //    Vector3 position = new Vector3(-1.01f, 0.24f, -3.28f);
        //    return position;
        //}
    }
}
