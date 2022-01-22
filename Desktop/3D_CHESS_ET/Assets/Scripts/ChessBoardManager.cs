using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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

	/*
	 *  __START METHOD__
	 */
	private void Start()
	{
		Instance = this;
		SpawnAllChessPieces();
		DrawGameBoard();
		CheckReadingMoves();
		CheckCameraRotation();
	}

	/*
	 *  __UPDATE METHOD__
	 */
	private void Update()
	{
		SelectedField();

		if(Input.GetMouseButtonDown(0))
		{

			Debug.Log(locationX);
			Debug.Log(locationY);
			MapPositionToChessPosition(locationX,locationY);
		
			if(locationX >= 0 && locationY >= 0)
			{
				if(selectedPiece == null)
				{
					SelectChessPiece(locationX, locationY);
				}
				else
				{
					MoveChessPiece(locationX, locationY);
				}
			}
		}
	}

	private void CheckReadingMoves()
	{
		if(GameOptionsManager.VoiceReadMovesEnabled == false)
		{
			MoveReaderCustom.GetComponent<AudioSource>().mute = true;
			speechReader.image.color = Color.red;
		}
		else
		{
			speechReader.image.color = Color.green;
		}


	}

	private void CheckCameraRotation()
	{
		if(GameOptionsManager.CameraRotationEnabled == false)
		{
			cameraRotation.SetActive(false);
		}
		else
		{
			cameraRotation.SetActive(true);
		}
	}
	
	public void SelectChessPiece(int x, int y)
	{
		if(Pieces[x,y] == null)
		{
			return;
		}
		if(Pieces[x,y].isWhite != isWhiteTurn)
		{
			return;
		}
		if(isCheck && Pieces[x,y].isPossibleToMoveInCheck == false)
		{
			return;
		}
		if(isCheck && Pieces[x, y].isPossibleToMoveInCheck == true)
		{
			Tuple<int, int> key = new Tuple<int, int>(x, y);
			allPossibleMovesForCheck.TryGetValue(key, out allowedForCheck);
			allowedMoves = allowedForCheck;
			if(Pieces[x,y].GetType() == typeof(King))
			{
				var kingObject = gameObject.GetComponent<King>();
				var filtered = kingObject.FilterForbiddenMoves(allowedMoves, x, y);
				allowedMoves = filtered;
			}
			else
			{
				var p = Pieces[x, y];
				FilterByPin(allowedMoves, p, x, y);
			}
			allPossibleMovesForCheck.Clear();
		}
		else
		{
			Debug.Log("selected:" + Pieces[x, y].name);
			if (Pieces[x, y].GetType() == typeof(King))
			{
				allowedMoves = Pieces[x, y].IsLegalMove();
				var kingObject = gameObject.GetComponent<King>();
				var filtered = kingObject.FilterForbiddenMoves(allowedMoves, x, y);
				allowedMoves = filtered;
			}
			else
			{
				allowedMoves = Pieces[x, y].IsLegalMove();
				var p = Pieces[x, y];
				FilterByPin(allowedMoves, p, x, y);

			}
		}

		selectedPiece = Pieces[x, y];
		BoardAddons.Instance.HighlightAllowedMoves(allowedMoves);

		MoveInSpeech = selectedPiece.GetType().ToString();
		whereFrom = MapPositionToChessPosition(x, y);
	}

	public void MoveChessPiece(int x, int y)
	{

		if (allowedMoves[x, y] && selectedPiece != null)
		{
			chessNotation += BoardAddons.Instance.GetNotationForAPiece(MoveInSpeech);
			chessNotation += whereFrom[0].ToString().ToLower();

			ChessPiece piece = Pieces[x, y];
			if (piece != null && piece.isWhite != isWhiteTurn)
			{
				chessPiecesActive.Remove(piece.gameObject);
				Destroy(piece.gameObject);
				chessNotation += "x";
			}

			//en passant
			if (piece == null)
			{
				if (selectedPiece.GetType() == typeof(Pawn))
				{
					if (isWhiteTurn)
					{
						if (Math.Abs(x - selectedPiece.PositionX) == 1 && Math.Abs(y - selectedPiece.PositionY) == 1)
						{
							piece = Pieces[x, y - 1];
							chessPiecesActive.Remove(piece.gameObject);
							Debug.Log(piece.gameObject.name);
							Destroy(piece.gameObject);
						}

					}
					else
					{
						if (Math.Abs(x - selectedPiece.PositionX) == 1 && Math.Abs(y - selectedPiece.PositionY) == 1)
						{
							piece = Pieces[x, y + 1];
							chessPiecesActive.Remove(piece.gameObject);
							Debug.Log(piece.gameObject.name);
							Destroy(piece.gameObject);
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
			int KingMove = selectedPiece.PositionX - x;
			Pieces[selectedPiece.PositionX, selectedPiece.PositionY] = null;
			selectedPiece.transform.position = GetTileCenter(x, y);
			selectedPiece.SetPosition(x, y);
			selectedPiece.SetInitialMoveDone();
			Pieces[x, y] = selectedPiece;


			int?[,] piecesTemp = new int?[8, 8];
			for (int k = 0; k < piecesTemp.GetLength(0); k++)
			{
				for (int m = 0; m < piecesTemp.GetLength(1); m++)
				{				
					var temp = Pieces[k, m]?.Id;
					piecesTemp[k, m] = temp;
				}
			}
			positionsList.Add(piecesTemp);
			bool threeFold = CheckThreeFoldRepetition(positionsList);

			string targetPos = MapPositionToChessPosition(x, y);
			chessNotation += targetPos.ToString().ToLower();

			numberOfMoves++;

			//check draw
			if(OnlyKingsOnTheBoard(chessPiecesActive) || SideCannotMoveAnywhere() || threeFold)
			{
				StartCoroutine(MoveReaderCustom.ReadDraw(MoveInSpeech, MapPositionToChessPosition(x, y),"draw"));
				EndGamePopUp("Game draw");
				EndGameByDraw();
				chessNotation = "";

				isDraw = true;
				var kingObject = gameObject.GetComponent<King>();
				var kingA = kingObject.GetKingPosition();
				var kingB = kingObject.GetOppositeKingPosition();
				BoardAddons.Instance.ShowDraw(marking, kingA.Item1, kingA.Item2, additionalMarking, kingB.Item1, kingB.Item2);

			}

			isCheck = false;
			if(!isDraw)
				BoardAddons.Instance.HideCheck(marking);

			//castle king & queen side
			if (KingMove == -2 && selectedPiece.GetType() == typeof(King))
			{
				chessNotation = chessNotation.Remove(chessNotation.Length - 4);
				chessNotation += "O-O";

				chessPiecesActive.Remove(Pieces[x + 1, y].gameObject);
				var t = Pieces[x + 1, y].Id;
				Destroy(Pieces[x + 1, y].gameObject);
				if (isWhiteTurn)
				{
					SpawnChessPiece(3, x - 1, y, "White Rook",t);
				}
				else
				{
					SpawnChessPiece(9, x - 1, y, "Dark Rook",t);
				}

				isCastle = true;
			}
			else if (KingMove == 2 && selectedPiece.GetType() == typeof(King))
			{
				chessNotation = chessNotation.Remove(chessNotation.Length - 4);
				chessNotation += "O-O-O";

				chessPiecesActive.Remove(Pieces[x - 2, y].gameObject);
				var t = Pieces[x - 2, y].Id;
				Destroy(Pieces[x - 2, y].gameObject);
				if (isWhiteTurn)
				{
					SpawnChessPiece(3, x + 1, y, "White Rook",t);
				}
				else
				{
					SpawnChessPiece(9, x + 1, y, "Dark Rook",t);
				}

				isCastle = true;
			}

			if ((y == 7 || y == 0) && selectedPiece.GetType() == typeof(Pawn))
			{
				HandlePromotion(x, y);
			}

			//check scan
			foreach (ChessPiece afterMove in Pieces)
				{
					doNotPerformCheckScanForEnPassant = true;
					if (afterMove != null && afterMove.isWhite == isWhiteTurn)
					{
						allowedMovesAfterMove = afterMove.IsLegalMove();
						for (int row = 0; row < allowedMovesAfterMove.GetLength(0); row++)
						{
							for (int col = 0; col < allowedMovesAfterMove.GetLength(1); col++)
							{
								if (allowedMovesAfterMove[row, col] == true)
								{
									var p = Pieces[row, col];
									if (p != null && p.GetType() == typeof(King) && p.isWhite != isWhiteTurn)
									{
										Debug.Log("CHECK STATE");
										checkingPiece = afterMove;
										isCheck = true;
										chessNotation += "+";

										BoardAddons.Instance.ShowCheck(marking, row, col);
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

					chessNotation = chessNotation.Remove(chessNotation.Length - 1);
					chessNotation += "#";
					var kingA = kingObject.GetKingPosition();
					var kingB = kingObject.GetOppositeKingPosition();
					EndGamePopUp(CheckWinner(!isWhiteTurn));
					isMate = true;
					if (isWhiteTurn)
					{
						mate = "mateWhite";
						BoardAddons.Instance.ShowMate(marking, kingA.Item1, kingA.Item2, additionalMarking, kingB.Item1, kingB.Item2);
					}
					else
					{
						mate = "mateBlack";
						BoardAddons.Instance.ShowMate(marking, kingB.Item1, kingB.Item2, additionalMarking, kingA.Item1, kingA.Item2);
					}
				}
			}
		}

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

		chessNotation += "\n";

	}

	private void HandlePromotion(int xPos, int yPos)
	{
		XLocation = xPos;
		YLocation = yPos;
		chessPiecesActive.Remove(Pieces[xPos, yPos].gameObject);
		var t = Pieces[xPos, yPos].Id;
		Destroy(Pieces[xPos, yPos].gameObject);
		if (isWhiteTurn)
		{
			SpawnChessPiece(4, xPos, yPos, "WhiteQueen",t);
		}
		else
		{
			SpawnChessPiece(10, xPos, yPos, "DarkQueen",t);
		}
	}

	private void CheckIncrement(bool isWhiteMoving)
	{
		if(Timer.Instance.incrementValue > 0)
		{
			if (isWhiteMoving)//zamiast 1 dodac wartosc modyfikowalna
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

	public void EndGamePopUp(string result)
	{
		endGameText.text = result;
		popUpWindowForEndGame.SetActive(true);
		Time.timeScale = 0;
	}

	public void EndGameByDraw()
	{
		DateTime now = DateTime.Now;
		string whitePlayer = GameOptionsManager.playerWhiteName;
		string darkPlayer = GameOptionsManager.playerBlackName;
		if (String.IsNullOrEmpty(GameOptionsManager.playerWhiteName))
			whitePlayer = "White color";
		if (String.IsNullOrEmpty(GameOptionsManager.playerBlackName))
			darkPlayer = "Dark color";
		manager.AddScore(new Score("1", whitePlayer +" - "+ darkPlayer, "1/2 - 1/2 (draw)", now.ToString("MM/dd/yyyy H:mm"),numberOfMoves,chessNotation));
		numberOfMoves = 0;

	}

	public string CheckWinner(bool turn)
	{
		string result = "Checkmate. ";
		DateTime now = DateTime.Now;

		string whitePlayer = GameOptionsManager.playerWhiteName;
		string darkPlayer = GameOptionsManager.playerBlackName;
		if (String.IsNullOrEmpty(GameOptionsManager.playerWhiteName))
			whitePlayer = "White color";
		if (String.IsNullOrEmpty(GameOptionsManager.playerBlackName))
			darkPlayer = "Dark color";

		if (turn)
		{				
			result += whitePlayer + " won!";
			manager.AddScore(new Score("1", whitePlayer, darkPlayer, now.ToString("MM/dd/yyyy H:mm"), numberOfMoves,chessNotation));
			numberOfMoves = 0;

		}
		else
		{
			result += darkPlayer + " won!";
			manager.AddScore(new Score("1", GameOptionsManager.playerBlackName, whitePlayer, now.ToString("MM/dd/yyyy H:mm"), numberOfMoves, chessNotation));
			numberOfMoves = 0;

		}
		return result;
	}

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

	private bool CheckScan()
	{
		//check scan
		foreach (ChessPiece afterMove in Pieces)
		{
			doNotPerformCheckScanForEnPassant = true;
			if (afterMove != null && afterMove.isWhite != isWhiteTurn)
			{
				allowedMovesAfterMove = afterMove.IsLegalMove();
				for (int row = 0; row < allowedMovesAfterMove.GetLength(0); row++)
				{
					for (int col = 0; col < allowedMovesAfterMove.GetLength(1); col++)
					{
						if (allowedMovesAfterMove[row, col] == true)
						{
							var p = Pieces[row, col];
							if (p != null && p.GetType() == typeof(King) && p.isWhite == isWhiteTurn)
							{
								return true;
							}
						}
					}
				}
			}
		}

		return false;
	}

	private bool OnlyKingsOnTheBoard(List<GameObject> figures)
	{
		if (figures.Count == 2) // kings cannot be captured
		{
			return true;
		}
		return false;
	}
	
	private bool SideCannotMoveAnywhere()
	{
		foreach(ChessPiece piece in Pieces)
		{
			if(piece != null && piece.isWhite != isWhiteTurn)
			{
				var temp = piece.IsLegalMove();
				if(piece.GetType() == typeof(King))
				{
					var kingObject = gameObject.GetComponent<King>();
					var kingPos = kingObject.GetOppositeKingPosition();
					var filtered = kingObject.FilterMovesForDraw(temp, kingPos.Item1, kingPos.Item2);
					temp = filtered;
				}

				for(int i = 0; i<8; i++)
				{
					for(int j = 0; j<8; j++)
					{
						if (temp[i, j] == true)
							return false;
					}
				}
			}
		}
		return true;
	}

	private bool CheckThreeFoldRepetition(List<int?[,]> positionList)
	{
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
						Debug.Log(contr);
						Debug.Log(curr);
						Debug.Log("Counter + 1  " + counter);
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


	//

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
	 * Spawn all pieces on the board
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
