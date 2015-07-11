// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module FsCactpot

open System
open System.Text
open System.Collections.Generic 

// Save computed results by using an internal dictionary.
// Note that memoize is inferred to have type
// memoize: ('a -> 'b) -> ('a -> 'b)

let memoize f =
    let cache = Dictionary<_, _>()
    fun x ->
        if cache.ContainsKey(x) then cache.[x]
        else let res = f x
             cache.[x] <- res
             res
let contains xs x = xs |> List.exists ((=) x)

let except omits xs = List.filter (fun x -> List.exists (fun y -> x = y) omits |> not) xs

type FsCactpotSolver(cSquares : int []) =
    class 
        let squares = cSquares
        let lines = [ [ 0; 1; 2 ];  [ 3; 4; 5 ];  [ 6; 7; 8 ]; 
                      [ 0; 3; 6 ];  [ 1; 4; 7 ];  [ 2; 5; 8 ]; 
                      [ 0; 4; 8 ];  [ 2; 4; 6 ]]
        let cactpotValues = [ 10000; 36; 720; 360; 80; 252; 108; 72; 54; 180; 72; 180; 119; 36; 306; 1080; 144; 1800; 3600 ]
        let remainingValues = 
            [1 .. 9] |> except (Array.toList squares)
        let lineValue x y z =
            5
        let lineValues =
            List.map lineValue lines
        let chooseNew square value = 
            let newBoard = new FsCactpotSolver(squares)
            newBoard.Choose(square, value)
            newBoard
        let chosenSquareValue square = 
            let nextBoards = remainingValues |> List.map (chooseNew square) |> List.map (fun (cs : FsCactpotSolver) -> cs.LineValues)
            squares |> Array.map double |> Array.average
        do assert false //until dummy code replaced+
        new() = FsCactpotSolver([|0; 0; 0; 0; 0; 0; 0; 0; 0|])
        member this.ChoicesRemaining =  
            squares |> Array.filter ((=) 0) |> Array.length |> ((-) 4)
        member this.IsUnchosenSquare(square : int) =
            Array.get squares square |> ((=) 0)
        member this.Choose(square, value) =
            Array.set squares square value
        member this.ChosenSquareValue(square) =
            chosenSquareValue square
        member this.ChooseNew(square, value) = 
            chooseNew square value
        member this.LineValues =
            lineValues
    end

[<EntryPoint>]
let main argv = 
    printfn "%A" <| except [1;3;5] [1 .. 9] 
    Console.ReadKey() |> ignore
    0 // return an integer exit code


