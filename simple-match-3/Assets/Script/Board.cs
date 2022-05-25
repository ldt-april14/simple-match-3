using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public sealed class Board : MonoBehaviour
{
    public  static Board instance { get; private set; }

    public Row[] rows;
    public Tile[,] tiles { get; private set; }

    public int Width => tiles.GetLength(0);
    public int Height => tiles.GetLength(1);

    private readonly List<Tile> _selection = new List<Tile>();

    private const float TweenDuration = 0.25f;

    private void Awake() => instance = this;

    private void Start()
    {
        tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];
        
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        for (var x = 0; x < Height; x++)
        {
            for (var y = 0; y < Width; y++)
            {
                var tile = rows[y].tiles[x];
                tile.x = x;
                tile.y = y;

                do
                    tile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
                while (tile.GetConnectedTiles().Skip(1).Count() >= 2);
                
                tiles[x, y] = tile;
            }
        }
    }
    
    

    public async void Select(Tile tile)
    {
        if (!_selection.Contains(tile))
        {
            if (_selection.Count > 0)
            {
                if (Array.IndexOf(_selection[0].neighbours, tile) != -1)
                {
                    _selection.Add(tile);
                }
            }
            else
            {
                _selection.Add(tile);
            }
        }

        if (_selection.Count < 2) return;
        
        Debug.Log($"Selected tiles at ({_selection[0].x},{_selection[0].y}) and ({_selection[1].x}, {_selection[1].y})");

        await Swap(_selection[0], _selection[1]);

        if (CanPop())
        {
            Pop();
            Debug.Log("Valid move");
        }
        else
        {
            await Swap(_selection[0], _selection[1]);
            Debug.Log("Invalid move");
        }
        
        _selection.Clear();
        
    }

    public async Task Swap(Tile tile1, Tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();
        
        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
                .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));
        
        await sequence.Play().AsyncWaitForCompletion();  
        
        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        var tile1Item = tile1.Item;
        tile1.Item = tile2.Item;
        tile2.Item = tile1Item;
    }

    private bool CanPop()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                if (tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 3) 
                    return true;
            }
        }

        return false;
    }

    private async void Pop()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                var tile = tiles[x, y];
                var connectedTiles = tile.GetConnectedTiles();
                
                if (connectedTiles.Skip(1).Count() < 3) continue;

                var deflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    deflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));
                }

                await deflateSequence.Play().AsyncWaitForCompletion();
                
                ScoreCounter.instance.Score += tile.Item.value * connectedTiles.Count;
                GameManager.instance.IncreaseTimeRemain();

                var inflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    connectedTile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];

                    inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                }

                await inflateSequence.Play().AsyncWaitForCompletion();

                x = 0;
                y = 0;
            }
        }
    }
}
