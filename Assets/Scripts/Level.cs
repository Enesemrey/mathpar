using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public LevelPiece[] Prefabs;
    public PieceTypes[] LevelPieces = new PieceTypes[] { };

    public List<LevelPiece> Instances;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
public enum PieceTypes
{
    Normal, Short, Gap, Circular, Obstacle,Finish, NULL
}