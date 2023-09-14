using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAmbush : MonoBehaviour
{
    [SerializeField]
    private GameObject _primaryLaserPrefab;
    [SerializeField]
    private GameObject _ambushTargetIndicator;
    [SerializeField]
    private GameObject _ambushLaser;
    [SerializeField]
    private AudioClip _primaryLaserClip;
    [SerializeField]
    private Vector3 _laserOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
