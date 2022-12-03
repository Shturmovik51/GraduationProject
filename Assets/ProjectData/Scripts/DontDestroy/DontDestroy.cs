using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [SerializeField] public string ObjectID { get; private set; }

    private void Awake()
    {
        ObjectID = name + transform.position.ToString();
    }

    private void Start()
    {
        for (int i=0; i < FindObjectsOfType<DontDestroy>().Length; i++)
        {
            if (FindObjectsOfType<DontDestroy>()[i] != this)
            {
                if (FindObjectsOfType<DontDestroy>()[i].ObjectID != ObjectID)
                {
                    Destroy(gameObject);
                }
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}
