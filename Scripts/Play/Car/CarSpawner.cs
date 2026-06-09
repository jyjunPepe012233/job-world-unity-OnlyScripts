using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint; // 스폰 위치

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            GameObject car = PhotonNetwork.Instantiate("NetworkedCar", spawnPoint.position, spawnPoint.rotation);
        }
    }
}
