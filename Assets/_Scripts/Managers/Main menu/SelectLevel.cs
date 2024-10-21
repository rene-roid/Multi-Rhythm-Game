using System;
using UnityEngine;

namespace _Scripts.Managers.Main_menu
{
    public class SelectLevel : MonoBehaviour
    {
        public GameObject[] selectors;
        public Transform playerTransform; // Reference to the player's transform

        public int currentLevel = 0;
        public float positionTolerance = 0.5f; // Tolerance for detecting the player's position
        public float scaleUpTime = 3f; // Time to scale up fully to target
        private float[] levelPositions = { -4f, -2f, 0f, 2f, 4f }; // X positions for levels
        
        [Header("Levels")]
        public string select1Name = "Game";

        private void Update()
        {
            CheckPlayerPosition();
        }

        private void FixedUpdate()
        {
            SelectingLevel();
        }

        private void CheckPlayerPosition()
        {
            float playerX = playerTransform.position.x;

            // Find the closest level based on player position
            for (int i = 0; i < levelPositions.Length; i++)
            {
                if (Mathf.Abs(playerX - levelPositions[i]) <= positionTolerance)
                {
                    currentLevel = i;
                    return;
                }
            }
            
            // Default to level 2 if no match is found
            currentLevel = 2;
        }

        private void SelectingLevel()
        {
            for (int i = 0; i < levelPositions.Length; i++)
            {
                var selector = selectors[i];
                
                // Scale up the selector for the current level
                if (i == currentLevel)
                {
                    selector.transform.localScale = Vector3.MoveTowards(selector.transform.localScale, new Vector3(selector.transform.localScale.x, selector.transform.localScale.y, 59), Time.deltaTime * scaleUpTime);
                    // If progress is complete, load the selected level
                    if (selector.transform.localScale.z >= 58)
                    {
                        switch (currentLevel)
                        {
                            case 0:
                                UnityEngine.SceneManagement.SceneManager.LoadScene(select1Name);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                    }
                }
                else
                {
                    selector.transform.localScale = Vector3.MoveTowards(selector.transform.localScale, new Vector3(selector.transform.localScale.x, selector.transform.localScale.y, 3), Time.deltaTime * scaleUpTime);
                }
            }
        }
    }
}
