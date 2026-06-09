using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingLight : MonoBehaviour
{
    public Transform transform;
    [HideInInspector] public float startYPosition;
    
    public float speed = 1.5f;
    public float moveAmplitude = 0.2f;

    public bool startDifferenceRandom = true;
    /*[HideInInspector]*/ public float startDifference = 0;
}
