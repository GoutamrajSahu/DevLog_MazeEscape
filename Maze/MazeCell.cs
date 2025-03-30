using UnityEngine;
using TMPro;
/* 
    ------------------- DreamGSoft -------------------

    Thank you for checking out the code! I hope you find it useful in your projects. Enjoy!

    --------------------------------------------------
 */
namespace DreamGSoft.ProceduralMazeGeneation
{
    public class MazeCell : MonoBehaviour
    {
        [Header("Cell Parts")]
        [SerializeField] GameObject leftWall;
        [SerializeField] GameObject rightWall;
        [SerializeField] GameObject frontWall;
        [SerializeField] GameObject backWall;
        [SerializeField] GameObject unvisitedBlock;

        [Header("Coordinates")]
        [SerializeField] public int X;
        [SerializeField] public int Z;

        [Header("Debugs")]
        [SerializeField] public TextMeshProUGUI coordinatesDebug;
        [SerializeField] public TextMeshProUGUI openCloseDebug;

        [Header("Values")]
        public bool isVisited { get; private set; }

        public void SetCoordinates(int X, int Z) //Only use while creating the cell.
        {
            this.X = X;
            this.Z = Z;
            coordinatesDebug.text = $"x:({X}, {Z})";
            openCloseDebug.text = "<color=red>Closed</color>";
        }
        public void Visit()
        {
            isVisited = true;
            unvisitedBlock.SetActive(false);
            openCloseDebug.text = "<color=green>Open</color>";
        }

        public void ClearWall(CellWalls wall)
        {
            switch (wall)
            {
                case CellWalls.left:
                    leftWall.SetActive(false);
                    break;
                case CellWalls.right:
                    rightWall.SetActive(false);
                    break;
                case CellWalls.front:
                    frontWall.SetActive(false);
                    break;
                case CellWalls.back:
                    backWall.SetActive(false);
                    break;
            }
                
        }
    }

    public enum CellWalls
    {
        left, right, front, back
    }
}
