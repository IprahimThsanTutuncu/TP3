using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    // Singleton
    public static UiManager Instance { get; private set; }
    
    // Seroalized field list of a definite length defined in an int
    [SerializeField] private List<GameObject> _pvImages;
    
    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void UpdatePv(int pv)
    {
        _pvImages[pv].SetActive(false);
    }
}
