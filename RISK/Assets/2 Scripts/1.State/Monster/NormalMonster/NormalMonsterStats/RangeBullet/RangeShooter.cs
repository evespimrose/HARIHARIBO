using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shotPos;
    
    public GameObject BulletSpwan()
    {
        GameObject monsterBullet = PhotonNetwork.Instantiate(bulletPrefab.name, shotPos.transform.position, shotPos.rotation);
        return monsterBullet;
    }
}
