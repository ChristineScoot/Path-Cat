using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node// : MonoBehaviour
{
    public Vector2 coord;
    public int minCostToStart;
    public Node parent;

    public Node(Vector2 coord, int minCostToStart, Node parent)
    {
        this.coord = coord;
        this.minCostToStart = minCostToStart;
        this.parent = parent;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
