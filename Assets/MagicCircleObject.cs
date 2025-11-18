using DTT.AreaOfEffectRegions;
using System;
using UnityEngine;

public class MagicCircleObject : MonoBehaviour
{
    CircleRegion circleRegion;

    private bool isActive = false;
    private bool done = false;

    public bool Done { get => done; set => done = value; }

    void Awake()
    {
        circleRegion = GetComponentInChildren<CircleRegion>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive && !Done)
        {
            circleRegion.FillProgress += Time.deltaTime * 0.25f;
            if (circleRegion.FillProgress >= 1f && !Done)
            {
                Done = true;
                // Do something when the circle is fully filled
                Debug.Log("Circle is fully filled!");
            }
        }
        else if (!isActive && !Done)
        {
            circleRegion.FillProgress -= Time.deltaTime * 0.25f;
        }
    }

    // Enter과 Out을 활용하면 더 나으려나? 
    public void Stay(Collider other)
    {
        other.TryGetComponent(out PlayerManager player);
        if (player == null)
            return;
        isActive = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    internal void OnPlayerEnter(PlayerManager player)
    {
        throw new NotImplementedException();
    }

    internal void OnPlayerExit(PlayerManager player)
    {
        throw new NotImplementedException();
    }
}
