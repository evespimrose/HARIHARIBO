using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shotPos;
    
    public GameObject BulletSpwan()
    {
        GameObject monsterBullet = Instantiate(bulletPrefab, shotPos.transform.position, shotPos.rotation);
        return monsterBullet;
    }
}
