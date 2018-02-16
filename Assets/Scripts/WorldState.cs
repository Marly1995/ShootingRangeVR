using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState
{
    private static WorldState instance = null;

    private WorldState() { }

    public static WorldState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WorldState();
            }
            return instance;
        }
    }

    private static float playerSize;
    public static float PlayerSize
    {
        get { return playerSize; }
        set { playerSize = value; }
    }

    private static Vector3 playerKnockback;
    public static Vector3 PlayerKnockback
    {
        get { return playerKnockback; }
        set { playerKnockback = value; }
    }

    private static bool meleeHit;
    public static bool MeleeHit
    {
        get { return meleeHit; }
        set { meleeHit = value; }
    }

    private static float trauma;
    public static float Trauma
    {
        get { return trauma; }
        set { trauma = value; }
    }

    private static float cameraAngle;
    public static float CameraANgle
    {
        get { return cameraAngle; }
        set { cameraAngle = value; }
    }

    private static int score;
    public static int Score
    {
        get { return score; }
        set { score = value; }
    }

    private static int waves;
    public static int Waves
    {
        get { return waves; }
        set { waves = value; }
    }
}
