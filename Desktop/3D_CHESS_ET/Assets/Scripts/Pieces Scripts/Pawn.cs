using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class handling pawn moves
 */
public class Pawn : ChessPiece
{
	public Pawn() : base()
	{

	}

	protected Pawn(Pawn k) : base(k)
	{

	}

	/**
	 * Clone method for Pawn game object
	 */
	public override ChessPiece Clone()
	{
		var piece = gameObject.AddComponent<Pawn>();
		piece.SetValues(this);
		return piece;
	}

	/**
	 * Override method determining allowed moves in given position for pawn. 
	 */
	public override bool[,] IsLegalMove()
	{
		bool[,] move = new bool[8, 8];

		ChessPiece p1, p2, p3, p4;
		Array.Clear(kingCapture, 0, kingCapture.Length);
			
		if(isWhite)
		{
			//capture left
			if (PositionX != 0 && PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY + 1]; // check if there is a piece on position x+1
				if (p1 != null && !p1.isWhite)
				{
					move[PositionX - 1, PositionY + 1] = true; // move allowed
				}
				if(p1 == null)
				{
					kingCapture[PositionX - 1, PositionY + 1] = true; // king in danger there
				}
			}

			//capture right
			if (PositionX != 7 && PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY + 1]; // check if there is a piece on position x+1
				if (p1 != null && !p1.isWhite)
				{
					move[PositionX + 1, PositionY + 1] = true; // move allowed
				}
				if(p1 == null)
				{
					kingCapture[PositionX + 1, PositionY + 1] = true; // king in danger there
				}
			}

			//Forward move by 1
			if(PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX, PositionY + 1]; // check if there is a piece on position x+1
				if (p1 == null)
				{
					move[PositionX, PositionY + 1] = true; // move allowed
				}
			}

			//Forward move by 2
			if(PositionY == 1) // pawn is on second row of side
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX, PositionY + 1];  // check if there is a piece on position x+1
				p2 = ChessBoardManager.Instance.Pieces[PositionX, PositionY + 2];  // check if there is a piece on position x+2
				if (p1 == null && p2 == null)
				{
					move[PositionX, PositionY + 2] = true;
					if(ChessBoardManager.Instance.doNotPerformCheckScanForEnPassant == false)
					{
						//En passant check for a black pawn
						if (PositionX > 0 && PositionX < 7)
						{
							p3 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY + 2]; // for position x+1 y+2 if there is a pawn give him possibility to capture en passant currently investigating pawn
							p4 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY + 2]; // for position x-1 y+2 if there is a pawn give him possibility to capture en passant currently investigating pawn
							if (p3 != null && p3.GetType() == typeof(Pawn) && !p3.isWhite)
							{
								p3.isEnPassantEnabledRight = true;
							}
							if (p4 != null && p4.GetType() == typeof(Pawn) && !p4.isWhite)
							{
								p4.isEnPassantEnabledLeft = true;
							}

						}
						else if (PositionX == 0) // en passant adjustment for chessboard border
						{
							//right
							p3 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY + 2];
							if (p3 != null && p3.GetType() == typeof(Pawn) && !p3.isWhite)
							{
								p3.isEnPassantEnabledRight = true;
							}

						}
						else if (PositionX == 7) // en passant adjustment for chessboard border
						{
							//left
							p4 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY + 2];
							if (p4 != null && p4.GetType() == typeof(Pawn) && !p4.isWhite)
							{
								p4.isEnPassantEnabledLeft = true;

							}
						}
					}

				}			
			}

			//en passant move right 
			if(PositionY == 4 && PositionX < 7) // if white pawn is on 5th rank of the chessboard
			{
				if (isEnPassantEnabledRight) // if this pawn has enpassant flag set
				{
					move[PositionX + 1, PositionY + 1] = true; // move allowed
				}				
			}

			//en passant move left 
			if (PositionY == 4 && PositionX > 0) // if white pawn is on 5th rank of the chessboard
			{
				if (isEnPassantEnabledLeft) // if this pawn has enpassant flag set
				{
					move[PositionX - 1, PositionY + 1] = true; // move allowed
				}
			}

		}
		else 
		{
			//capture left
			if (PositionX != 7 && PositionY != 0)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY - 1];
				if (p1 != null && p1.isWhite)
				{
					move[PositionX + 1, PositionY - 1] = true; // move allowed
				}
				if(p1 == null)
				{
					kingCapture[PositionX + 1, PositionY - 1] = true; // king in danger there
				}
			}

			//capture right
			if (PositionX != 0 && PositionY != 0)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY - 1];
				if (p1 != null && p1.isWhite)
				{
					move[PositionX - 1, PositionY - 1] = true; // move allowed
				}
				if(p1 == null)
				{
					kingCapture[PositionX - 1, PositionY - 1] = true; // king in danger there

				}
			}

			//Forward move by 1
			if (PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX, PositionY - 1];
				if (p1 == null)
				{
					move[PositionX, PositionY - 1] = true; // move allowed
				}
			}

			//Forward move by 2
			if (PositionY == 6)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX, PositionY - 1]; // check if there is a piece on position x-1
				p2 = ChessBoardManager.Instance.Pieces[PositionX, PositionY - 2]; // check if there is a piece on position x-2
				if (p1 == null && p2 == null)
				{
					move[PositionX, PositionY - 2] = true;

					if(ChessBoardManager.Instance.doNotPerformCheckScanForEnPassant == false)
					{
						//En passant check for a white pawn
						if (PositionX > 0 && PositionX < 7)
						{
							p3 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY - 2]; // for position x+1 y-2 if there is a pawn give him possibility to capture en passant currently investigating pawn
							p4 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY - 2]; // for position x-1 y-2 if there is a pawn give him possibility to capture en passant currently investigating pawn
							//left
							if (p3 != null && p3.GetType() == typeof(Pawn))
							{
								p3.isEnPassantEnabledLeft = true;
							}
							//right
							if (p4 != null && p4.GetType() == typeof(Pawn))
							{
								p4.isEnPassantEnabledRight = true;
							}

						}
						else if (PositionX == 0) // en passant adjustment for chessboard border
						{
							//left only
							p3 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY - 2];
							if (p3 != null && p3.GetType() == typeof(Pawn))
							{
								p3.isEnPassantEnabledLeft = true;
							}

						}
						else if (PositionX == 7) // en passant adjustment for chessboard border
						{
							//right only
							p4 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY - 2];
							if (p4 != null && p4.GetType() == typeof(Pawn))
							{
								p4.isEnPassantEnabledRight = true;
							}

						}
					}


				}
			}

			//en passant move right 
			if (PositionY == 3 && PositionX > 0) // if white pawn is on 4th rank of the chessboard
			{
				if (isEnPassantEnabledRight) // if this pawn has enpassant flag set
				{
					move[PositionX - 1, PositionY - 1] = true; // move allowed
				}
			}

			//en passant move left 
			if (PositionY == 3 && PositionX < 7) // if white pawn is on 4th rank of the chessboard
			{
				if (isEnPassantEnabledLeft) // if this pawn has enpassant flag set
				{
					move[PositionX + 1, PositionY - 1] = true; // move allowed
				}
			}
		}

		return move;
	}

	/**
	 * Override method determining what pieces are defended by a pawn in given position
	 */
	public override bool[,] IsPieceDefended()
	{
		bool[,] defend = new bool[8, 8];

		ChessPiece p1;;

		if (isWhite)
		{
			//capture left
			if (PositionX != 0 && PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY + 1]; // look for a piece in a position
				if (p1 != null && p1.isWhite) // piece found with the same color
				{
					defend[PositionX - 1, PositionY + 1] = true; // piece defended
				}
			}

			//capture right
			if (PositionX != 7 && PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY + 1]; // look for a piece in a position
				if (p1 != null && p1.isWhite) // piece found with the same color
				{
					defend[PositionX + 1, PositionY + 1] = true; // piece defended
				}
			}
		}
		else
		{
			//capture left
			if (PositionX != 7 && PositionY != 0)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY - 1]; // look for a piece in a position
				if (p1 != null && !p1.isWhite) // piece found with the same color
				{
					defend[PositionX + 1, PositionY - 1] = true; // piece defended
				}
			}

			//capture right
			if (PositionX != 0 && PositionY != 0)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY - 1]; // look for a piece in a position
				if (p1 != null && !p1.isWhite) // piece found with the same color
				{
					defend[PositionX - 1, PositionY - 1] = true; // piece defended
				}
			}
		}


		return defend;

	}
}
