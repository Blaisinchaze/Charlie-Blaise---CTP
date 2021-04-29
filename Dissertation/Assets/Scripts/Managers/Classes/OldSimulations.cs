using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class OldPlayers
{
    public string type;
    public float[] xs;
    public float[] ys;
}

[System.Serializable]
public class OldSimulations
{
    [System.Serializable]
    public struct OldSimulationClass
    {
        public float[][] map;
        public OldPlayers[] players;
    }
    public OldSimulationClass[] simulations;

    public static OldSimulations CreateFromJSON(string jsonString)
    {
        return JsonConvert.DeserializeObject<OldSimulations>(jsonString);
        //return JsonUtility.FromJson<Simulations>(jsonString);
    }
}

