﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov.Learning;

using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;

using System;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.ComponentModel;

public class MousePositionRecorder : MonoBehaviour
{
    public Button StoreGestureBtn;
    public Button LearnGesturesBtn;
    public Button PredictGestureBtn;
    public Button SaveGesturesBtn;
    public Button LoadGesturesBtn;
    public InputField nameInputField;
    public Text text;

    [Space]
    public int valuesTracked;
    public int statesUsed;
    public int mode;
    
    [Space]
    public Transform leftHand;
    public Transform rightHand;

    List<Vector3> constantPositions;

    List<Vector3> rightHandPositions;
    List<Vector3> leftHandPositions;
    List<Vector3> rightHandRotations;
    List<Vector3> leftHandRotations;

    public List<Gesture> storedGestures;
    public Dictionary<string, int> gestureIndex;
    bool _isRecording;

    HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> hmm;
    ITopology vector;
    
    private int index;

    public string databaseFile;
    public GestureDatabase database;

    public ControllerTrail leftTrail;
    [Space]
    public ControllerTrail rightTrail;

    [Space]
    public CharacterAnimator animator;

    void Start ()
    {

        rightHandPositions = new List<Vector3>();
        leftHandPositions = new List<Vector3>();
        rightHandRotations = new List<Vector3>();
        leftHandRotations = new List<Vector3>();
        constantPositions = new List<Vector3>();

        storedGestures = new List<Gesture>();
        gestureIndex = new Dictionary<string, int>();

        StoreGestureBtn.onClick.AddListener(() => StoreGesture());
        LearnGesturesBtn.onClick.AddListener(() => LearnGesture());
        PredictGestureBtn.onClick.AddListener(() => CheckRecognized(rightHandPositions));
        SaveGesturesBtn.onClick.AddListener(() => SaveDatabase());
        LoadGesturesBtn.onClick.AddListener(() => LoadDatabase());
	}

	void FixedUpdate ()
    {
        if (_isRecording)
        {
            rightHandPositions.Add(rightHand.localPosition);
            leftHandPositions.Add(leftHand.localPosition);
            rightHandRotations.Add(rightHand.localEulerAngles);
            leftHandRotations.Add(leftHand.localEulerAngles);
            index++;
        }
        constantPositions.Add(rightHand.localPosition);
        if (constantPositions.Count >= 120)
        {
            constantPositions.RemoveAt(0);
        }
    }

    public void BeginRecording()
    {
        Debug.Log("Recording Begun!");
        rightHandPositions.Clear();
        leftHandPositions.Clear();
        rightHandRotations.Clear();
        leftHandRotations.Clear();
        _isRecording = true;
        index = 0;
        leftTrail.StartTrailing();
        rightTrail.StartTrailing();
        text.text = "Thinking";
    }

    public void EndRecording()
    {
        Debug.Log("Recording Ended!");
        _isRecording = false;
        leftTrail.StopTrailing();
        rightTrail.StopTrailing();
    }

    public void StoreGesture()
    {
        double[][] points = new double[rightHandPositions.Count][];
        switch(mode)
        {
            case 3:
                for (int i = 0; i < rightHandPositions.Count; i++)
                {
                    points[i] = new double[3] { rightHandPositions[i].x, rightHandPositions[i].y, rightHandPositions[i].z };
                }
                break;
            case 6:
                for (int i = 0; i < rightHandPositions.Count; i++)
                {
                    points[i] = new double[6] { rightHandPositions[i].x, rightHandPositions[i].y, rightHandPositions[i].z,
                                                rightHandRotations[i].x, rightHandRotations[i].y, rightHandRotations[i].z };
                }
                break;
            case 33:
                for (int i = 0; i < rightHandPositions.Count; i++)
                {
                    points[i] = new double[6] { rightHandPositions[i].x, rightHandPositions[i].y, rightHandPositions[i].z,
                                                leftHandPositions[i].x, leftHandPositions[i].y, leftHandPositions[i].z };
                }
                break;
            case 66:
                for (int i = 0; i < rightHandPositions.Count; i++)
                {
                    points[i] = new double[12] { rightHandPositions[i].x, rightHandPositions[i].y, rightHandPositions[i].z,
                                                 rightHandRotations[i].x, rightHandRotations[i].y, rightHandRotations[i].z,
                                                 leftHandPositions[i].x, leftHandPositions[i].y, leftHandPositions[i].z,
                                                 leftHandRotations[i].x, leftHandRotations[i].y, leftHandRotations[i].z };
                }
                break;
        }
        if (!gestureIndex.ContainsKey(nameInputField.text))
        {
            gestureIndex.Add(nameInputField.text, gestureIndex.Count);
        }
        Gesture gesture = new Gesture(points, nameInputField.text, gestureIndex[nameInputField.text]);
        storedGestures.Add(gesture);
        Debug.Log("Gesture Recorded as: " + nameInputField.text);
    }

    void LearnGesture()
    {
        double[][][] inputs = new double[storedGestures.Count][][];
        int[] outputs = new int[storedGestures.Count];

        for (int i = 0; i < inputs.Length; i++)
        {
            double[][] atemp = new double[storedGestures[i].points.Length][];
            for (int j = 0; j < storedGestures[i].points.Length; j++)
            {
                double[] btemp = new double[valuesTracked];
                for (int k = 0; k < valuesTracked; k++)
                {
                    btemp[k] = storedGestures[i].points[j][k];
                }
                atemp[j] = btemp;
            }

            inputs[i] = atemp;
            outputs[i] = storedGestures[i].index;
        }

        List<String> classes = new List<String>();

        int states = gestureIndex.Count;

        MultivariateNormalDistribution dist = new MultivariateNormalDistribution(valuesTracked);

        hmm = new HiddenMarkovClassifier<MultivariateNormalDistribution, double[]>
            (states, new Forward(statesUsed), dist);

        var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]>(hmm)
        {
            Learner = i => new BaumWelchLearning<MultivariateNormalDistribution, double[]>(hmm.Models[i])
            {
                Tolerance = 0.01,
                MaxIterations = 1,

                FittingOptions = new NormalOptions()
                {
                    Regularization = 1e-5
                }
            }
        };

        teacher.Empirical = true;
        teacher.Rejection = false;

        teacher.Learn(inputs, outputs);

        Debug.Log("Sequence Learned!");
    }

    public void LearnGesture(int valuesUsed,int modeUsed, int statesUsed)
    {
        double[][][] inputs = new double[storedGestures.Count][][];
        int[] outputs = new int[storedGestures.Count];

        for (int i = 0; i < inputs.Length; i++)
        {
            double[][] points = new double[storedGestures[i].points.Length][];
            switch (modeUsed)
            {
                case 3:
                    for (int j = 0; j < storedGestures[i].points.Length; j++)
                    {
                        points[j] = new double[3] { storedGestures[i].points[j][0], storedGestures[i].points[j][1], storedGestures[i].points[j][2] };
                    }
                    break;
                case 33:
                    for (int j = 0; j < storedGestures[i].points.Length; j++)
                    {
                        points[j] = new double[6] { storedGestures[i].points[j][0], storedGestures[i].points[j][1], storedGestures[i].points[j][2],
                                                storedGestures[i].points[j][6], storedGestures[i].points[j][7], storedGestures[i].points[j][8] };
                    }
                    break;
                case 6:
                    for (int j = 0; j < storedGestures[i].points.Length; j++)
                    {
                        points[j] = new double[6] { storedGestures[i].points[j][0], storedGestures[i].points[j][1], storedGestures[i].points[j][2],
                                                storedGestures[i].points[j][3], storedGestures[i].points[j][4], storedGestures[i].points[j][5] };
                    }
                    break;
                case 66:
                    for (int j = 0; j < storedGestures[i].points.Length; j++)
                    {
                        points[j] = new double[12] { storedGestures[i].points[j][0], storedGestures[i].points[j][1], storedGestures[i].points[j][2],
                                                 storedGestures[i].points[j][3], storedGestures[i].points[j][4], storedGestures[i].points[j][5],
                                                 storedGestures[i].points[j][6], storedGestures[i].points[j][7], storedGestures[i].points[j][8],
                                                 storedGestures[i].points[j][9], storedGestures[i].points[j][10], storedGestures[i].points[j][11] };
                    }
                    break;
            }

            inputs[i] = points;
            outputs[i] = storedGestures[i].index;
        }

        List<String> classes = new List<String>();

        int states = gestureIndex.Count;

        MultivariateNormalDistribution dist = new MultivariateNormalDistribution(valuesUsed);

        hmm = new HiddenMarkovClassifier<MultivariateNormalDistribution, double[]>
            (states, new Forward(statesUsed), dist);

        var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]>(hmm)
        {
            Learner = i => new BaumWelchLearning<MultivariateNormalDistribution, double[]>(hmm.Models[i])
            {
                Tolerance = 0.01,
                MaxIterations = 0,

                FittingOptions = new NormalOptions()
                {
                    Regularization = 1e-5
                }
            }
        };

        teacher.Empirical = true;
        teacher.Rejection = false;

        teacher.Learn(inputs, outputs);

        Debug.Log("Sequence Learned!");
    }

    public void ContinuousCheckRecognized()
    {
        Debug.Log("ContinuousChecking!");
        double[][] points = new double[constantPositions.Count][];
        switch (valuesTracked)
        {
            case 3:
                for (int i = 0; i < constantPositions.Count; i++)
                {
                    points[i] = new double[3] { constantPositions[i].x, constantPositions[i].y, constantPositions[i].z };
                }
                break;
            case 6:
                for (int i = 0; i < constantPositions.Count; i++)
                {
                    points[i] = new double[6] { constantPositions[i].x, constantPositions[i].y, constantPositions[i].z,
                                                rightHandRotations[i].x, rightHandRotations[i].y, rightHandRotations[i].z };
                }
                break;
            case 12:
                for (int i = 0; i < constantPositions.Count; i++)
                {
                    points[i] = new double[12] { constantPositions[i].x, constantPositions[i].y, constantPositions[i].z,
                                                 rightHandRotations[i].x, rightHandRotations[i].y, rightHandRotations[i].z,
                                                 leftHandPositions[i].x, leftHandPositions[i].y, leftHandPositions[i].z,
                                                 leftHandRotations[i].x, leftHandRotations[i].y, leftHandRotations[i].z };
                }
                break;
        }

        double[] probabilities = hmm.Probabilities(points);
        double[] likelihoods = hmm.LogLikelihoods(points);
        double[] scores = hmm.Scores(points);
        double bestFit = Mathf.NegativeInfinity;
        for (int i = 0; i < likelihoods.Length; i++)
        {
            if (likelihoods[i] > bestFit)
            { bestFit = likelihoods[i]; }
        }
        Debug.Log(bestFit);
        if (bestFit >= 0)
        {
            int decision = hmm.Decide(points);
            string value = string.Empty;
            foreach (KeyValuePair<string, int> item in gestureIndex)
            {
                if (item.Value == decision)
                { value = item.Key; }
            }
            text.text = value;
            nameInputField.text = value;
            Debug.Log("Did you write a: " + value + "?");

            foreach (Gesture gesture in storedGestures)
            {
                if (gesture.name == value)
                {
                    //animator.BeginAnimation(gesture.points);
                }
            }
        }
    }

    public void CheckRecognized(List<Vector3> positions)
    {
        double[][] points = new double[positions.Count][];
        switch (valuesTracked)
        {
            case 3:
                for (int i = 0; i < rightHandPositions.Count; i++)
                {
                    points[i] = new double[3] { rightHandPositions[i].x, rightHandPositions[i].y, rightHandPositions[i].z };
                }
                break;
            case 6:
                for (int i = 0; i < rightHandPositions.Count; i++)
                {
                    points[i] = new double[6] { rightHandPositions[i].x, rightHandPositions[i].y, rightHandPositions[i].z,
                                                rightHandRotations[i].x, rightHandRotations[i].y, rightHandRotations[i].z };
                }
                break;
            case 12:
                for (int i = 0; i < rightHandPositions.Count; i++)
                {
                    points[i] = new double[12] { rightHandPositions[i].x, rightHandPositions[i].y, rightHandPositions[i].z,
                                                 rightHandRotations[i].x, rightHandRotations[i].y, rightHandRotations[i].z,
                                                 leftHandPositions[i].x, leftHandPositions[i].y, leftHandPositions[i].z,
                                                 leftHandRotations[i].x, leftHandRotations[i].y, leftHandRotations[i].z };
                }
                break;
        }
        double[] probabilities = hmm.Probabilities(points);
        double[] likelihoods = hmm.LogLikelihoods(points);
        double[] scores = hmm.Scores(points);

        //double[] likelihoods = hmm.LogLikelihoods(points);
        double bestFit = 0;
        for (int i = 0; i < likelihoods.Length; i++)
        {
            Debug.Log(likelihoods[i]);
            if (likelihoods[i] > bestFit)
            { bestFit = likelihoods[i]; }
        }

        int decision = hmm.Decide(points);
        string value = string.Empty;
        foreach (KeyValuePair<string, int> item in gestureIndex)
        {
            if (item.Value == decision)
            { value = item.Key; }
        }
        text.text = value;
        nameInputField.text = value;
        Debug.Log("Did you write a: " + value + "?");

        foreach(Gesture gesture in storedGestures)
        {
            if (gesture.name == value)
            {
                //animator.BeginAnimation(gesture.points);
            }
        }
    }

    public string CheckRecognized(double[][] positions, int valuesUsed)
    {
        Debug.Log("Checking sequence!");

        double[][] points = new double[positions.Length][];

        switch (valuesUsed)
        {
            case 3:
                for (int i = 0; i < positions.Length; i++)
                {
                    points[i] = new double[3] { positions[i][0], positions[i][1], positions[i][2] };
                }
                break;
            case 33:
                for (int i = 0; i < positions.Length; i++)
                {
                    points[i] = new double[6] { positions[i][0], positions[i][1], positions[i][2],
                                                positions[i][6], positions[i][7], positions[i][8] };
                }
                break;
            case 6:
                for (int i = 0; i < positions.Length; i++)
                {
                    points[i] = new double[6] { positions[i][0], positions[i][1], positions[i][2],
                                                positions[i][3], positions[i][4], positions[i][5] };
                }
                break;
            case 66:
                for (int i = 0; i < positions.Length; i++)
                {
                    points[i] = new double[12] { positions[i][0], positions[i][1], positions[i][2],
                                                 positions[i][3], positions[i][4], positions[i][5],
                                                 positions[i][6], positions[i][7], positions[i][8],
                                                 positions[i][9], positions[i][10], positions[i][11] };
                }
                break;
        }

        int decision = hmm.Decide(points);
        string value = string.Empty;
        foreach (KeyValuePair<string, int> item in gestureIndex)
        {
            if (item.Value == decision)
            { value = item.Key; }
        }
        text.text = value;
        nameInputField.text = value;
        Debug.Log("Did you write a: " + value + "?");
        return value;
    }

    void SaveDatabase()
    {
        database.CheckDatabaseExists(databaseFile);
        database.Gestures = new BindingList<Gesture>(storedGestures);
        var stream = new FileStream(databaseFile, FileMode.Open);
        database.Save(stream);
        stream.Close();
        Debug.Log("Database Saved!");
    }

    public void LoadDatabase()
    {
        database.CheckDatabaseExists(databaseFile);
        var stream = new FileStream(databaseFile, FileMode.Open);
        database.Load(stream);
        storedGestures = database.Gestures.ToList();
        gestureIndex = new Dictionary<string, int>();
        if (storedGestures.Count > 0)
        {
            for (int i = 0; i < storedGestures.Count; i++)
            {
                gestureIndex[storedGestures[i].name] = storedGestures[i].index;
            }
        }
        else
        {
            Debug.Log("GOT NO DATA MATE");
        }
        stream.Close();
        Debug.Log("Gesture Database Loaded!");
    }

    public List<Gesture> LoadDatabase(string name)
    {
        database.CheckDatabaseExists(name);
        var stream = new FileStream(name, FileMode.Open);
        database.Load(stream);
        List<Gesture> gestures = database.Gestures.ToList();
        stream.Close();
        Debug.Log("Test Case Database Loaded!");
        return gestures;
    }
}

[Serializable]
public struct Gesture
{
    public double[][] points;
    public string name;
    public int index;

    public Gesture(double[][] points, string name, int index)
    {
        this.points = points;
        this.name = name;
        this.index = index;
    }
}