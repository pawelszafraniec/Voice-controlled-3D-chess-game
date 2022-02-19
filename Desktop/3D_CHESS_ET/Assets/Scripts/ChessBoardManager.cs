using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/**
 * Key method in a program used for selecting piece, assigning moves to it and actually moving it
 */
public class ChessBoardManager : MonoBehaviour
{
	#region fields
	public static ChessBoardManager Instance { get; set; }
	public ScoreManager manager;
	public ChessPiece checkingPiece;
	public King king;

	private const float TileSize = 1.0f;
	private const float TileOffSet = 0.5f; // TileSize / 2
	private const int TilesX = 8, TilesY = 8;

	private int locationX = -1, locationY = -1;
	public int XLocation;
	public int YLocation;

	private Quaternion PieceRotation = Quaternion.Euler(0, -90, 0);

	private ChessPiece selectedPiece;
	public bool isWhiteTurn = true;

	public bool isCheck = false;
	public bool isMate = false;
	public bool isDraw = false;
	public bool isCastle = false;
	public int numberOfMoves = 0;

	public string chessNotation = "";
	public string figureSelected;
	public string whereFrom;

	public bool figureUnableToProtect = false;
	public bool doNotPerformCheckScanForEnPassant = false;

	public Text endGameText;
	public GameObject popUpWindowForEndGame;
	public GameObject popUpWindowForWhitePromotion;
	public GameObject popUpWindowForBlackPromotion;
	public GameObject popUpExitConfirmation;
	public GameObject cameraRotation;

	private string MoveInSpeech = "";
	private string check = "";
	private string mate = "";
	public Button speechReader;
	public AudioSourceManager MoveReaderCustom;

	private List<int?[,]> positionsList = new List<int?[,]>();

	#endregion
	#region serialized_fields
	[SerializeField] public GameObject cube;
	[SerializeField] public GameObject marking;
	[SerializeField] public GameObject additionalMarking;
	#endregion
	#region structures
	public ChessPiece[,] Pieces { get; set; }

	private bool [,] allowedMoves { get; set; }
	private bool [,] tempAllowedMoves { get; set; }

	private bool [,] allowedMovesAfterMove { get; set; }
	private Dictionary<Tuple<int, int>, bool[,]> allPossibleMovesForCheck = new Dictionary<Tuple<int, int>, bool[,]>();
	private bool [,] currentlyPossibleMoves = new bool[8, 8];
	private bool [,] tempPossibleMoves = new bool[8, 8];
	private bool [,] allowedForCheck;

	public List<GameObject> chessPiecesPrefabs;
	private List<GameObject> chessPiecesActive;
	#endregion
	#region methods

	/**
	 * START method - runs when script is being enabled
	 */
	private void Start()
	{
		Instance = this;
		SpawnAllChessPieces(); // spawn pieces on the board
		DrawGameBoard(); // draw the game board
		CheckReadingMoves(); // read option and adjust reading moves system
		CheckCameraRotation(); // read option and adjust camera rotation

		Time.timeScale = 1; // enable time scale

	}

	/**
	 * UPDATE method - runs on each frame of the game
	 */
	private void Update()
	{
		SelectedField(); // select place on the game screen

		if(Input.GetMouseButtonDown(0)) // detect left click
		{

			MapPositionToChessPosition(locationX,locationY); // map clicked place into logic value
		
			if(locationX >= 0 && locationY >= 0) 
			{
				if(selectedPiece == null)
				{
					SelectChessPiece(locationX, locationY); // prepare possible moves for a chess piece
				}
				else
				{
					MoveChessPiece(locationX, locationY); // move selected chess piece
				}
			}
		}
	}

	/**
	 * Method checking status of reading moves system
	 */
	private void CheckReadingMoves()
	{
		if(GameOptionsManager.VoiceReadMovesEnabled == false) // if system not enabled
		{
			MoveReaderCustom.GetComponent<AudioSource>().mute = true; // mute audio
			speechReader.image.color = Color.red; // set control color to red
		}
		else
		{
			speechReader.image.color = Color.green; // set control color to green
		}


	}

	/**
	 * Method checking status of camera rotation system
	 */
	private void CheckCameraRotation()
	{ 
		if(GameOptionsManager.CameraRotationEnabled == false) // if option not checked for the game
		{
			cameraRotation.SetActive(false); // disable game object
		}
		else
		{
			cameraRotation.SetActive(true); // enable game object
		}
	}
	
	/**
	 * Select piece and define its possible moves
	 */
	public void SelectChessPiece(int x, int y)
	{
		if(Pieces[x,y] == null) // no piece selected
		{
			return;
		}
		if(Pieces[x,y].isWhite != isWhiteTurn) // selected piece is not from the proper color, e.g. black pawn when white color is moving
		{
			return;
		}
		if(isCheck && Pieces[x,y].isPossibleToMoveInCheck == false) // check state on the board and selected piece cannot move in such state
		{
			return;
		}
		if(isCheck && Pieces[x, y].isPossibleToMoveInCheck == true) // check state and selected piece can move in such state
		{
			Tuple<int, int> key = new Tuple<int, int>(x, y);
			allPossibleMovesForCheck.TryGetValue(key, out allowedForCheck);
			allowedMoves = allowedForCheck; // read allowed moves for a selected piece from dictionary of possible moves for a piece during a check state 
			if(Pieces[x,y].GetType() == typeof(King)) // filter allowed moves when selected piece is a king
			{
				var kingObject = gameObject.GetComponent<King>();
				var filtered = kingObject.FilterForbiddenMoves(allowedMoves, x, y); // filter by forbidden moves 
				allowedMoves = filtered;
			}
			else // filter for any other type of piece
			{
				var p = Pieces[x, y];
				FilterByPin(allowedMoves, p, x, y); // filter by pin
			}
			allPossibleMovesForCheck.Clear(); // clear list of possible moves during check
		}
		else
		{
			Debug.Log("selected:" + Pieces[x, y].name);
			if (Pieces[x, y].GetType() == typeof(King)) // if selected piece is a king
			{
				allowedMoves = Pieces[x, y].IsLegalMove();
				var kingObject = gameObject.GetComponent<King>();
				var filtered = kingObject.FilterForbiddenMoves(allowedMoves, x, y); // filter possible moves by forbidden moves
				allowedMoves = filtered; 
			}
			else // any other piece is selected
			{
				allowedMoves = Pieces[x, y].IsLegalMove();
				var p = Pieces[x, y];
				FilterByPin(allowedMoves, p, x, y); // filter possible moves by pin

			}
		}

		selectedPiece = Pieces[x, y]; // assign selected piece from Pieces array
		BoardAddons.Instance.HighlightAllowedMoves(allowedMoves); // highlight allowed moves for a selected piece

		MoveInSpeech = selectedPiece.GetType().ToString(); // assign name of selected piece to be read by reading move system
		whereFrom = MapPositionToChessPosition(x, y); 
	}

	/**
	 * Method handling move of a chess piece
	 */
	public void MoveChessPiece(int x, int y)
	{

		if (allowedMoves[x, y] && selectedPiece != null) // if target field is allowed move
		{
			chessNotation += BoardAddons.Instance.GetNotationForAPiece(MoveInSpeech); // get PGN notation for a piece
			chessNotation += whereFrom[0].ToString().ToLower(); // get start position for PGN notation

			ChessPiece piece = Pieces[x, y]; // get piece on a target position
			if (piece != null && piece.isWhite != isWhiteTurn) // capture check
			{
				chessPiecesActive.Remove(piece.gameObject); // remove from list of active chess pieces prefabs
				Destroy(piece.gameObject); // destroy game object
				chessNotation += "x"; // add to PGN notation
			}

			//en passant
			if (piece == null)
			{
				if (selectedPiece.GetType() == typeof(Pawn)) // if moving piece is a pawn
				{
					if (isWhiteTurn)
					{
						if (Math.Abs(x - selectedPiece.PositionX) == 1 && Math.Abs(y - selectedPiece.PositionY) == 1)
						{
							piece = Pieces[x, y - 1];
							chessPiecesActive.Remove(piece.gameObject); // remove from list of active chess pieces prefabs
							Debug.Log(piece.gameObject.name);
							Destroy(piece.gameObject); //capture a pawn
						}

					}
					else 
					{
						if (Math.Abs(x - selectedPiece.PositionX) == 1 && Math.Abs(y - selectedPiece.PositionY) == 1)
						{
							piece = Pieces[x, y + 1];
							chessPiecesActive.Remove(piece.gameObject); // remove from list of active chess pieces prefabs
							Debug.Log(piece.gameObject.name);
							Destroy(piece.gameObject); //capture a pawn
						}
					}
				}
			}

			//reset en passant after 1 move
			foreach (ChessPiece pawn in Pieces)
			{
				if (pawn != null && pawn.isWhite == isWhiteTurn && pawn.GetType() == typeof(Pawn))
				{
					if (pawn.PositionX != selectedPiece.PositionX)
					{
						pawn.isEnPassantEnabledLeft = false;
						pawn.isEnPassantEnabledRight = false;
					}
				}
			}

			//core
			int KingMove = selectedPiece.PositionX - x; // prepare variable detecting if castle is performed
			Pieces[selectedPiece.PositionX, selectedPiece.PositionY] = null; //remove old position of a piece
			selectedPiece.transform.position = GetTileCenter(x, y); // add new position to the piece
			selectedPiece.SetPosition(x, y); // assign new position
			selectedPiece.SetInitialMoveDone(); // set initial move done for a piece
			Pieces[x, y] = selectedPiece; // assign new position of a piece in Pieces array


			int?[,] piecesTemp = new int?[8, 8];
			for (int k = 0; k < piecesTemp.GetLength(0); k++) // copy content of Pieces array into temporary array 
			{
				for (int m = 0; m < piecesTemp.GetLength(1); m++)
				{				
					var temp = Pieces[k, m]?.Id;
					piecesTemp[k, m] = temp;
				}
			}
			positionsList.Add(piecesTemp); // add temporary array to list arrays 
			bool threeFold = CheckThreeFoldRepetition(positionsList); // check threefold repetition basing on list of Pieces arrays

			string targetPos = MapPositionToChessPosition(x, y);
			chessNotation += targetPos.ToString().ToLower(); // add entry to PGN notation

			numberOfMoves++; // increment number of moves

			//check draw
			if(OnlyKingsOnTheBoard(chessPiecesActive) || SideCannotMoveAnywhere() || threeFold) // check draw conditions
			{
				StartCoroutine(MoveReaderCustom.ReadDraw(MoveInSpeech, MapPositionToChessPosition(x, y),"draw")); // start coroutine reading draw move
				EndGamePopUp("Game draw"); // open end game pop-up
				EndGameByDraw(); // end game
				chessNotation = ""; // add entry to chess notation

				isDraw = true;
				var kingObject = gameObject.GetComponent<King>();
				var kingA = kingObject.GetKingPosition();
				var kingB = kingObject.GetOppositeKingPosition();
				BoardAddons.Instance.ShowDraw(marking, kingA.Item1, kingA.Item2, additionalMarking, kingB.Item1, kingB.Item2); // add draw markings to kings

			}

			isCheck = false; // reset check
			if(!isCheck)
				BoardAddons.Instance.HideCheck(marking);

			//castle king & queen side
			if (KingMove == -2 && selectedPiece.GetType() == typeof(King))
			{
				chessNotation = chessNotation.Remove(chessNotation.Length - 4); // adjustment
				chessNotation += "O-O"; // add entry to PGN notation

				chessPiecesActive.Remove(Pieces[x + 1, y].gameObject); // removing rook
				var t = Pieces[x + 1, y].Id;
				Destroy(Pieces[x + 1, y].gameObject); // destroying rook
				if (isWhiteTurn)
				{
					SpawnChessPiece(3, x - 1, y, "White Rook",t); // spawning it on the left of the king
				}
				else
				{
					SpawnChessPiece(9, x - 1, y, "Dark Rook",t); // spawning it on the left of the king
				}

				isCastle = true;
			}
			else if (KingMove == 2 && selectedPiece.GetType() == typeof(King))
			{
				chessNotation = chessNotation.Remove(chessNotation.Length - 4); // adjustment
				chessNotation += "O-O-O"; // add entry to PGN notation

				chessPiecesActive.Remove(Pieces[x - 2, y].gameObject); // removing rook
				var t = Pieces[x - 2, y].Id;
				Destroy(Pieces[x - 2, y].gameObject); // destroying rook
				if (isWhiteTurn)
				{
					SpawnChessPiece(3, x + 1, y, "White Rook",t); // spawning it on the right of the king
				}
				else
				{
					SpawnChessPiece(9, x + 1, y, "Dark Rook",t); // spawning it on the right of the king
				}

				isCastle = true;
			}

			if ((y == 7 || y == 0) && selectedPiece.GetType() == typeof(Pawn)) // promotion check
			{
				HandlePromotion(x, y); // perform pawn promotion
			}

			//check scan
			foreach (ChessPiece afterMove in Pieces) // for each piece of moving color
				{
					doNotPerformCheckScanForEnPassant = true; // adjustment
					if (afterMove != null && afterMove.isWhite == isWhiteTurn)
					{
						allowedMovesAfterMove = afterMove.IsLegalMove(); // allowed moves for a given piece
						for (int row = 0; row < allowedMovesAfterMove.GetLength(0); row++)
						{
							for (int col = 0; col < allowedMovesAfterMove.GetLength(1); col++)
							{
								if (allowedMovesAfterMove[row, col] == true)
								{
									var p = Pieces[row, col]; // piece on allowed move
									if (p != null && p.GetType() == typeof(King) && p.isWhite != isWhiteTurn) // if piece is attacked and this piece is a king
									{
										Debug.Log("CHECK STATE");
										checkingPiece = afterMove;
										isCheck = true;
										chessNotation += "+"; // add entry to PGN notation

										BoardAddons.Instance.ShowCheck(marking, row, col); // show marking for a attacked king
									}
								}
							}
						}
					}
				}

			doNotPerformCheckScanForEnPassant = false;

			//turn change
			BoardAddons.Instance.ChangeTurnCubeColor(isWhiteTurn, cube);
			CheckIncrement(isWhiteTurn);
			isWhiteTurn = !isWhiteTurn;
		}

		BoardAddons.Instance.HideHighlights();
		selectedPiece = null;
		if(isCheck)
		{
			check = "check";

			int savedX = 0;
			int savedY = 0;
			bool handle = false;

			foreach(ChessPiece checkPiece in Pieces) // for each piece
			{
				if (checkPiece != null && checkPiece.isWhite == isWhiteTurn) // for each piece of color being under check
				{
					tempAllowedMoves = checkPiece.IsLegalMove(); // all possible moves - core
					for(int i = 0; i< tempAllowedMoves.GetLength(0); i++)
					{
						for(int j = 0; j<tempAllowedMoves.GetLength(1); j++)
						{
							if(tempAllowedMoves[i,j] == true) //if move of a piece is possible
							{
								savedX = checkPiece.PositionX; //Simulate the position
								savedY = checkPiece.PositionY; //...
								ChessPiece saved = Pieces[i, j];
								Pieces[i, j] = checkPiece; //...
								checkPiece.SetPosition(i, j);
								Pieces[savedX, savedY] = null; //...

								allowedMovesAfterMove = checkingPiece.IsLegalMove(); // checking piece possible moves

								for (int row = 0; row < allowedMovesAfterMove.GetLength(0); row++) // available moves to block
								{
									for (int col = 0; col < allowedMovesAfterMove.GetLength(1); col++)
									{
										if (allowedMovesAfterMove[row, col] == true) 
										{
											var p = Pieces[row, col];
											if (p != null && p.GetType() == typeof(King) && p.isWhite == isWhiteTurn) // if it hits a piece and the piece is a king (check)
											{
												figureUnableToProtect = true; // such move can not be used for protection 
											}
											else
											{
												currentlyPossibleMoves[i, j] = true;
											}
										}
									}
								}
								if (checkingPiece.PositionX == checkPiece.PositionX && checkingPiece.PositionY == checkPiece.PositionY) // available moves to capture the checking piece
								{
									currentlyPossibleMoves[i, j] = true;
									figureUnableToProtect = false;
								}

								if (figureUnableToProtect) // reset if a piece cannot block the check
								{
									checkPiece.SetPosition(savedX, savedY);
									Array.Clear(currentlyPossibleMoves, 0, currentlyPossibleMoves.Length);
								}
								else
								{
									checkPiece.SetPossibleToMoveInCheck(); // piece can block check
									for(int a = 0; a< tempPossibleMoves.GetLength(0); a++)
									{
										for(int b = 0; b< tempPossibleMoves.GetLength(1); b++)
										{
											if(tempPossibleMoves[a,b] == false && currentlyPossibleMoves[a,b] == true)
											{
												tempPossibleMoves[a, b] = true; // fill all possible block moves for current piece
											}
										}
									}
									checkPiece.SetPosition(savedX, savedY);
									handle = true;
								}

								Pieces[i, j] = saved;
								Pieces[savedX, savedY] = checkPiece;
								checkPiece.SetPosition(savedX, savedY);//
								figureUnableToProtect = false;
								Array.Clear(currentlyPossibleMoves, 0, currentlyPossibleMoves.Length);

							}
						}
					} 
				}

				if (handle) // add values to the dictionary of possible blocking moves in check
				{
					for (int a = 0; a < tempPossibleMoves.GetLength(0); a++)
					{
						for (int b = 0; b < tempPossibleMoves.GetLength(1); b++)
						{
							if (tempPossibleMoves[a, b] == true)
							{
								Tuple<int, int> key = new Tuple<int, int>(savedX, savedY);
								if (!allPossibleMovesForCheck.ContainsKey(key))
								{
									bool[,] toAdd = new bool[8, 8];
									for(int i=0;i<tempPossibleMoves.GetLength(0);i++)
									{
										for(int j=0; j<tempPossibleMoves.GetLength(1);j++)
										{
											if(tempPossibleMoves[i,j] == true)
											{
												toAdd[i, j] = true;
											}
										}
									}

									allPossibleMovesForCheck.Add(new Tuple<int, int>(savedX, savedY), toAdd);
								}
							}
						}
					}

					handle = false;
					Array.Clear(tempPossibleMoves, 0, tempPossibleMoves.Length);

				}
			}

			var kingObject = gameObject.GetComponent<King>();
			//checkmate check
			if (isCheck && 
				((allPossibleMovesForCheck.Count == 1 && allPossibleMovesForCheck.ContainsKey(kingObject.GetKingPosition())) || (allPossibleMovesForCheck.Count == 0)))
			{
				//game over conditions - check, king cannot move anywhere and no piece to protect
				if(!kingObject.SimulateKingMove())
				{

					chessNotation = chessNotation.Remove(chessNotation.Length - 1); // adjustment
					chessNotation += "#"; // add PGN notation entry
					var kingA = kingObject.GetKingPosition();
					var kingB = kingObject.GetOppositeKingPosition();
					EndGamePopUp(CheckWinner(!isWhiteTurn)); // open end game pop-up
					isMate = true;
					if (isWhiteTurn) // who won?
					{
						mate = "mateWhite";
						BoardAddons.Instance.ShowMate(marking, kingA.Item1, kingA.Item2, additionalMarking, kingB.Item1, kingB.Item2); // show marking for checkmate
					}
					else
					{
						mate = "mateBlack";
						BoardAddons.Instance.ShowMate(marking, kingB.Item1, kingB.Item2, additionalMarking, kingA.Item1, kingA.Item2); // show marking for checkmate
					}
				}
			}
		}

		//Start coroutine reading performed move depending on type of the move
		if (isCheck)
		{
			if (isMate)
			{
				StartCoroutine(MoveReaderCustom.ReadMate(MoveInSpeech, MapPositionToChessPosition(x, y), mate));
			}
			else
			{
				StartCoroutine(MoveReaderCustom.ReadCheck(MoveInSpeech, MapPositionToChessPosition(x, y), check));
			}
		}
		else if(isCastle)
		{
			StartCoroutine(MoveReaderCustom.ReadCastle(MoveInSpeech, MapPositionToChessPosition(x, y),"castle"));
			isCastle = false;
		}
		else
		{
			if(allowedMoves[x,y] == true)
				StartCoroutine(MoveReaderCustom.ReadMove(MoveInSpeech, MapPositionToChessPosition(x, y)));
		}

		chessNotation += "\n"; // new line in PGN notation

	}

	/*
	 * Method handling promotion of a pawn
	 */
	private void HandlePromotion(int xPos, int yPos)
	{
		XLocation = xPos;
		YLocation = yPos;
		chessPiecesActive.Remove(Pieces[xPos, yPos].gameObject);
		var t = Pieces[xPos, yPos].Id;
		Destroy(Pieces[xPos, yPos].gameObject); // delete pawn
		if (isWhiteTurn)
		{
			SpawnChessPiece(4, xPos, yPos, "WhiteQueen",t); // spawn queen
		}
		else
		{
			SpawnChessPiece(10, xPos, yPos, "DarkQueen",t); // spawn queen
		}
	}

	/**
	 * Method handling time increment for timers
	 */
	private void CheckIncrement(bool isWhiteMoving)
	{
		if(Timer.Instance.incrementValue > 0)
		{
			if (isWhiteMoving)
			{
				Timer.Instance.timeRemainingWhite += Timer.Instance.incrementValue;
				Timer.Instance.DisplayTime(Timer.Instance.timeRemainingWhite, Timer.Instance.timeTextWhite);
			}
			else
			{
				Timer.Instance.timeRemainingBlack += Timer.Instance.incrementValue;
				Timer.Instance.DisplayTime(Timer.Instance.timeRemainingBlack, Timer.Instance.timeTextBlack);

			}
		}

	}

	/**
	 * Method opening end game pop-up
	 */
	public void EndGamePopUp(string result)
	{
		endGameText.text = result;
		popUpWindowForEndGame.SetActive(true); // show pop-up
		Time.timeScale = 0; // stop timers
	}

	/**
	 * Method purposed for adding new record to scoreboard table for a game draw
	 */
	public void EndGameByDraw()
	{
		DateTime now = DateTime.Now; // current date
		string whitePlayer = GameOptionsManager.playerWhiteName;
		string darkPlayer = GameOptionsManager.playerBlackName;
		if (String.IsNullOrEmpty(GameOptionsManager.playerWhiteName)) // check player name
			whitePlayer = "White color";
		if (String.IsNullOrEmpty(GameOptionsManager.playerBlackName)) // check player name
			darkPlayer = "Dark color";
		manager.AddScore(new Score("1", whitePlayer +" - "+ darkPlayer, "1/2 - 1/2 (draw)", now.ToString("MM/dd/yyyy H:mm"),numberOfMoves,chessNotation)); // add record to scoreboard
		numberOfMoves = 0; // reset number of moves

	}

	/**
	 * Method purposed for adding new record to scoreboard table for a checkmate
	 */
	public string CheckWinner(bool turn)
	{
		string result = "Checkmate. ";
		DateTime now = DateTime.Now; // current date

		string whitePlayer = GameOptionsManager.playerWhiteName;
		string darkPlayer = GameOptionsManager.playerBlackName;
		if (String.IsNullOrEmpty(GameOptionsManager.playerWhiteName)) // check player name
			whitePlayer = "White color";
		if (String.IsNullOrEmpty(GameOptionsManager.playerBlackName)) // check player name 
			darkPlayer = "Dark color";

		if (turn)
		{				
			result += whitePlayer + " won!";
			manager.AddScore(new Score("1", whitePlayer, darkPlayer, now.ToString("MM/dd/yyyy H:mm"), numberOfMoves,chessNotation)); // add record to scoreboard
			numberOfMoves = 0; // reset number of moves

		}
		else
		{
			result += darkPlayer + " won!";
			manager.AddScore(new Score("1", darkPlayer, whitePlayer, now.ToString("MM/dd/yyyy H:mm"), numberOfMoves, chessNotation)); // add record to scoreboard
			numberOfMoves = 0; // reset number of moves

		}
		return result;
	}

	/**
	 * Method filtering moves by pin
	 */
	private bool [,] FilterByPin(bool [,] allowed, ChessPiece piece, int posX, int posY)
	{
		bool checkFlag;

		for (int i = 0; i < allowed.GetLength(0); i++)
		{
			for (int j = 0; j < allowed.GetLength(1); j++)
			{
				if (allowed[i, j] == true) //if move of a piece is possible
				{
					int savedX = posX; //Simulate the position
					int savedY = posY; //...
					ChessPiece saved = Pieces[i, j];
					Pieces[i, j] = piece; //...
					piece.SetPosition(i, j);
					Pieces[savedX, savedY] = null; //...
												   // for all available moves of selected piece
												   // If after move simulation there is a check - move is forbidden
					checkFlag = CheckScan();
					if (checkFlag)
					{
						allowed[i, j] = false;
					}

					//reset the position
					Pieces[i, j] = saved;
					Pieces[savedX, savedY] = piece;
					piece.SetPosition(savedX, savedY);
				}
			}
		}
			return allowed;
	}

	/**
	 * Method determining a check
	 */
	private bool CheckScan()
	{
		//check scan
		foreach (ChessPiece afterMove in Pieces) // for each piece
		{
			doNotPerformCheckScanForEnPassant = true;
			if (afterMove != null && afterMove.isWhite != isWhiteTurn) // for each piece of opposite color
			{
				allowedMovesAfterMove = afterMove.IsLegalMove();
				for (int row = 0; row < allowedMovesAfterMove.GetLength(0); row++)
				{
					for (int col = 0; col < allowedMovesAfterMove.GetLength(1); col++)
					{
						if (allowedMovesAfterMove[row, col] == true)
						{
							var p = Pieces[row, col];
							if (p != null && p.GetType() == typeof(King) && p.isWhite == isWhiteTurn) // if possible move of a piece is a position of a king
							{
								return true; // there is a check
							}
						}
					}
				}
			}
		}

		return false;
	}

	/**
	 * Method checking if there are only 2 pieces left on the chessboard - kings
	 */
	private bool OnlyKingsOnTheBoard(List<GameObject> figures)
	{
		if (figures.Count == 2) // kings cannot be captured
		{
			return true;
		}
		return false;
	}
	
	/** 
	 * Method determining if a side has any legal move in position
	 */
	private bool SideCannotMoveAnywhere()
	{
		foreach(ChessPiece piece in Pieces) // for each piece
		{
			if(piece != null && piece.isWhite != isWhiteTurn) // for each piece of opposite color
			{
				var temp = piece.IsLegalMove();
				if(piece.GetType() == typeof(King))
				{
					var kingObject = gameObject.GetComponent<King>();
					var kingPos = kingObject.GetOppositeKingPosition();
					var filtered = kingObject.FilterMovesForDraw(temp, kingPos.Item1, kingPos.Item2); // define possible moves with all limitations
					temp = filtered;
				}

				for(int i = 0; i<8; i++)
				{
					for(int j = 0; j<8; j++)
					{
						if (temp[i, j] == true) // if any move possible
							return false;
					}
				}
			}
		}
		return true;
	}

	/**
	 * Method checking threefold repetition
	 */
	private bool CheckThreeFoldRepetition(List<int?[,]> positionList)
	{
		//Method enumerates through elements of the list, if three the same elements (arrays, meaning all its elements are the same) exists there, the method return true
		for (int i = 0; i < positionList.Count(); i++)
		{
			var curr = positionList.ElementAt(i);
			int counter = 0;
			bool equal = true;

			for (int j = 0; j < positionList.Count(); j++)
			{
				var contr = positionList.ElementAt(j);
				equal = true;
				if (i != j)
				{
					equal = Enumerable.Range(0, curr.Rank).All(dimension => curr.GetLength(dimension) == contr.GetLength(dimension)) &&
							curr.Cast<int?>().SequenceEqual(contr.Cast<int?>());

					if (equal == true)
					{
						counter++;
					}
					if (counter == 2)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	/**
	 * Method mapping logical position to chess position, e.g 00 -> A1
	 */
	private string MapPositionToChessPosition(int x, int y)
	{
		var o = PreparePositionDictionary();
		string val;
		string key = x.ToString() + y.ToString();
		if (o.TryGetValue(key, out val))
		{
			Debug.Log(val);
		}
		return val;
	}

	/**
	 * Method creating mapping dictionary
	 */
	public Dictionary<string, string> PreparePositionDictionary()
	{
		var dictionary = new Dictionary<string, string>();
		for(int i=0;i<8;i++)
		{
			for(int j=0;j<8;j++)
			{
				string Xpos = "";
				int sec = j+1;
				string Ypos = sec.ToString();
				if (i == 0)
				{
					Xpos = "A";
				}
				if (i == 1)
				{
					Xpos = "B";

				}
				if (i == 2)
				{
					Xpos = "C";

				}
				if (i == 3)
				{
					Xpos = "D";

				}
				if (i == 4)
				{
					Xpos = "E";

				}
				if (i == 5)
				{
					Xpos = "F";

				}
				if (i == 6)
				{
					Xpos = "G";

				}
				if (i == 7)
				{
					Xpos = "H";

				}


				string second = Xpos + Ypos;
				string first = i.ToString() + j.ToString();
				dictionary.Add(first, second);

			}
		}

		return dictionary;
	}

	/**
	 * Method used for assigning selected field on a screen in relation to place on the screen
	 */
	private void SelectedField()
	{
		if (!Camera.main)
			return;

		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50.0f,LayerMask.GetMask("ChessPlane")))
		{
			locationX = (int)hit.point.x;
			locationY = (int)hit.point.z;
		}
		else // reset
		{
			locationX = -1;
			locationY = -1;
		}
	}

	//Helper function - used in designing
	private void DrawGameBoard()
	{
		//Method creates a 8x8 grid used for developing figures moves

		//fields
		Vector3 width = Vector3.right * 8; //8 - number of tiles horizontally on the board
		Vector3 height = Vector3.forward * 8; // 8 - number of tiles vertically on the board

		for (int i = 0; i <= TilesX; i++)
		{
			var start = Vector3.forward * i;
			Debug.DrawLine(start, start + width);
			for (int j = 0; j <= TilesY; j++)
			{
				start = Vector3.right * j;
				Debug.DrawLine(start, start + height);
			}
		}

		if (locationX >= 0 && locationY >= 0)
		{
			Debug.DrawLine(Vector3.forward * locationY + Vector3.right * locationX,
						   Vector3.forward * (locationY + 1) + Vector3.right * (locationX + 1));

			Debug.DrawLine(Vector3.forward * (locationY + 1) + Vector3.right * locationX,
						   Vector3.forward * locationY + Vector3.right * (locationX + 1));

		}
	}

	/*
	 * Method spawning all pieces on the chessboard
	 */
	private void SpawnAllChessPieces()
	{
		int counterId = 0;

		chessPiecesActive = new List<GameObject>();
		Pieces = new ChessPiece[TilesX, TilesY];

		for(int i=0; i<TilesX; i++)
		{
			SpawnChessPiece(0, i, 1,"White Pawn",counterId++);
			SpawnChessPiece(6, i, 6,"Dark Pawn", counterId++);
		}

		SpawnChessPiece(1, 2, 0,"White Bishop", counterId++);
		SpawnChessPiece(1, 5, 0,"White Bishop", counterId++);
		SpawnChessPiece(7, 2, 7,"Dark Bishop", counterId++);
		SpawnChessPiece(7, 5, 7,"Dark Bishop", counterId++);

		SpawnChessPiece(2, 1, 0,"White Knight", counterId++);
		SpawnChessPiece(2, 6, 0,"White Knight", counterId++);
		SpawnChessPiece(8, 1, 7,"Dark Knight", counterId++);
		SpawnChessPiece(8, 6, 7,"Dark Knight", counterId++);

		SpawnChessPiece(3, 0, 0,"White Rook", counterId++);
		SpawnChessPiece(3, 7, 0,"White Rook", counterId++);
		SpawnChessPiece(9, 0, 7,"Dark Rook", counterId++);
		SpawnChessPiece(9, 7, 7,"Dark Rook", counterId++);

		SpawnChessPiece(4, 3, 0,"White Queen", counterId++);
		SpawnChessPiece(5, 4, 0,"White King", counterId++);

		SpawnChessPiece(10, 3, 7,"Dark Queen", counterId++);
		SpawnChessPiece(11, 4, 7,"Dark King", counterId++);

	}

	/*
	 * Spawn single chess piece
	 */
	public void SpawnChessPiece(int index,int x, int y, string name, int id)
	{
		var inst = Instantiate(chessPiecesPrefabs[index], GetTileCenter(x,y), PieceRotation) as GameObject;
		inst.transform.SetParent(transform);
		Pieces[x, y] = inst.GetComponent<ChessPiece>();
		Pieces[x, y].SetPosition(x, y);
		Pieces[x, y].name = name;
		Pieces[x, y].Id = id;
		chessPiecesActive.Add(inst);
	}

	/*
	 * Helper function to place piece in the center of a tile
	 */
	private Vector3 GetTileCenter(int x, int z)
	{
		Vector3 v = Vector3.zero;
		v.x += (TileSize * x) + TileOffSet;
		v.z += (TileSize * z) + TileOffSet;
		return v;
	}
	#endregion
}
