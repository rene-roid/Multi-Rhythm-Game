using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Managers.Level_Controller
{
    public enum BlockType
    {
        DirectionUp,
        DirectionDown,
        
        Dodge1,
        Dodge2,
        Dodge3,
        Dodge4,
        Dodge5,
    }
    
    public enum LevelType
    {
        Dodge,
        Direction,
        Both,
        Null
    }
    
    [System.Serializable]
    public class Level
    {
        public List<LevelEvents> levelEvents;
    }

    [System.Serializable]
    public class BlockProperties
    {
        public BlockType blockType;
        public int spawnPosition;
        public float speed;
        
        public BlockProperties(BlockType blockType, int spawnPosition, float speed)
        {
            this.blockType = blockType;
            this.spawnPosition = spawnPosition;
            this.speed = speed;
        }
    }
    
    [System.Serializable]
    public class LevelEvents
    {
        public float time;
        public List<BlockProperties> blockProperties;
        public LevelType levelType = LevelType.Null;

        public LevelEvents(float time, List<BlockProperties> blockProperties, LevelType levelType)
        {
            this.time = time;
            this.blockProperties = blockProperties;
            this.levelType = levelType;
        }
        
        public LevelEvents(float time, List<BlockProperties> blockProperties)
        {
            this.time = time;
            this.blockProperties = blockProperties;
        }
        
        public LevelEvents(float time, LevelType levelType)
        {
            this.time = time;
            this.levelType = levelType;

            blockProperties = new List<BlockProperties>();
        }
        
        public LevelEvents(float time)
        {
            this.time = time;

            blockProperties = new List<BlockProperties>();
        }
        
        public void AddBlock(BlockProperties block)
        {
            blockProperties.Add(block);
        }
    }
}
