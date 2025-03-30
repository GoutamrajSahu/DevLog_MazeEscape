For Maze:
You will receive two prefabs, MazeGenerator and MazeCell. Attach the required scripts to each prefab. Place the MazeGenerator prefab in the Scene Hierarchy, and assign the MazeCell prefab to the MazeGenerator component through the Inspector, along with all other necessary content.
After that you can use it as you want.


Class Structure
Class Name
MazeGenerator

Fields
Serialized Fields
MazeCell mazeCellPrefab

Prefab for individual maze cells.

Transform mazeHolder

Parent transform to hold the maze cells in the hierarchy.

int mazeWidth

Width of the maze grid.

int mazeLength

Length of the maze grid.

Private Fields
MazeCell[,] mazeGrid

2D array representing the maze grid structure.

Methods
1. void Start()
Initializes the maze grid and begins the maze generation process.

Key Steps
Grid Initialization:

Instantiates MazeCell prefabs for every cell in the grid based on mazeWidth and mazeLength.

Places cells in the correct position and stores them in the mazeGrid array.

MazeCell cell = Instantiate(mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity); cell.SetCoordinates(x, z); mazeGrid[x, z] = cell; 
Maze Generation:

Starts the maze generation using a coroutine.

Initial cell: (0, 0).

StartCoroutine(GenerateMaze(null, GetACell(0, 0))); 
2. MazeCell GetACell(int x, int z)
Retrieves the maze cell at the specified coordinates (x, z).

3. IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
Recursive coroutine that generates the maze using depth-first search.

Key Steps
Marks the currentCell as visited.

currentCell.Visit(); 
Clears the wall between previousCell and currentCell.

ClearWalls(previousCell, currentCell); 
Waits for a short duration to visualize the generation.

yield return new WaitForSeconds(0.1f); 
Recursively generates the maze for unvisited neighboring cells.

MazeCell nextCell = GetANewUnVisitedCell(currentCell); if (nextCell != null) {     yield return GenerateMaze(currentCell, nextCell); } else {     yield break; } 
4. MazeCell GetANewUnVisitedCell(MazeCell currentCell)
Finds and returns a random unvisited neighboring cell of currentCell.

Process
Uses GetUnvisitedNeighbourCells to get all unvisited neighbors.

Selects one randomly using Random.Range.

Returns null if there are no unvisited neighbors.

5. List<MazeCell> GetUnvisitedNeighbourCells(MazeCell currentCell)
Retrieves a list of unvisited neighbors for a given cell.

Process
Extracts the cell’s coordinates.

Checks each neighboring direction (left, right, forward, backward).

Adds unvisited neighbors to a list.

Ensures neighbors are within grid bounds using CheckBound.

6. bool CheckBound(int x, int z)
Verifies if the coordinates (x, z) are within the grid bounds.

Logic
if ((x >= 0 && x < mazeWidth) && (z >= 0 && z < mazeLength)) {     return true; } else {     return false; } 
7. void ClearWalls(MazeCell previousCell, MazeCell currentCell)
Removes walls between two neighboring cells to create a path.

Wall Clearing Logic
Determines the direction between previousCell and currentCell.

Clears the walls accordingly:

Left -> Right: Clears right wall of previousCell and left wall of currentCell.

Right -> Left: Clears left wall of previousCell and right wall of currentCell.

Back -> Front: Clears front wall of previousCell and back wall of currentCell.

Front -> Back: Clears back wall of previousCell and front wall of currentCell.

Dependencies
1. MazeCell Class
Represents individual cells of the maze. Must include:

float X, Z: Coordinates of the cell.

bool isVisited: Indicates whether the cell has been visited.

void Visit(): Marks the cell as visited.

void ClearWall(CellWalls wall): Clears a specified wall.

void SetCoordinates(int x, int z): Sets the cell’s coordinates.

2. CellWalls Enum
Defines the walls of a cell (e.g., left, right, front, back).

Notes
Dynamic Cell Size:

Currently assumes a cell size of 1 unit.

To handle different sizes, adjust positions during instantiation (e.g., multiply coordinates by the cell size).

Maze Visualization:

The coroutine includes a WaitForSeconds delay to visualize the generation process step-by-step.
