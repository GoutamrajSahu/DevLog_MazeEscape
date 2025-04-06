using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/* 
    ------------------- DreamGSoft -------------------

    Hi, This is Goutamraj, Thank you for Checking the code! I hope you find it useful. Enjoy!

    --------------------------------------------------
 */
namespace DreamGSoft.ProceduralMazeGeneation
{
    public class MazeGenerator : MonoBehaviour
    {
        [SerializeField] private MazeCell mazeCellPrefab;
        [SerializeField] private Transform mazeHolder;

        [Header("Maze Length and Width")]
        [SerializeField] private int mazeWidth;
        [SerializeField] private int mazeLength;
        
        private MazeCell[,] mazeGrid;

        private void Start()
        {
            mazeGrid = new MazeCell[mazeWidth, mazeLength];

            for(int x = 0; x < mazeWidth; x++)
            {
                for(int z = 0; z < mazeLength; z++)
                {
                    MazeCell cell = Instantiate(mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity); //Note: the positio is not dynamic for diffirent cell size, currenly considering cell size as 1 unit in zise, incase of increment or decrement of side, we have to multiply or divide the size for position. if 2 unit then "Vector3(x, 0, z) * 2", if 0.5 unit then "Vector3(z, 0, z) * 0.5".
                    cell.SetCoordinates(x, z);
                    mazeGrid[x, z] = cell;
                } 
            }

            StartCoroutine(GenerateMaze(null, GetACell(0, 0)));
        }

        //Getting a requested cell.
        public MazeCell GetACell(int x, int z)
        {
            return mazeGrid[x, z];
        }

        private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
        {
            currentCell.Visit();
            ClearWalls(previousCell, currentCell);

            yield return new WaitForSeconds(0.1f);

            while (true)
            {
                MazeCell nextCell = GetANewUnVisitedCell(currentCell);

                if (nextCell != null)
                {
                    yield return GenerateMaze(currentCell, nextCell); //<= This is suitable: because The current coroutine pauses and resumes only after the nested coroutine (GenerateMaze) finishes. Creates a sequential, depth-first recursion flow.

                    //StartCoroutine(GenerateMaze(currentCell, nextCell)); //<= This is Not suitable: Because If StartCoroutine is used recursively, multiple coroutines will run concurrently, which may lead to unexpected behavior in a depth-first search context.
                }
                else
                { 
                    yield break;
                }
            }
        }

        private MazeCell GetANewUnVisitedCell(MazeCell currentCell)
        {
            List<MazeCell> unvisitedNeighbours = GetUnvisitedNeighbourCells(currentCell);
            if(unvisitedNeighbours.Count == 0)
            {
                return null;
            }
            else
            {
                return unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
            }
        }

        private  List<MazeCell> GetUnvisitedNeighbourCells(MazeCell currentCell)
        {
            int x = (int)currentCell.X;
            int z = (int)currentCell.Z;

            List<MazeCell> unvisitedNeighbours = new List<MazeCell>();

            if (!CheckBound(x, z)) return null;
            
            //LeftCell
            if(CheckBound(x + 1, z))
            {
                MazeCell cell = GetACell(x + 1, z);
                if (!cell.isVisited)
                {
                    unvisitedNeighbours.Add(cell);
                }
            }
            //RightCell
            if (CheckBound(x - 1, z))
            {
                MazeCell cell = GetACell(x - 1, z);
                if (!cell.isVisited)
                {
                    unvisitedNeighbours.Add(cell);
                }
            }
            //ForwardCell
            if (CheckBound(x, z + 1))
            {
                MazeCell cell = GetACell(x, z + 1);
                if (!cell.isVisited)
                {
                    unvisitedNeighbours.Add(cell);
                }
            }
            //BackwardCell
            if (CheckBound(x, z - 1))
            {
                MazeCell cell = GetACell(x, z - 1);
                if (!cell.isVisited)
                {
                    unvisitedNeighbours.Add(cell);
                }
            }
            return unvisitedNeighbours;
        }

        //Used to make sure the cell is under the bound of x/width and z/length
        public bool CheckBound(int x, int z)
        {
            //x is greater then or Equal to 0, and less than maze Width and z is greater then or Equal to 0, and less than maze Length. 
            if ((x >= 0 && x < mazeWidth) && (z >= 0 && z < mazeLength))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //This function clears the walls in between 2 cells. 
        private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
        {
            if (previousCell == null)
                return;

            //Checking directing and clearing the walls accordingly

            //Left->Right
            if (previousCell.X < currentCell.X)
            {
                previousCell.ClearWall(CellWalls.right);
                currentCell.ClearWall(CellWalls.left);
                return;
            }

            //Right->Left
            if (previousCell.X > currentCell.X)
            {
                previousCell.ClearWall(CellWalls.left);
                currentCell.ClearWall(CellWalls.right);
                return;
            }

            //Back->Front
            if (previousCell.Z < currentCell.Z)
            {
                previousCell.ClearWall(CellWalls.front);
                currentCell.ClearWall(CellWalls.back);
                return;
            }

            //Front->Back
            if (previousCell.Z > currentCell.Z)
            {
                previousCell.ClearWall(CellWalls.back);
                currentCell.ClearWall(CellWalls.front);
                return;
            }
        }
    }


}
