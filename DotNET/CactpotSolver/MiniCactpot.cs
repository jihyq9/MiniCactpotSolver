using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public class MiniCactpot
{
    static void Main(string[] args)
    {
        var testBoard = new MiniCactpot();
        //MiniCactpot.LoadValueStore();
        //testBoard.Choose(0, 4);
        //Console.WriteLine("Max: " + testBoard.BoardValue);
        for (int i = 0; i < testBoard.CactpotSquares.Length; i++) {
            Console.WriteLine("{0}: {1}", i, testBoard.ChosenSquareValue(i));
        }
        //MiniCactpot.SaveValueStore();
        Console.ReadLine();
    }

    #region Variables
    /// <summary>
    /// All acceptable sequences of squares.
    /// 0 1 2
    /// 3 4 5
    /// 6 7 8
    /// </summary>
    private int[][] _lines = { new int[]{ 0, 1, 2 }, new int[]{ 3, 4, 5 }, new int[]{ 6, 7, 8 }, 
                               new int[]{ 0, 3, 6 }, new int[]{ 1, 4, 7 }, new int[]{ 2, 5, 8 }, 
                               new int[]{ 0, 4, 8 }, new int[]{ 2, 4, 6 } };
    /// <summary>
    /// The value of all sums of lines, from 6 to 24.
    /// </summary>
    private int[] _cactpotValues = { 10000, 36, 720, 360, 80, 252, 108, 72, 54, 180, 72, 180, 119, 36, 306, 1080, 144, 1800, 3600 };

    /// <summary>
    /// All allowed square values.
    /// </summary>
    private int[] _possibilities = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    #endregion

    #region Constructors
    public MiniCactpot()
    {
        this.CactpotSquares = new int[9];
        if (ChosenSquareValueStore == null) { 
            ChosenSquareValueStore = new Dictionary<long, double>();
            LoadValueStore();
        }
    }

    public MiniCactpot(MiniCactpot copiedCactpot)
        : this()
    {
        copiedCactpot.CactpotSquares.CopyTo(this.CactpotSquares, 0);
    }
    #endregion

    #region Properties
    private static Dictionary<long, double> ChosenSquareValueStore { get; set; }

    public int[] CactpotSquares{get;set;}

    public int[] RemainingValues
    {
        get
        {
            return _possibilities.Except(CactpotSquares).ToArray();
        }
    }

    public int ChoicesRemaining
    {
        get
        {
            int count = 0;
            for (int i = 0; i < CactpotSquares.Length; i++) {
                if (CactpotSquares[i] == 0) { count += 1; }
            }
            return count - 5;
        }
    }

    /// <summary>
    /// The indices of all squares that are uncovered.
    /// </summary>
    public IEnumerable<int> UnchosenSquares
    {
        get
        {
            for (int i = 0; i < CactpotSquares.Length; i++) {
                if (CactpotSquares[i] == 0) {
                    yield return i;
                }
            }
        }
    }

    public double BoardValue
    {
        get
        {
            // Value of a board with no choices left is the value of the best line
            if (ChoicesRemaining <= 0) {
                return LineValues().Max();
            } else {
                // Value of a board with choices left is the value of the best square choice
                return (from choice in this.UnchosenSquares 
                        select this.ChosenSquareValue(choice))
                        .Max();
            }
        }
    }
    #endregion

    #region Methods
    public bool IsUnchosenSquare(int square)
    {
        return CactpotSquares[square] == 0;
    }

    public override int GetHashCode()
    {
        int hash = 0;
        for (int i = CactpotSquares.Length - 1; i >= 0; i--) {
            hash *= 10;
            hash += CactpotSquares[i];
        }
        return hash;
    }

    public void Choose(int squareChosen, int value)
    {
        this.CactpotSquares[squareChosen] = value;
    }

    public double ChosenSquareValue(int squareChosen)
    {
        if (ChoicesRemaining < 0) {
            throw new ArgumentException("No choices to make!");
        }
        if (CactpotSquares[squareChosen] != 0) {
            return -1;
        }
        long choiceHash = (long)this.GetHashCode() * 10 + squareChosen;
        if (!ChosenSquareValueStore.ContainsKey(choiceHash)) {
            // Value of a chosen square is the average of the values of all possible boards that follow that choice
            double totalNextBoardValue = 0;
            foreach (int value in RemainingValues) {
                var newBoard = new MiniCactpot(this);
                newBoard.Choose(squareChosen, value);
                totalNextBoardValue += newBoard.BoardValue;
            }
            ChosenSquareValueStore.Add(choiceHash, totalNextBoardValue * 1.0 / RemainingValues.Count());
        }
        return ChosenSquareValueStore[choiceHash];
    }

    public IEnumerable<double> LineValues()
    {
        foreach (int[] line in _lines) {
            yield return LineValue(line);
        }
    }

    public double LineValue(int[] line)
    {
        return LineValue(line[0], line[1], line[2]);
    }

    public double LineValue(int startSquare, int midSquare, int endSquare)
    {
        int[] lineSquares = { CactpotSquares[startSquare], CactpotSquares[midSquare], CactpotSquares[endSquare] };
        int[] filledSquares = lineSquares.Where(ls => ls != 0).ToArray();
        int emptySquares = 3 - filledSquares.Length;
        if (emptySquares == 0) {
            return GetCactpotJackpot(lineSquares.Sum());
        } else {
            var jackpots = new List<List<int>>();
            var valuesLeft = RemainingValues;
            // Get sets of possible empty squares, each # representing an item in valuesLeft
            var possibilityMap = Helpers.Combinations(emptySquares, valuesLeft.Length);
            int totalPossible = 0;
            foreach (var possibility in possibilityMap) {
                List<int> line = filledSquares.ToList();
                foreach (var square in possibility) {
                    line.Add(valuesLeft[square]);
                }
                totalPossible += GetCactpotJackpot(line.Sum());
            }

            return totalPossible * 1.0 / possibilityMap.Count();
        }
    }

    public int GetCactpotJackpot(int lineTotal)
    {
        return _cactpotValues[lineTotal - 6];
    }

    public static void SaveValueStore()
    {
        using (var outp = new StreamWriter(@"C:\Users\morbious\text.txt")) {
            int i = 0;
            foreach (long key in ChosenSquareValueStore.Keys.OrderBy(val => val)) {
                i = i % 5;
                outp.Write(String.Format("({0},{1}){2}", key, Math.Round(ChosenSquareValueStore[key], 2), i == 4 ? "\n" : "\t"));
                i += 1;
            }
        }
    }

    public static void LoadValueStore()
    {
        foreach (string elem in CactpotSolver.Properties.Resources.cactpotValues.Split('\t', '\n')) {
            if (elem.Length > 0) {
                var pair = elem.Trim('(', ')').Split(',');
                ChosenSquareValueStore[long.Parse(pair[0])] = double.Parse(pair[1]);
            }
        }
    }
    #endregion
}

public class Helpers
{
    public static IEnumerable<int[]> Combinations(int m, int n)
    {
        int[] result = new int[m];
        Stack<int> stack = new Stack<int>();
        stack.Push(0);

        while (stack.Count > 0) {
            int index = stack.Count - 1;
            int value = stack.Pop();

            while (value < n) {
                result[index++] = value++;
                stack.Push(value);
                if (index == m) {
                    yield return result;
                    break;
                }
            }
        }
    }
}
