using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaptureMoves : MonoBehaviour
{
    public static CaptureMoves Instance { get; set; }

    private bool[,] pieceAllowedMoves { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

 //   public bool [,] CheckAndUpdateForbiddenMoves(bool turn)
	//{
 //       int counter = 0;
 //       bool[,] captureMoves = new bool[8, 8];
 //       foreach (ChessPiece piece in ChessBoardManager.Instance.Pieces)
	//	{
 //           if(piece != null && piece.isWhite != ChessBoardManager.Instance.isWhiteTurn)
	//		{
 //               Debug.Log(piece.name);
 //               counter++;
 //               if (counter == 16)
 //                   break;
 //               pieceAllowedMoves = piece.IsLegalMove();

 //               for(int i = 0; i<8; i++)
	//			{
 //                   for(int j = 0; j<8; j++)
	//				{
 //                       if (pieceAllowedMoves[i,j] == true)
	//					{
 //                           captureMoves[i, j] = true;
	//					}
 //                   }
           
	//			}
 //   		}
	//	}

 //       return captureMoves;
	//}

}
