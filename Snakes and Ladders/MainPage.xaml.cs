namespace Snakes_and_Ladders;

public partial class MainPage : ContentPage
{
    Random _random; // Global declaration.
    const int LEFT = 1, RIGHT = 10;
    const int L2R = 1, R2L = -1;
    const int MAX_PLAYERS = 3;
    int _currentEdge = RIGHT;
    int _currentDirection;
    int _currentPlayer;
    int[] _playerEdges = { RIGHT, RIGHT, RIGHT };
    int[] _playerDirs = { L2R, L2R, L2R };

    const string STATE_FILE = "GameState.txt";
    // Save player positions, directions, edges, dice rolls, player turns.

    List<BoardObject> _snakes;
    List<BoardObject> _ladders;

    public MainPage()
    {
        LoadGameState();
        InitializeComponent();
        InitialiseGameStart();

    }

    private void InitialiseGameStart()
    {
        CreateSnakesList();
        CreateLaddersList();
        _random = new Random(); // instantiate the object
        _currentEdge = RIGHT;   // default at start.
        _currentDirection = L2R;
        _currentPlayer = 1;
    }

    private void CreateLaddersList()
    {
        if (_ladders == null) _ladders = new List<BoardObject>();
        BoardObject b;

        b = new BoardObject();
        b.StartX = 1; b.StartY = 10;
        b.EndX = 3; b.EndY = 7;

        b = new BoardObject();
        b.StartX = 4; b.StartY = 10;
        b.EndX = 7; b.EndY = 9;

        b = new BoardObject();
        b.StartX = 9; b.StartY = 10;
        b.EndX = 10; b.EndY = 7;

        b = new BoardObject();
        b.StartX = 1; b.StartY = 8;
        b.EndX = 2; b.EndY = 6;

        b = new BoardObject();
        b.StartX = 8; b.StartY = 8;
        b.EndX = 4; b.EndY = 2;

        b = new BoardObject();
        b.StartX = 10; b.StartY = 5;
        b.EndX = 7; b.EndY = 4;

        b = new BoardObject();
        b.StartX = 1; b.StartY = 3;
        b.EndX = 1; b.EndY = 1;

        b = new BoardObject();
        b.StartX = 10; b.StartY = 3;
        b.EndX = 10; b.EndY = 1;

    }

    private void CreateSnakesList()
    {
        if (_snakes == null) _snakes = new List<BoardObject>();
        BoardObject b; // Use this to create the snakes.

        // Create a new board object.
        b = new BoardObject();
        b.StartX = 4; b.StartY = 9;
        b.EndX = 7; b.EndY = 10;
        _snakes.Add(b);

        b = new BoardObject();
        b.StartX = 7; b.StartY = 5;
        b.EndX = 7; b.EndY = 7;
        _snakes.Add(b);

        b = new BoardObject();
        b.StartX = 2; b.StartY = 4;
        b.EndX = 2; b.EndY = 9;
        _snakes.Add(b);

        b = new BoardObject();
        b.StartX = 4; b.StartY = 4;
        b.EndX = 1; b.EndY = 5;
        _snakes.Add(b);

        b = new BoardObject();
        b.StartX = 7; b.StartY = 2;
        b.EndX = 4; b.EndY = 8;
        _snakes.Add(b);

        b = new BoardObject();
        b.StartX = 8; b.StartY = 1;
        b.EndX = 8; b.EndY = 3;
        _snakes.Add(b);

        b = new BoardObject();
        b.StartX = 6; b.StartY = 1;
        b.EndX = 6; b.EndY = 3;
        _snakes.Add(b);

        b = new BoardObject();
        b.StartX = 3; b.StartY = 1;
        b.EndX = 2; b.EndY = 3;
        _snakes.Add(b);
    }

    private void BtnDice_Clicked(object sender, EventArgs e)
    {
        // If "rollInProgress" == F, then progress with the move.
        // Get the random number, display the button in text.
        // Set a boolean value (True/False) to true. 

        int roll = _random.Next(1, 7);
        BtnDice.Text = roll.ToString();
        MoveCurrentPlayer(roll);

        // Reset roll in progress to false.
    }

    private async void MoveCurrentPlayer(int roll)
    {
        int playerNumber = _currentPlayer;
        _currentEdge = _playerEdges[_currentPlayer - 1];
        _currentDirection = _playerDirs[_currentPlayer - 1];
        string playerName = "BVPlayerPiece" + playerNumber.ToString();

        BoxView b = GridGameTable.FindByName(playerName) as BoxView;

        await MovePiece(b, roll);
        _playerEdges[_currentPlayer - 1] = _currentEdge;
        _playerDirs[_currentPlayer - 1] = _currentDirection;

        _currentPlayer++;

        if (_currentPlayer > MAX_PLAYERS)
        {
            _currentPlayer = 1;
        }
    }

    private async Task MoveHorizontal(BoxView piece, int roll)
    {
        int direction = 1;

        // Get the row and check if it should be moving left or right.

        int row = (int)piece.GetValue(Grid.RowProperty);
        /* if (row % 2 != 0)
             direction = -1; */

        // Get the column, move it left or right, set it to that value.

        int col = (int)piece.GetValue(Grid.ColumnProperty);
        col += (roll * direction);
        //piece.SetValue(Grid.ColumnProperty, col);

        // Translation
        // Get the actual width and distance (since its device dependant).

        double xStep = GridGameTable.Width / 12;
        double xDistance = xStep * roll;

        await piece.TranslateTo(xDistance, 0, 1000);
        piece.SetValue(Grid.ColumnProperty, col);
        piece.TranslationX = 0;

    }

    private async Task MoveUp(BoxView piece)
    {
        // Get row value, -1, reset.

        int row = (int)piece.GetValue(Grid.RowProperty);

        // May have to check the winning row later.

        row--;

        // piece.SetValue(Grid.RowProperty, row);

        /*  _currentEdge = LEFT;
          if (row % 2 == 0)
              _currentEdge = RIGHT;
        */

        if (_currentEdge == LEFT)
        {
            _currentEdge = RIGHT;
            _currentDirection = L2R;
        }
        else
        {
            _currentEdge = LEFT;
            _currentDirection = R2L;
        }

        double yStep = GridGameTable.Height / 12;
        double yDistance = yStep * -1;

        await piece.TranslateTo(0, yDistance, 500);
        piece.SetValue(Grid.RowProperty, row);
        piece.TranslationY = 0;
    }

    private async Task MovePiece(BoxView piece, int roll)
    {
        int col = (int)piece.GetValue(Grid.ColumnProperty);

        // To solve the problem of 1-10, edge - col, use the absolute value.

        if (Math.Abs(_currentEdge - col) >= roll)
        {
            await MoveHorizontal(piece, roll);
            // Basically if you have enough spaces available to move
            // until you hit the edge, proceed. If you don't, go to
            // the else statement.
        }
        else
        {
            // Move the distance between col and _currentEdge.
            await MoveHorizontal(piece, Math.Abs(_currentEdge - col));
            roll -= Math.Abs(_currentEdge - col);
            await MoveUp(piece);
            roll--;
            if (roll > 0)
            {
                await MoveHorizontal(piece, roll);
            }
        }

        await CheckForSnakesOrLadders(piece, _snakes);
        await CheckForSnakesOrLadders(piece, _ladders);
    }

    private void BtnSave_Clicked(object sender, EventArgs e)
    {
        SaveGameState();
    }

    private void SaveGameState()
    {
        string fileText = "Hello World";
        // To write to a file we need a file name. 
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string fileName = Path.Combine(path, STATE_FILE);

        using (var w = new StreamWriter(fileName, false))
        {
            w.WriteLine(fileText);
        }
    }

    private void LoadGameState()
    {
        string fileText = "";
        // To write to a file we need a file name. 
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string fileName = Path.Combine(path, STATE_FILE);

        try // Do something. If it works, then great.
        {
            using (var r = new StreamReader(fileName, false))
            {
                fileText = r.ReadToEnd();
            }

        }
        catch (Exception) // Catch the error and deal with it. 
        {
            fileText = "There are no saved games, enjoy a new one on us...";
        }

        LblStatus.Text = fileText;
    }

    private async Task CheckForSnakesOrLadders(BoxView piece, List<BoardObject> theList)
    {
        int pieceX = (int)piece.GetValue(Grid.ColumnProperty);
        int pieceY = (int)piece.GetValue(Grid.RowProperty);

        foreach (BoardObject b in theList)
        { // Logic statement. A && B
            if ((pieceX == b.StartX) && (pieceY == b.StartY))
            {
                // On a snake is true -> move the piece.
                await SnakeLadderTranslation(piece, b.StartX, b.StartY, b.EndX, b.EndY);
                break;
            }
        }
    }

    private async Task SnakeLadderTranslation(BoxView piece,
                                          int startX, int startY,
                                          int endX, int endY)
    {
        // xdistance = endX - startX;
        // ydistance = endY - startY;
        // xstep, ystep
        double xStep = (GridGameTable.Width / 12);
        double yStep = (GridGameTable.Height / 12);

        int xDistance = endX - startX;
        int yDistance = endY - startY;

        double xTranslation = xStep * xDistance;
        double yTranslation = yStep * yDistance;

        await piece.TranslateTo(xTranslation, yTranslation, 1000);
        piece.TranslationX = 0;
        piece.TranslationY = 0;

        // reposition the piece to the end values
        piece.SetValue(Grid.RowProperty, endY);
        piece.SetValue(Grid.ColumnProperty, endX);

        if (endY % 2 == 0)
        {
            _currentDirection = L2R;
            _currentEdge = RIGHT;
        }
        else
        {
            _currentDirection = R2L;
            _currentEdge = LEFT;
        }
    }
}

