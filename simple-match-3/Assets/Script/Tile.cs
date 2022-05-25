using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Tile : MonoBehaviour
{
    public int x;
    public int y;

    private Item _item;

    public Item Item
    {
        get => _item;
        set
        {
            if (_item == value) return;

            _item = value;

            icon.sprite = _item.sprite;
        }
    }

    public Image icon;

    public Button button;

    public Tile left => x > 0 ? Board.instance.tiles[x - 1, y] : null;
    public Tile top => y > 0 ? Board.instance.tiles[x, y - 1] : null;
    public Tile right => x < Board.instance.Width - 1 ? Board.instance.tiles[x + 1, y] : null;
    public Tile bottom => y < Board.instance.Height - 1 ? Board.instance.tiles[x, y + 1] : null;

    public Tile[] neighbours => new[]
    {
        left,
        top,
        right,
        bottom,
    };

    private void Start() => button.onClick.AddListener(()=>Board.instance.Select(this));

    public List<Tile> GetConnectedTiles(List<Tile> exclude = null)
    {
        var result = new List<Tile> {this, };

        if (exclude == null)
        {
            exclude = new List<Tile>();
        }
        else
        {
            exclude.Add(this);
        }

        foreach (var neighbour in neighbours)
        {
            if (neighbour==null || exclude.Contains(neighbour) || neighbour.Item!=Item) continue;
            result.AddRange(neighbour.GetConnectedTiles(exclude));
        }

        return result;
    }
}
