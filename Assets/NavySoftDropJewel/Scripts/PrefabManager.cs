using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;


    public List<GameObject> blocks = new List<GameObject>();
    public GameObject textEffectPrefab ;
    private void Awake()
    {
        instance = this;
    }
}
