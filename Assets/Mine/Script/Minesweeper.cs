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

        // �v���n�u����Z���𐶐�����
        var parent = _gridLayoutGroup.gameObject.transform;
        _cells = CreateCells(_rows, _columns, parent);
        HiddenCells = _rows * _columns;
        // �n����ݒu����
        InitializeCells(_mineCount);
    }
    //�n���̐ݒ�
    private void InitializeCells(int mineCount)
    {
        if (mineCount > _cells.Length)
        {
            Debug.LogError($"�n�����̓Z������菭�Ȃ��ݒ肵�Ă�������\n" +
                $"�n����={mineCount}, �Z����={_cells.Length}");
            return;
        }

        // ���ׂẴZ���̏�Ԃ� None �ɂ���B
        ClearCells();

        for (var i = 0; i < mineCount; i++)
        {
            var r = Random.Range(0, _rows);
            var c = Random.Range(0, _columns);
            var cell = _cells[r, c];

            // �����_���ɑI�΂ꂽ�Z�����n�����ǂ���
            if (cell.CellState == CellState.Mine)
            {
                Debug.Log("�d�������̂ōĒ��I");
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
    //�N���b�N���ꂽ�Z������ю��͂̃Z���̒��g���`�F�b�N����
    public void OpenCell(int row , int column)
    {
        //���݂��Ȃ�Cell���Q�Ƃ��悤�Ƃ�����return
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
                //���͂ɂ�OpenCell��������
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

    //�Q�[�����N���A�����ɂ��邩�A�Q�[���I�[�o�[�ɂȂ�^�C�~���O�����m�F���鏈��
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

        // ���͂̃Z���̒n���̐��𐔂���
        var count = 0;

        var up = row - 1;
        var down = row + 1;
        var left = column - 1;
        var right = column + 1;

        if (IsMine(up, left)) { count++; } // ����
        if (IsMine(up, column)) { count++; } // ��
        if (IsMine(up, right)) { count++; } // �E��
        if (IsMine(row, left)) { count++; } // ��
        if (IsMine(row, right)) { count++; } // �E
        if (IsMine(down, left)) { count++; } // ����
        if (IsMine(down, column)) { count++; } // ��
        if (IsMine(down, right)) { count++; } // �E��

        return (CellState)count;
    }

    private bool IsMine(int row, int column)
    {
        // TODO: �w��̍s�ԍ��Ɨ�ԍ��̃Z�����n�����ǂ�����Ԃ��B
        // ���݂��Ȃ��s�ԍ��E��ԍ��Ȃ��� false ��Ԃ��B
        if(_rows - 1 < row || _columns - 1 < column || row < 0 || column < 0) { return false; }
        Debug.Log(row + "" + column);
        return _cells[row, column].CellState == CellState.Mine ? true : false;
    }
}