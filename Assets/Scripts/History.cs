using System.Collections.Generic;
using UnityEngine;

// logs the game's history (for undoing)

public static class History {
    public static List<int[,]> moves; // list of 2d grid arrays

    public static void ResetMoves(){
        moves = new List<int[,]>();
    }

    public static void PushMove(SudokuGrid grid){ // adds a move onto the stack
        int[,] move =(int[,])grid.data.Clone();
        moves.Add(move);

        foreach (var element in moves){
            Debug.Log(Data.SerializeArray(element));
        }
    }

    public static int[,] PopMove(){ // removes the current move and returns it
        int[,] move = moves[moves.Count - 1];
        moves.RemoveAt(moves.Count - 1);
        Debug.Log(Data.SerializeArray(move));
        return move;
    }
}