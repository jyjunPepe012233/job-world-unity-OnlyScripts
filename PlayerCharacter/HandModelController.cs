using UnityEngine;
using UnityEngine.Serialization;

public class HandModelController : MonoBehaviour
{
    [SerializeField] private Transform[] m_models; // 여러 손 모델(메시나 오브젝트)
    
    public void SetModelActive(bool value)
    {
        foreach (var model in m_models)
        {
            if (model != null)
                model.gameObject.SetActive(value);
        }
    }
}