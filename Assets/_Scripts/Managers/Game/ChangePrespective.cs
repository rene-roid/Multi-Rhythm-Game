using System;
using UnityEngine;
using _Scripts.Managers.Camera;
using _Scripts.Units.Player;

namespace _Scripts.Managers.Game
{
    public class ChangePrespective : MonoBehaviour
    {
        public PlayerController player;
        public PerspectiveSwitcher perspectiveSwitcher;
        public GameObject orthoScene;
        public GameObject perspectiveScene;

        private bool isOrtho = false;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (isOrtho)
                {
                    ChangePerspective();
                    isOrtho = false;
                }
                else  if (!isOrtho)
                {
                    ChangeOrtho();
                    isOrtho = true;
                }
            }
        }

        public void ChangeOrtho()
        {
            perspectiveSwitcher.SetOrtho();
            orthoScene.SetActive(true);
            player.SwitchPlayerMode(PlayerMode.Runner);
            
            perspectiveScene.SetActive(false);
            isOrtho = true;
        }
        
        public void ChangePerspective()
        {
            perspectiveSwitcher.SetPerspective();
            orthoScene.SetActive(false);
            player.SwitchPlayerMode(PlayerMode.Dodge);
            
            perspectiveScene.SetActive(true);
            isOrtho = false;
        }
    }
}
