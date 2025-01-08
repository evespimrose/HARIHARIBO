using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterPanel : MonoBehaviour
{
    public Button deleteButton;
    public Button closeButton;

    public Button selectButton;
    public Image characterModelImage;

    public Transform characterListTransform;

    public GameObject characterDataPrefab;
    public Button characterCreatePrefab;
}
