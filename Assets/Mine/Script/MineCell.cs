using UnityEngine;
using UnityEngine.UI;

public class MineCell : MonoBehaviour
{
    [SerializeField]
    private Text _view = null;

    [SerializeField]
    private CellState _cellState = CellState.None;

    private GameObject MineSweeper;

    public GameObject SetMineSweeper { set { MineSweeper = value; } }

    private Minesweeper _mineSweeper => MineSweeper.GetComponent<Minesweeper>();

    //開いたか否か
    private bool isOpen = false;

    public bool getsetOpen
    {
        get
        {
            return isOpen;
        }
        set{
            isOpen = value;
            OnCellStateChanged();
        } 
    }

    //自分がどこの位置にいるセルかを保持
    public int row;

    public int column;
    public CellState CellState
    {
        get => _cellState;
        set
        {
            _cellState = value;
            OnCellStateChanged();
        }
    }

    //セルがクリックされた時用のイベント
    public void ClickEvent()
    {
        if (isOpen) { return; }

        if (_cellState == CellState.Mine)
        {
            _mineSweeper.GameOver(row, column);
        }
        else
        {
            _mineSweeper.OpenCell(row, column);
        }
    }

    private void OnValidate()
    {
        OnCellStateChanged();
    }

    private void OnCellStateChanged()
    {
        if (_view == null) { return; }

        //Open状態になったら色を塗る
        if (isOpen)
        {
            if (_cellState == CellState.Mine)
            {
                GetComponent<Image>().color = new Color32(255, 177, 177, 255);
            }
            else
            {
                GetComponent<Image>().color = new Color32(177, 255, 255, 255);
            }

            if (_cellState == CellState.None)
            {
                _view.text = "";
            }
            else if (_cellState == CellState.Mine)
            {
                _view.text = "X";
                _view.color = Color.red;
            }
            else
            {
                _view.text = ((int)_cellState).ToString();
                _view.color = Color.blue;
            }
        }
    }
}