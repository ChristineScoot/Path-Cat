using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic// : MonoBehaviour
{
    public int playerScore;

    public Logic()
    {
        playerScore = 100;
    }

    public void substractScore(int sub)
    {
        playerScore -= sub;
    }

    public int getScore()
    {
        return playerScore;
    }
}
