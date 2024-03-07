using _Scripts.Managers.Spawners;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using _Scripts.Managers.Camera;
using _Scripts.Managers.Game;

namespace _Scripts.Managers.Level_Controller
{
    // This class controls the level creation and management
    public class LevelController : MonoBehaviour
    {
        // This is the duration of the level in seconds
        public float levelDuration = 60f;
        // This is the current time in the level
        private float currentTime;

        // This is the current level
        public Level level;

        public DodgeSpawner dodgeSpawner;
        public RunnerSpawner runnerSpawner;
        public GameManager gameManager;
        
        
        // This method is called at the start of the level
        void Start()
        {
            SetupLevel();
        }

        // This method is called every frame
        void Update()
        {
            currentTime += Time.deltaTime;

            InstanceBlocks();
        }

#region Level Instance
        private Level currentLevel;

        private void SetupLevel() {
            currentLevel = level;
            currentTime = 0f;
        }

        private void InstanceBlocks() {
            List<LevelEvents> eventsToRemove = new List<LevelEvents>();

            foreach (var levelEvent in currentLevel.levelEvents) {
                // exit if the event time is greater than the current time
                if (levelEvent.time > currentTime)
                    break;

                if (levelEvent.time > currentTime)
                    continue;

                foreach (var block in levelEvent.blockProperties) {
                    SpawnBlock(block);
                    eventsToRemove.Add(levelEvent);
                }

                UpdatePrespective(levelEvent);
            }

            foreach (var levelEvent in eventsToRemove) {
                currentLevel.levelEvents.Remove(levelEvent);
            }
            

            void SpawnBlock(BlockProperties block) {
                switch (block.blockType) {
                    case BlockType.DirectionUp:
                        runnerSpawner.SpawnBlock(0);
                        break;
                    case BlockType.DirectionDown:
                        runnerSpawner.SpawnBlock(1);
                        break;
                    case BlockType.Dodge1:
                    case BlockType.Dodge2:
                    case BlockType.Dodge3:
                    case BlockType.Dodge4:
                    case BlockType.Dodge5:
                        dodgeSpawner.SpawnBlock(block.spawnPosition);
                        break;
                }
            }

            void UpdatePrespective(LevelEvents levelEvent) {
                switch (levelEvent.levelType) {
                    case LevelType.Direction:
                        gameManager.ChangeOrtho();
                        break;
                    case LevelType.Dodge:
                        gameManager.ChangePrespective();
                        break;
                }
            }
        }

#endregion

#region OLD LEVEL IDK
        // This method inserts a new block into the level
        private int InsertNewBlock(BlockType blockType, int spawnPosition, float speed)
        {
            // Create a new block with the given properties
            BlockProperties blockProperties = new BlockProperties(blockType, spawnPosition, speed);
            // Check the level type of the block
            LevelType levelType = CheckBlockToLevel(blockType);
            
            // If the level type is null, return 1
            if (levelType == LevelType.Null)
                return 1;

            // Loop through all the level events
            foreach (var levelEvent in level.levelEvents)
            {
                // If the time of the event is the current time
                if (levelEvent.time == currentTime)
                {
                    // If the level type of the event is the same as the block's level type
                    if (levelType == levelEvent.levelType)
                    {
                        // Add the block to the event and return 0
                        levelEvent.AddBlock(blockProperties);
                        return 0;
                    }
                    else
                        // If the level type of the event is not the same as the block's level type, return 1
                        return 1;
                }
            }
            
            // If no event was found for the current time, create a new event and add the block to it
            LevelEvents newEvent = new LevelEvents(currentTime, levelType);
            newEvent.AddBlock(blockProperties);
            
            return 0;
        }
        
        // This method removes a block from the level
        private int RemoveBlock(BlockType blockType)
        {
            // Currently not implemented, returns 1
            return 1;
        }
        
        // This method inserts a new event into the level
        private int InsertNewEvent(LevelType levelType)
        {
            // Loop through all the level events
            foreach (var levelEvent in level.levelEvents)
            {
                // If the time of the event is the current time, return 1
                if (levelEvent.time == currentTime)
                    return 1;
            }
            
            // If no event was found for the current time, create a new event
            LevelEvents newEvent = new LevelEvents(currentTime, levelType);
            level.levelEvents.Add(newEvent);
            
            return 0;
        }
        
        // This method checks the level type of a block
        private LevelType CheckBlockToLevel(BlockType blockType)
        {
            // Depending on the block type, return the corresponding level type
            switch (blockType)
            {
                case BlockType.DirectionUp:
                case BlockType.DirectionDown:
                    return LevelType.Direction;
                case BlockType.Dodge1:
                case BlockType.Dodge2:
                case BlockType.Dodge3:
                case BlockType.Dodge4:
                case BlockType.Dodge5:
                    return LevelType.Dodge;
            }
            
            // If the block type is not recognized, return null
            return LevelType.Null;
        }
#endregion    
    }
}