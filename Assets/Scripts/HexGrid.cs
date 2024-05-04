using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;

internal class HexGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float _cellRadius = 30;
    [SerializeField]
    private float _bufferDistance = 4;
    [SerializeField]
    private int _mapRadius = 5;


    [SerializeField]
    public int MapRadius
    {
        get { return _mapRadius; }
        set
        {
            _mapRadius = value; InitGrid();
        }
    }

    public float CellRadius
    {
        get { return _cellRadius; }
        set
        {
            _cellRadius = value; InitGrid();
        }
    }
    public float BufferDistance
    {
        get { return _bufferDistance; }
        set
        {
            _bufferDistance = value; InitGrid();
        }
    }

    GameObject[,] GridItems;
    //Button[,] GridButtons;

    [SerializeField]
    GameObject EntryPrefab;
    GameObject EntryParent;

    public void InitGrid()
    {
        int radius = MapRadius;
        if (GridItems != null)
        {
            foreach (var item in GridItems)
            {
                Destroy(item.gameObject);
            }

        }


        if (EntryParent == null)
            EntryParent = gameObject;

        GridItems = new GameObject[(radius + 1) * 2, (radius + 1) * 2];

        float XStep, YStep;
        XStep = (CellRadius ) * Mathf.Sqrt(3f) + BufferDistance;
        YStep = (3f / 2f) * (CellRadius + BufferDistance) ;


        for (int x = -radius ; x <= radius; ++x)
        {
            for (int y = -radius; y <= radius; ++y)
            {
                for (int index = -radius ; index <= radius; ++index)
                {

                    if (x + y + index != 0)
                        continue;
                    
                    var HexEntry = Object.Instantiate(EntryPrefab, EntryParent.transform);
                    HexEntry.SetActive(true);
                    HexEntry.TryGetComponent<RectTransform>(out var rect);
                    
                    rect.anchoredPosition = new Vector2(x * XStep + (0.5f*y * XStep), y * YStep);
                    //var btn = HexEntry.AddOrGet<Button>();
                    Debug.Log(x + ", " + y + " -> " + radius * 2);
                    GridItems[x + radius, y + radius] = HexEntry;
                    //GridButtons[x, y] = btn;

                }
            }
        }
    }
    [SerializeField]
    private bool CursorInside = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorInside = false;
    }
    public void Update()
    {

    }
}
