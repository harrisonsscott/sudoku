using System.Collections.Generic;
using UnityEngine;

// logs the game's history (for undoing)

public static class History {
    public static List<int[,]> moves; // list of 2d grid arrays

    public static void ResetMoves(){
        moves = new List<int[,]>();
    }

    public static void SaveMove(SudokuGrid grid){
        moves.Add(grid.data);
        Debug.Log(grid.data);
    }
}