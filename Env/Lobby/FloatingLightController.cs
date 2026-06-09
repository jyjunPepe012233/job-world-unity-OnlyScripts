using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FloatingLightController : MonoBehaviour
{ 
    [SerializeField] private List<FloatingLight> floatingRightObjects;
    
    private float elapsedTime = 0f;

    private void Awake()
    {
        FloatingLight iFloatingLight;
        
        for (int i = 0; i < floatingRightObjects.Count; i++)
        {
            iFloatingLight = floatingRightObjects[i];
            
            iFloatingLight.startYPosition = iFloatingLight.transform.position.y;
            if (iFloatingLight.startDifferenceRandom)
            {
                iFloatingLight.startDifference = Random.Range(0f, 2f * Mathf.PI);
            }
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        FloatingLight iFloatingLight;
        for (int i = 0; i < floatingRightObjects.Count; i++)
        {
            iFloatingLight = floatingRightObjects[i];
            iFloatingLight.transform.position = new Vector3(iFloatingLight.transform.position.x , 
                iFloatingLight.startYPosition + Mathf.Sin(elapsedTime * iFloatingLight.speed + iFloatingLight.startDifference) * iFloatingLight.moveAmplitude
                , iFloatingLight.transform.position.z);
        }
    }
}
