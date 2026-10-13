using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mazecell : MonoBehaviour
{
    // maze generator: this code is para ma knock down ung mga wall to make a maze
   [SerializeField]
   private GameObject _leftWall;
   
   [SerializeField]
   private GameObject _rightWall;

   [SerializeField]
   private GameObject _frontWall;

   [SerializeField]
   private GameObject _backWall;

   [SerializeField]
   private GameObject _unvisitedBlock;

    //bool property keep track whether the cell has visited by the maze generator
    public bool IsVisited {get; private set;}

    // helper methods: code for one call visit
    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);
    }

    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        _backWall.SetActive(false);
    }

}
