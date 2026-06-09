
using UnityEngine;

namespace Jobworld
{
    public class EquipmentSlot : MonoBehaviour
    {
        public Tool item;

        private void Start()
        {
            if (item != null)
            { 
                item.SetInitialTransform(transform);
            }
        }

        public void ResetItemTransform()
        {
            if (item != null)
            {
                item.ResetToInitialTransform();
            }
        }
    }
}
