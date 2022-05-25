using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter instance { get; private set; }

    private int _score;

    public int Score
    {
        get => _score;

        set
        {
            if (_score == value) return;
            
            _score = value;

            scoreText.text = "Score = " + _score;
        }
    }

    public Text scoreText;

    private void Awake() => instance = this;
    
}
