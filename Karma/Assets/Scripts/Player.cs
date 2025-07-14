using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Data")]
    [HideInInspector] public string heldItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        heldItem = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
