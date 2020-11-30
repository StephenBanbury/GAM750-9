using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class FormationSelect : MonoBehaviour
    {
        //[SerializeField] private Text _formationIdText;

        //private int _formationId;
        private int _previousId;
        private int _scene;
        
        private FormationSelectSync _formationSelectSync;


        void Start()
        {
            _formationSelectSync = gameObject.GetComponent<FormationSelectSync>();
        }

        public void SetFormationId(int id, int animationSeconds)
        {
            //_formationId = id;

            if (id > 0 && id != _previousId)
            {
                MediaDisplayManager.instance.TweenScreens((ScreenFormation)id, animationSeconds);
            }
        }

        public void SetFormationId(Scene scene, int id, int animationSeconds)
        {
            //_formationId = id;
            //_scene = (int) scene;

            if (id > 0 && id != _previousId)
            {
                MediaDisplayManager.instance.TweenScreens(scene, (ScreenFormation)id, animationSeconds);
            }
        }

        public void KeepInSync(int id)
        {
            // generate cross-scene id

            // create id in 'composite' form, e.g. 12 = scene 1, formation 2.

            string scenePlusFormation = $"{_scene}{id}";
            int compoundId = int.Parse(scenePlusFormation);

            if (compoundId != _previousId)
            {
                _formationSelectSync.SetId(compoundId);
                _previousId = compoundId;
            }
        }
    }
}