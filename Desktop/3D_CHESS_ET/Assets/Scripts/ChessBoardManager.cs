using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class ChessBoardManager : MonoBehaviour
{
	#region fields
	public static ChessBoardManager Instance { get; set; }

	private const float TileSize = 1.0f;
	private const float TileOffSet = 0.5f; // TileSize / 2
	private const int TilesX = 8, TilesY = 8;  

	private int locationX = -1, locationY = -1;

	private Quaternion PieceRotation = Quaternion.Euler(0, -90, 0);

	private ChessPiece selectedPiece;
	public bool isWhiteTurn = true;

	public bool isCheck = false;
	public bool figureUnableToProtect = false;
	public ChessPiece checkingPiece; 

	private SpecialMoves specialMoves;

	#endregion
	#region serialized_fields
	[SerializeField] public GameObject cube;
	#endregion
	#region structures
	public ChessPiece [,] Pieces { get; set; }
	private bool [,] allowedMoves { get; set; }
	private bool [,] tempAllowedMoves { get; set; }

	private bool [,] forbiddenMoves { get; set; }
	private bool [,] protectedMoves { get; set; }

	private bool [,] allowedMovesAfterMove { get; set; }

	private Dictionary<Tuple<int, int>, bool[,]> allPossibleMovesForCheck = new Dictionary<Tuple<int, int>, bool[,]>();
	private bool [,] currentlyPossibleMoves = new bool[8, 8];
	private bool [,] tempPossibleMoves = new bool[8, 8];
	private bool [,] allowedForCheck;
	private bool[,] add { get; set; }


	public List<GameObject> chessPiecesPrefabs;
	private List<GameObject> chessPiecesActive;
	#endregion
	#region methods

	private void Start()
	{
		Instance = this;
		SpawnAllChessPieces();

	}

	private void Update()
	{
		SelectedField();
		DrawGameBoard();


		if(Input.GetMouseButtonDown(0))
		{
			Debug.Log(locationX);
			Debug.Log(locationY);
			MapPositionToChessPosition(locationX,locationY);

		
			if(locationX >= 0 && locationY >= 0)
			{
				if(selectedPiece == null)
				{
					//select piece
					SelectChessPiece(locationX, locationY);
				}
				else
				{
					//move already selected piece
					MoveChessPiece(locationX, locationY);
				}
			}
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

			// additional check for king in a check state for forbidden moves

		}
		else
		{
			Debug.Log("selected:" + Pieces[x, y].name);
			if (Pieces[x, y].GetType() == typeof(King))
			{
				allowedMoves = Pieces[x, y].IsLegalMove();
				foreach (ChessPiece p in Pieces)// king forbidden moves because of the capture on them
				{
					if (p != null && p.isWhite != isWhiteTurn)
					{
						forbiddenMoves = p.IsLegalMove();
						protectedMoves = p.IsPieceDefended();

						for (int i = 0; i < 8; i++)
						{
							for (int j = 0; j < 8; j++)
							{
								if (p.GetType() == typeof(Pawn)) 
								{
									if (p.kingCapture[i, j] == true && allowedMoves[i, j] == true)
									{
										allowedMoves[i, j] = false;
									}
									if (protectedMoves[i, j] == true && allowedMoves[i, j] == true)
									{
										allowedMoves[i, j] = false;
									}
								}
								else
								{
									if (forbiddenMoves[i, j] == true && allowedMoves[i, j] == true)
									{
										allowedMoves[i, j] = false;
									}
									if (protectedMoves[i, j] == true && allowedMoves[i, j] == true)
									{
										allowedMoves[i, j] = false;
									}
									if (Pieces[x, y].isInitialMoveDone == false && allowedMoves[x + 1, j] == false)
									{
										if (allowedMoves[x + 1, j] == false)
										{
											allowedMoves[x + 2, j] = false;
										}
										if (allowedMoves[x - 1, j] == false || allowedMoves[x - 2, j] == false)
										{
											allowedMoves[x - 2, j] = false;
										}
									}
								}
							}

						}
					}
				}
			}
			else
			{
				allowedMoves = Pieces[x, y].IsLegalMove();
			}
		}

		selectedPiece = Pieces[x, y];

		BoardAddons.Instance.HighlightAllowedMoves(allowedMoves);
	}

	public void MoveChessPiece(int x, int y)
	{
		if(allowedMoves[x,y])
		{
			ChessPiece piece = Pieces[x, y];
			if(piece != null && piece.isWhite != isWhiteTurn)
			{
				chessPiecesActive.Remove(piece.gameObject);
				Destroy(piece.gameObject);
			}

			//en passant
			if(piece == null)
			{
				if(selectedPiece.GetType() == typeof(Pawn))
				{
					if(isWhiteTurn)
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
			foreach (ChessPiece pawn in Pieces)//wydzielic do funkcji
			{
				if(pawn != null && pawn.isWhite == isWhiteTurn && pawn.GetType() == typeof(Pawn))
				{
					if(pawn.PositionX != selectedPiece.PositionX)
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

			isCheck = false;

			//castle king & queen side
			if (KingMove == -2 && selectedPiece.GetType() == typeof(King))
			{
				chessPiecesActive.Remove(Pieces[x + 1, y].gameObject);
				Destroy(Pieces[x + 1, y].gameObject);
				if(isWhiteTurn)
				{
					SpawnChessPiece(3, x - 1, y, "White Rook");
				}
				else
				{
					SpawnChessPiece(9, x - 1, y, "Dark Rook");
				}

			}
			else if(KingMove == 2 && selectedPiece.GetType() == typeof(King))
			{
				chessPiecesActive.Remove(Pieces[x - 2, y].gameObject);
				Destroy(Pieces[x - 2, y].gameObject);
				if (isWhiteTurn)
				{
					SpawnChessPiece(3, x + 1, y, "White Rook");
				}
				else
				{
					SpawnChessPiece(9, x + 1, y, "Dark Rook");
				}
			}


			//TO DO - choose to which piece promote
			//white promotion
			if (y == 7 && selectedPiece.GetType() == typeof(Pawn))
			{
				chessPiecesActive.Remove(Pieces[x,y].gameObject);
				Destroy(Pieces[x, y].gameObject);
				SpawnChessPiece(4, x, y, "White Queen");
			}
			//
			// black promotion - TODO
			//

			//check scan
			foreach (ChessPiece afterMove in Pieces)
			{
				if(afterMove != null && afterMove.isWhite == isWhiteTurn)
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
									Debug.Log("Check essssssssssa");
									checkingPiece = afterMove;
									isCheck = true;
								}
							}
						}
					}
				}
			}
			
			//turn change
			BoardAddons.Instance.ChangeTurnCubeColor(isWhiteTurn, cube);
			isWhiteTurn = !isWhiteTurn;

		}

		BoardAddons.Instance.HideHighlights();
		selectedPiece = null;
		if(isCheck)
		{
			//HandleCheck.Instance.HandleKingCheck(isCheck);
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
											if (p != null && p.GetType() == typeof(King) && p.isWhite == isWhiteTurn) // if it hits a piece and the piece is a king
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

								Pieces[i, j] = null;
								Pieces[savedX, savedY] = checkPiece;
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

			if(isCheck && allPossibleMovesForCheck.Count == 0)
			{
				Debug.Log("GAME OVER - CHECKMATE" + isWhiteTurn + "wins");
			}
		}
	}
	
	private void MapPositionToChessPosition(int x, int y)
	{
		var o = PreparePositionDictionary();
		string val;
		string key = x.ToString() + y.ToString();
		if (o.TryGetValue(key, out val))
		{
			Debug.Log(val);
		}
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
		chessPiecesActive = new List<GameObject>();
		Pieces = new ChessPiece[TilesX, TilesY];

		for(int i=0; i<TilesX; i++)
		{
			SpawnChessPiece(0, i, 1,"White Pawn");
			SpawnChessPiece(6, i, 6,"Dark Pawn");
		}

		SpawnChessPiece(1, 2, 0,"White Bishop");
		SpawnChessPiece(1, 5, 0,"White Bishop");
		SpawnChessPiece(7, 2, 7,"Dark Bishop");
		SpawnChessPiece(7, 5, 7,"Dark Bishop");

		SpawnChessPiece(2, 1, 0,"White Knight");
		SpawnChessPiece(2, 6, 0,"White Knight");
		SpawnChessPiece(8, 1, 7,"Dark Knight");
		SpawnChessPiece(8, 6, 7,"Dark Knight");

		SpawnChessPiece(3, 0, 0,"White Rook");
		SpawnChessPiece(3, 7, 0,"White Rook");
		SpawnChessPiece(9, 0, 7,"Dark Rook");
		SpawnChessPiece(9, 7, 7,"Dark Rook");

		SpawnChessPiece(4, 3, 0,"White Queen");
		SpawnChessPiece(5, 4, 0,"White King");

		SpawnChessPiece(10, 3, 7,"Dark Queen");
		SpawnChessPiece(11, 4, 7,"Dark King");


	}

	/*
	 * Spawn single chess piece
	 */
	public void SpawnChessPiece(int index,int x, int y, string name)
	{
		var inst = Instantiate(chessPiecesPrefabs[index], GetTileCenter(x,y), PieceRotation) as GameObject;
		inst.transform.SetParent(transform);
		Pieces[x, y] = inst.GetComponent<ChessPiece>();
		Pieces[x, y].SetPosition(x, y);
		Pieces[x, y].name = name;
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

	private void EndGame()
	{
		if(isWhiteTurn)
		{
			Debug.Log("White wins");
		}
		else
		{
			Debug.Log("Black wins");
		}
	}

	#endregion
}
