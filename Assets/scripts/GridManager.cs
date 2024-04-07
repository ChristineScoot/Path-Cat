using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform mom;

    [SerializeField] private GameObject tapCanvas;
    [SerializeField] private GameObject goButton;
    [SerializeField] private Text scoreText;
    private Logic logic;
    private int shortestPath;

    private Dictionary<Vector2, Tile> tiles;
    private List<Vector2> directions;
    private List<Vector2> diagonalDirections;
    private int touchCount;

    void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x}{y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);

                tiles[new Vector2(x, y)] = spawnedTile;
            }
        }


        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        mom.transform.position = new Vector3((float)width - 1, (float)height - 1, -1);
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }

    void Start()
    {
        logic = new Logic();
        GenerateGrid();
        int playerScore = logic.getScore();
        scoreText.text = "Current score     " + playerScore.ToString();
    }

    public void onClickGoButton()
    {
        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(width - 1, height - 1);

        //calculate players path length
        int pathLength;
        pathLength = calculatePlayerPath(startPosition, endPosition) - 1;

        if (pathLength > 0)
        {
            //find shortest path and display it on the grid
            List<Vector2> path = calculatePath(startPosition, new Vector2(width - 1, height - 1));
            //display player's score
            if (pathLength > shortestPath)
            {
                logic.substractScore(pathLength);
                int playerScore = logic.getScore();
                scoreText.text = "Shorter path detected     " + playerScore.ToString();
                foreach (var p in path) //display possible shortest path
                {
                    GetTileAtPosition(p).setShortestPathCircle();
                }
                tapCanvas.SetActive(true);
                goButton.SetActive(false);
            }
            else //no change in score
            {
                scoreText.text = "PERFECT!     " + logic.getScore().ToString();
                tapCanvas.SetActive(true);
                goButton.SetActive(false);
            }
        }
    }
    private int calculatePlayerPath(Vector2 startVector, Vector2 endVector)
    {
        initialiseDirections();
        GetTileAtPosition(startVector).setPlayerPath(true);
        GetTileAtPosition(endVector).setPlayerPath(true);
        Vector2 current = startVector;
        int playerPathLength = 0;

        var visited = new List<Vector2>();
        while (current != null)
        {
            if (current == endVector)
                break;
            visited.Add(current);
            int neigbourCount = 0;
            Vector2 neighbour = current;
            Vector2 nextNeighbour = neighbour;
            foreach (var direction in directions)
            {
                neighbour = current + direction;
                if (visited.Contains(neighbour))
                    continue;
                if (!(neighbour.x >= width || neighbour.x < 0 || neighbour.y >= height || neighbour.y < 0) && GetTileAtPosition(neighbour).isPlayerPath())
                {
                    neigbourCount++;
                    nextNeighbour = neighbour;
                }
            }
            if (neigbourCount == 1)
            {
                playerPathLength++;
                current = nextNeighbour;
                continue;
            }
            else if (neigbourCount > 1)
            {
                scoreText.text = "Detected more than 1 path or a loop     " + logic.getScore();
                return 0;
            }

            foreach (var direction in diagonalDirections)
            {
                neighbour = current + direction;
                if (visited.Contains(neighbour))
                    continue;
                if (!(neighbour.x >= width || neighbour.x < 0 || neighbour.y >= height || neighbour.y < 0) && GetTileAtPosition(neighbour).isPlayerPath())
                {
                    neigbourCount++;
                    nextNeighbour = neighbour;
                }
                print(direction.x + " " + direction.y);
                print("neigbourCount = " + neigbourCount);
                if (GetTileAtPosition(neighbour) != null)
                {
                    print(neighbour.x + ", " + neighbour.y + " " + GetTileAtPosition(neighbour).isPlayerPath());
                }
            }
            if (neigbourCount == 1)
            {
                playerPathLength++;
                current = nextNeighbour;
                continue;
            }
            else if (neigbourCount > 1)
            {
                scoreText.text = "Detected more than 1 path or a loop     " + logic.getScore();
                return 0;
            }
            else if (neigbourCount == 0)
            {
                scoreText.text = "No path detected     " + logic.getScore();
                return 0;
            }
        }
        return playerPathLength;
    }
    private List<Vector2> calculatePath(Vector2 startVector, Vector2 endVector)
    {
        initialiseDirections();
        var path = new List<Vector2>();
        path.Add(startVector);
        path.Add(endVector);

        Node startNode = new Node(startVector, 0, null);
        var priorityQueue = new List<Node>();
        var visited = new List<Vector2>();
        priorityQueue.Add(startNode);
        while (priorityQueue.Count != 0)
        {
            priorityQueue = priorityQueue.OrderBy(x => x.minCostToStart).ToList();
            Node current = priorityQueue.First();
            priorityQueue.Remove(current);
            visited.Add(current.coord);
            if (current.coord == endVector)
            {
                //print(current.minCostToStart - 1);
                shortestPath = current.minCostToStart - 1;
                while (current.parent != null)
                {
                    path.Add(current.coord);
                    current = current.parent;
                }
                return path;
            }
            foreach (var neighbour in getNeighbours(current))
            {
                if (visited.Contains(neighbour.coord))
                    continue;
                if (!priorityQueue.Contains(neighbour))
                    priorityQueue.Add(neighbour);
                //GetTileAtPosition(neighbour.coord).setShortestPathCircle();
            }
        }

        return path;
    }

    private List<Node> getNeighbours(Node current)
    {
        //Vector2 neighbourRight = Vector2.Add(current.coord, directions[0]);
        var neighbours = new List<Node>();
        foreach (var direction in directions)
        {
            Vector2 neighbour = current.coord + direction;
            if (!(neighbour.x >= width || neighbour.x < 0 || neighbour.y >= height || neighbour.y < 0))
                neighbours.Add(new Node(neighbour, current.minCostToStart + 1, current));
        }

        foreach (var direction in diagonalDirections)
        {
            Vector2 neighbour = current.coord + direction;
            if (!(neighbour.x >= width || neighbour.x < 0 || neighbour.y >= height || neighbour.y < 0))
                neighbours.Add(new Node(neighbour, current.minCostToStart + 1, current));
        }
        return neighbours;
    }

    private void initialiseDirections()
    {
        directions = new List<Vector2>();
        directions.Add(new Vector2(1, 0));
        directions.Add(new Vector2(-1, 0));
        directions.Add(new Vector2(0, 1));
        directions.Add(new Vector2(0, -1));
        //diagonal
        diagonalDirections = new List<Vector2>();
        diagonalDirections.Add(new Vector2(1, 1));
        diagonalDirections.Add(new Vector2(1, -1));
        diagonalDirections.Add(new Vector2(-1, -1));
        diagonalDirections.Add(new Vector2(-1, 1));

    }

    public void onTapButton()
    {
        SceneManager.LoadSceneAsync("End Scene");
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }
}
