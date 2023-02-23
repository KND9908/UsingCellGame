using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LifeGame : MonoBehaviour
{
    [SerializeField]
    private int _rows = 1;

    [SerializeField]
    private int _columns = 1;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;

    [SerializeField]
    private LifeCell _cellPrefab = null;

    private LifeCell[,] _cells;

    //全セルを確認し、一定の形になっていたら動作を行わせるモードのフラグ
    private bool RunMode = false;

    [SerializeField]
    private float AnimationChgTimeSec = 1;

    void Start()
    {
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        // プレハブからセルを生成する
        var parent = _gridLayoutGroup.gameObject.transform;
        _cells = CreateCells(_rows, _columns, parent);
    }

    private LifeCell[,] CreateCells(int rows, int columns, Transform parent)
    {
        var cells = new LifeCell[_rows, _columns];
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
    private void Update()
    {
        if (RunMode)
        {
            foreach (LifeCell life in _cells)
            {
                //周期的に全セルを確認し、チェックされているCellを見つけたらその周囲に特定の形を示してならんでないかチェックする
                if (life.isChecked)
                {
                    CheckFiveLines(life.row, life.column);
                }
            }
        }
    }
    /// <summary>
    /// セル確認モードON
    /// </summary>
    public void CheckCells()
    {
        RunMode = true;
    }
    /// <summary>
    /// セル確認モードOFF
    /// </summary>
    public void ClickLifeOff()
    {
        RunMode = false;
    }
    /// <summary>
    /// 指定の場所にセルが存在するかを確認するフラグ
    /// </summary>
    private bool ExistCell(int row, int column)
    {
        if (_rows - 1 < row || _columns - 1 < column || row < 0 || column < 0) { return false; }
        else { return true; }
    }
    /// <summary>
    /// 指定の場所にセルが存在するかを確認するフラグ
    /// </summary>
    private void CheckFiveLines(int row, int column)
    {
        int LineCount = 5;
        for (int i = 0; i < LineCount; i++)
        {
            if(!ExistCell(row, column + i)) { return; }
            if(!_cells[row, column + i].isChecked || _cells[row, column + i].setgetMoveLife) { return; }
        }
            StartCoroutine(ActFiveLines(row, column));
    }
    IEnumerator ActFiveLines(int row, int column)
    {
        float f = 0;
        int frontrow = row;
        int frontcolumn = column + 2;

        while (RunMode)
        {
            f += Time.deltaTime;
            if (AnimationChgTimeSec < f)
            {
                //力技で一旦記述
                ChangeCell(frontrow, frontcolumn);
                ChangeCell(frontrow, frontcolumn - 1);
                ChangeCell(frontrow, frontcolumn + 1);
                ChangeCell(frontrow, frontcolumn - 2);
                ChangeCell(frontrow, frontcolumn + 2);
                ChangeCell(frontrow - 1, frontcolumn - 1);
                ChangeCell(frontrow - 1, frontcolumn);
                ChangeCell(frontrow - 1, frontcolumn + 1);
                ChangeCell(frontrow + 1, frontcolumn - 1);
                ChangeCell(frontrow + 1, frontcolumn);
                ChangeCell(frontrow + 1, frontcolumn + 1);
            }
            yield return null;
        }
        yield break;
    }

    private void ChangeCell(int row, int column)
    {
        if (!ExistCell(row, column)) { return; }
        _cells[row, column].ClickEvent();
        _cells[row, column].setgetMoveLife = true;
    }

}