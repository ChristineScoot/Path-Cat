using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor, colour;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject shortestPathCircle;
    bool mouseClicked = false;
    bool playerPath = false;

    public void Init(bool isOffset)
    {
        colour = isOffset ? offsetColor : baseColor;
        _renderer.color = colour;
        playerPath = false;
    }

    void OnMouseDown()
    {
        mouseClicked = !mouseClicked;
        if (gridManager.GetTileAtPosition(new Vector2(0, 0)) == this || gridManager.GetTileAtPosition(new Vector2(gridManager.getWidth() - 1, gridManager.getHeight() - 1)) == this)
        {
            return;
        }
        _renderer.color = mouseClicked ? new Color(0.588f, 0.427f, 0.145f) : colour;
        playerPath = !playerPath;
    }

    public void setShortestPathCircle()
    {
        shortestPathCircle.SetActive(true);
    }

    public void setPlayerPath(bool isPlayerPath)
    {
        playerPath = isPlayerPath;
    }

    public bool isPlayerPath()
    {
        return playerPath;
    }
}
