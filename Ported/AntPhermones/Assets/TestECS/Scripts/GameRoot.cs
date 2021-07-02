using System;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public bool useECS = false;

    public bool useJobSystem = false;
        
    public Mesh mesh;
    public Material mat;
    [SerializeField] private int maxCount = 100;

    public static GameRoot Instance => _instance;

    private static GameRoot _instance = null;

    private MainBase _mainBase = null;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        if (useECS)
        {
            _mainBase = new MainECS();
        }
        else
        {
            useJobSystem = false;
            _mainBase = new MainMono();
        }

        _mainBase.CreateObject(mat, maxCount,mesh);
    }

    private void Update()
    {
        if (_mainBase != null)
        {
            _mainBase.Update();
        }
    }
}