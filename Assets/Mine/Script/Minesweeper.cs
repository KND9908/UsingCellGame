using UnityEngine;
using UnityEngine.UI;

public class Minesweeper : MonoBehaviour
{
    [SerializeField]
    private int _rows = 1;

    [SerializeField]
    private int _columns = 1;

    [SerializeField]
    private int _mineCount = 1;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;

    [SerializeField]
    private MineCell _cellPrefab = null;

    [SerializeField]
    private GameObject TextClear;

    [SerializeField]
    private GameObject TextGameOver;

    private MineCell[,] _cells;

    private int HiddenCells;

    private bool isClickMine = false;
    void Start()
    {
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        // プレハブからセルを生成する
        var parent = _gridLayoutGroup.gameObject.transform;
        _cells = CreateCells(_rows, _columns, parent);
        HiddenCells = _rows * _columns;
        // 地雷を設置する
        InitializeCells(_mineCount);
    }
    //地雷の設定
    private void InitializeCells(int mineCount)
    {
        if (mineCount > _cells.Length)
        {
            Debug.LogError($"地雷数はセル数より少なく設定してください\n" +
                $"地雷数={mineCount}, セル数={_cells.Length}");
            return;
        }

        // すべてのセルの状態を None にする。
        ClearCells();

        for (var i = 0; i < mineCount; i++)
        {
            var r = Random.Range(0, _rows);
            var c = Random.Range(0, _columns);
            var cell = _cells[r, c];

            // ランダムに選ばれたセルが地雷かどうか
            if (cell.CellState == CellState.Mine)
            {
                Debug.Log("重複したので再抽選");
                i--;
            }
            else { 
                cell.CellState = CellState.Mine;
                Debug.Log(r +","+ c + "is Mine");
            }
        }
    }

    private void ClearCells()
    {
        foreach (var cell in _cells)
        {
            cell.CellState = CellState.None;
        }
    }

    private MineCell[,] CreateCells(int rows, int columns, Transform parent)
    {
        var cells = new MineCell[_rows, _columns];
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.SetMineSweeper = this.gameObject;
                cell.row = r;
                cell.column = c;
                cell.transform.SetParent(parent);
                cells[r, c] = cell;
            }
        }
        return cells;
    }
    //クリックされたセルおよび周囲のセルの中身をチェックする
    public void OpenCell(int row , int column)
    {
        //存在しないCellを参照しようとしたらreturn
        if (_rows - 1 < row || _columns - 1 < column || row < 0 || column < 0 || _cells[row, column].getsetOpen) { return; }
        
        if (_cells[row,column].CellState == CellState.Mine) 
        {
            return;
        }
        else
        {
            CellState cellstate = GetMineCount(row, column);
            _cells[row, column].getsetOpen = true;
            HiddenCells--;
            if (cellstate == CellState.None)
            {
                var up = row - 1;
                var down = row + 1;
                var left = column - 1;
                var right = column + 1;
                //周囲にもOpenCellをかける
                OpenCell(up, left);
                OpenCell(up, column);
                OpenCell(up, right);
                OpenCell(row, left);
                OpenCell(row, right);
                OpenCell(down, left);
                OpenCell(down, column);
                OpenCell(down, right);
            }
            else
            {
                var cell = _cells[row, column];
                cell.CellState = GetMineCount(row, column);
            }
        }
        GameStateCheck();
    }

    public void GameOver(int row , int column)
    {
        _cells[row, column].getsetOpen = true;
        TextGameOver.SetActive(true);
    }

    //ゲームがクリア条件にあるか、ゲームオーバーになるタイミングかを確認する処理
    private void GameStateCheck()
    {
        if(HiddenCells == _mineCount)
        {
            TextClear.SetActive(true);
        }
    }

    private CellState GetMineCount(int row, int column)
    {
        var cell = _cells[row, column];
        if (cell.CellState == CellState.Mine) { return CellState.Mine; }

        // 周囲のセルの地雷の数を数える
        var count = 0;

        var up = row - 1;
        var down = row + 1;
        var left = column - 1;
        var right = column + 1;

        if (IsMine(up, left)) { count++; } // 左上
        if (IsMine(up, column)) { count++; } // 上
        if (IsMine(up, right)) { count++; } // 右上
        if (IsMine(row, left)) { count++; } // 左
        if (IsMine(row, right)) { count++; } // 右
        if (IsMine(down, left)) { count++; } // 左下
        if (IsMine(down, column)) { count++; } // 下
        if (IsMine(down, right)) { count++; } // 右下

        return (CellState)count;
    }

    private bool IsMine(int row, int column)
    {
        // TODO: 指定の行番号と列番号のセルが地雷かどうかを返す。
        // 存在しない行番号・列番号なら常に false を返す。
        if(_rows - 1 < row || _columns - 1 < column || row < 0 || column < 0) { return false; }
        Debug.Log(row + "" + column);
        return _cells[row, column].CellState == CellState.Mine ? true : false;
    }
}