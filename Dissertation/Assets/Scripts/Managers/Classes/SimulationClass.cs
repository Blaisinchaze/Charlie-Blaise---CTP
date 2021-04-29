using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Players
{
    public int team;
    public float[][] coords;
}

[System.Serializable]
public class Teams
{
    public string name;
    public float[] scores;
    public float[] goals;
}

[System.Serializable]
public class Simulations
{
    //[System.Serializable]
    //public struct SimulationClass
    //{
        public float[][] map;
        public Players[] players;
        public Teams[] teams;
    //}
    //public SimulationClass[] simulations;

    public static Simulations CreateFromJSON(string jsonString)
    {
        return JsonConvert.DeserializeObject<Simulations>(jsonString);
        //return JsonUtility.FromJson<Simulations>(jsonString);
    }
}

