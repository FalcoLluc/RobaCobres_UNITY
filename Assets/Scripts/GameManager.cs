using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;


public class GameManager : MonoBehaviour
{
    private BoardManager boardScript;
    private int level = 3;

    void Awake()
    {
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        boardScript.SetupScene(level);
    }

    void Update() { }

}
