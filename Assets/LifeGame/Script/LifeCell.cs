using UnityEngine;
using UnityEngine.UI;

public class LifeCell : MonoBehaviour
{
    [SerializeField]
    private Text _view = null;

    [SerializeField]
    private CellState _cellState = CellState.None;

    private GameObject Lifegame;
    public GameObject SetMineSweeper { set { Lifegame = value; } }

    private LifeGame _lifegame => Lifegame.GetComponent<LifeGame>();

    //開いたか否か
    public bool isChecked = false;

    //現在動作中のセルか
    private bool isMoveLife  = false;

    public bool setgetMoveLife { get => isMoveLife;
        set { isMoveLife = value; } }

    public bool getsetOpen
    {
        get
        {
            return isChecked;
        }
        set{
            isChecked = value;
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
        isChecked = !isChecked;
        OnCellStateChanged();
    }
    private void OnValidate()
    {
        OnCellStateChanged();
    }
    private void OnCellStateChanged()
    {
        if (_view == null) { return; }

        //Open状態になったら色を塗る
        if (isChecked)
        {
            GetComponent<Image>().color = new Color32(177, 255, 255, 255);
        }
        else
        {
            GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }
}