using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Endgame : MonoBehaviour
{
    public Text scoreText;
    public Text rank;
    
    private void OnEnable()
    {
        int score = ScoreCounter.instance.Score;
        scoreText.text = "Your Score = " + score;

        string rating = "Noob";
        if (score > 99999) rating = "Grand Master";
        else if (score > 50000) rating = "Master";
        else if (score > 20000) rating = "Diamond";
        else if (score > 10000) rating = "Platinum";
        else if (score > 5000) rating = "Gold";
        else if (score > 2000) rating = "Silver";
        else if (score > 1000) rating = "Bronze";

        rank.text = "YOUR RATING: " + rating;
    }
}
