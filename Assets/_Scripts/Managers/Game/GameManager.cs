using UnityEngine;
using _Scripts.Managers.Camera;

namespace _Scripts.Managers.Game {    
    public class GameManager : MonoBehaviour
    {
        public ChangePrespective changePrespective;
        public PerspectiveSwitcher prespectiveSwitcher;

        private bool isOrtho = false;

        void Start()
        {
        }
        
        void Update()
        {
            
        }

        public void ChangePrespective()
        {
            if (!isOrtho) return;
            isOrtho = false;
            changePrespective.ChangePerspective();
            prespectiveSwitcher.SetPerspective();
        }

        public void ChangeOrtho()
        {
            if (isOrtho) return;
            isOrtho = true;
            changePrespective.ChangeOrtho();
            prespectiveSwitcher.SetOrtho();
        }
    }
}
