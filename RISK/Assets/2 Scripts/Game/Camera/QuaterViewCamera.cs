using System.Collections;
using UnityEngine;

public class QuaterViewCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(10f, 10f, -10f);

    private void LateUpdate()
    {
        if (target == null)
        {
            GameObject localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
            if (localPlayer != null)
            {
                target = localPlayer.transform;
            }
        }

        if (target != null)
        {
            Vector3 newPosition = new Vector3(target.position.x, target.position.y, target.position.z) + offset;

            transform.position = newPosition;
            transform.LookAt(target.position);
        }

    }
}
