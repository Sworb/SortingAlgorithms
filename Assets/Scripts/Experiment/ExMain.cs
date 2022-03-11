﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Sorting;
using TMPro;
using UnityEngine;

public class ExMain : MonoBehaviour
{
    private float sortTime = 0;

    public float SortTime
    {
        get { return sortTime; }
    }
    
    [Header("Settings")]
    public int ballsCount = 250;
    public int closestBallsCount = 10;
    public int iterationsMax = 1000;
    public Algorithms algoDropdown;
    public bool fileOverrideToggle;
    
    [Header("Attached")]
    public Ball ballPrf;
    public Sprite whiteBallSprite;
    public Target target;
    public GameObject startButton;
    public TMP_Text timeText;
    public TMP_Text iterationsText;
    
    private List<Ball> ballList = new List<Ball>();
    private Ball[] ballArray;
    private int currentIteration;
    private double currentExecutionTime, avarageExecutionTime;
    private bool running = false;
    private bool printToFile = false;
    private Stopwatch sw = new Stopwatch();
    
    // Start is called before the first frame update
    public void StartButton()
    {
        currentIteration = 0;
        running = true;
        
        GameObject balls = GameObject.FindGameObjectWithTag("Balls");
        
        for (int i = 0; i < ballsCount; i++)
        {
            Ball ball = Instantiate(ballPrf, balls.transform, true);

            ballList.Add(ball);
        }

        startButton.SetActive(false);
    }

    public void StopButton()
    {
        //delete ball children under Balls in hierarcy
        //empty ball list
        //Reset itterations
    }

    // Update is called once per frame
    void Update()
    {
        
        if (running)
        {
            foreach (Ball ball in ballList)
            {
                ball.CalcDistance(target.transform.position);
            }

            ballArray = ballList.ToArray();
        
            sw.Restart();
            
            SortHelper sh = new SortHelper();
            sh.SetAlgorithm(algoDropdown);
            sh.sort(ballArray);
            currentIteration++;
            iterationsText.text = currentIteration.ToString("0000");
            
            sw.Stop();
        
            currentExecutionTime = sw.Elapsed.TotalSeconds;
            CalcAverage(currentIteration);

            for (int i = closestBallsCount - 1; i < ballArray.Length; i++)
            {
                ballArray[i].GetComponent<SpriteRenderer>().sprite = ballArray[i].startSprite;
            }
        
            for (int i = 0; i < closestBallsCount; i++)
            {
                ballArray[i].GetComponent<SpriteRenderer>().sprite = whiteBallSprite;
            }
            
            if (currentIteration > iterationsMax)
            {
                running = false;
                printToFile = true;
            }
        }

        if (printToFile == true)
        {
            AddRecord(algoDropdown.ToString(), iterationsMax, ballsCount, avarageExecutionTime, algoDropdown.ToString() + "Data.csv");
            printToFile = false;
        }
    }
    
    private void CalcAverage(int i)
    {
        if (i == 1) {
            avarageExecutionTime = currentExecutionTime;
        } else {
            avarageExecutionTime = avarageExecutionTime + (currentExecutionTime - avarageExecutionTime) / (i);
        }
        timeText.text = avarageExecutionTime.ToString("0.0000000");
        //distanceText.text = avarageShortestPath.ToString("0000.00");
    }
    
    private void AddRecord(string algoType, int iterations, int ballAmount, double time, string filepath)
    {
        if (fileOverrideToggle) {
            fileOverrideToggle = false;
            try {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, false)) {
                    file.WriteLine(algoType + "," + iterations + "," + ballAmount + "," + time);
                }
            }
            catch(Exception ex) {
                throw new AggregateException("This program messed up: ", ex);
            }
        }
        else {
            try {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, true)) {
                    file.WriteLine(algoType + "," + iterations + "," + ballAmount + "," + time);
                }
            }
            catch(Exception ex) {
                throw new AggregateException("This program messed up: ", ex);
            }
        }
    }
    
}
