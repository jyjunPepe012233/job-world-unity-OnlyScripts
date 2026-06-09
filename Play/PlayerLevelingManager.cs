using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jobworld
{
    public class PlayerLevelingManager : MonoBehaviour
    {
        private List<ILevelingObject> m_levelingObjects = new List<ILevelingObject>();
        
        public event Action<string> levelUpdated;
        public event Action levelingObjectsChanged;
        
        public ILevelingObject[] levelingObjects => m_levelingObjects.ToArray();
        
        public void AddLevelingObject(ILevelingObject instance)
        {
            if (instance == null || instance.levelingSetting == null) return;
            
            if (!m_levelingObjects.Contains(instance))
            {
                Debug.Log("Adding Leveling Object: " + instance.levelingSetting.Id);
                m_levelingObjects.Add(instance);
                levelingObjectsChanged?.Invoke();
            }
        }
        
        public void RemoveLevelingObject(ILevelingObject instance)
        {
            m_levelingObjects.Remove(instance);
            levelingObjectsChanged?.Invoke();
        }
        
        public void SetLevel(string id, int value)
        {
            if (string.IsNullOrEmpty(id)) return;
            
            var targetObject = m_levelingObjects.FirstOrDefault(obj => 
                obj?.levelingSetting != null && obj.levelingSetting.Id == id);
            if (targetObject != null)
            {
                targetObject.SetLevel(value);
                levelUpdated?.Invoke(id);
            }
        }
    }
}