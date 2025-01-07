using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("무기 설정")]
    [SerializeField]
    private GameObject weaponPrefab;
    [SerializeField]
    private Transform weaponSocket;

    private GameObject currentWeapon;

    private void Start()
    {
        EquipWeapon();
    }

    public void EquipWeapon()
    {
        if (weaponPrefab != null && weaponSocket != null)
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            currentWeapon = Instantiate(weaponPrefab,weaponSocket);
        }
    }
    public void UnequipWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy (currentWeapon);
            currentWeapon = null;
        }
    }
}
