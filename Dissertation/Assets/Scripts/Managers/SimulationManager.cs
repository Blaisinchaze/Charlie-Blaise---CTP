using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;

public enum CurrentGeneration
{
    GEN1,
    GEN2,
    GEN3,
    GEN4,
    NULL
}
public enum PlayStates
{
    Menu,
    InSim,
    Null
}
public class SimulationManager : MonoBehaviour
{

    public static SimulationManager myInstance = null;

    public CurrentGeneration currentGeneration;

    public Simulations sims;
    public OldSimulations oldSims;
    public GameObject simulationPrefab;
    public float gameTime;
    public int simToSim;
    public bool timeRunning;
    public bool forwards;

    public PlayStates states;
    public GameObject tabHolder;
    public GameObject tabPrefab;
    public CameraController cController;
    private GameObject demo;
    public GameObject inSimUI;
    public GameObject inMenuUI;
    [SerializeField] SimInformation simInformation;
    public float GameSpeed = 1;
    private float maxTurns;
    public TMP_InputField speedText;
    // Start is called before the first frame update
    void Start()
    {    
        { myInstance = this; }
        cController = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CameraController>();
        changeSim("0");
    }

    // Update is called once per frame
    void Update()
    {
        switch (states)
        {
            case PlayStates.Menu:
                timeRunning = false;
                forwards = true;
                GameSpeed = 1;
                break;
            case PlayStates.InSim:
                if (timeRunning)
                {
                    gameTime += Time.deltaTime * GameSpeed;                   
                    gameTime =  Mathf.Clamp(gameTime,0,maxTurns/10);
                }    

                break;
            case PlayStates.Null:
                break;
            default:
                break;
        }
    }

    public void Forwards(bool fwards)
    {
        forwards = fwards;
    }
    public void Playing(bool playing)
    {
        timeRunning = playing;
    }
    public void changeSim(string simNumber)
    {
        switch (currentGeneration)
        {
            case CurrentGeneration.GEN1:
                if (File.Exists(Application.dataPath + "/Resources/GameRecordings1/recordings_tabbed.json") && states == PlayStates.Menu)
                {
                    Destroy(demo);
                    string json = File.ReadAllText(Application.dataPath + "/Resources/GameRecordings1/recordings_tabbed.json");
                    oldSims = (OldSimulations.CreateFromJSON(json));
                    demo = Instantiate(simulationPrefab, this.transform);
                    demo.GetComponent<SimulationGameObject>().currentGeneration = currentGeneration;
                    demo.GetComponent<SimulationGameObject>().map = oldSims.simulations[int.Parse(simNumber)].map;
                    demo.GetComponent<SimulationGameObject>().oldPlayers = oldSims.simulations[int.Parse(simNumber)].players;
                    changeSimInfo(simNumber);
                }
                break;
            case CurrentGeneration.GEN2:
            case CurrentGeneration.GEN3:
            case CurrentGeneration.GEN4:
                string filepath = "";
                switch (currentGeneration)
                {
                    case CurrentGeneration.GEN2:
                        filepath = "/Resources/GameRecordings2/epoch_" + simNumber + ".json";
                        break;
                    case CurrentGeneration.GEN3:
                        filepath = "/Resources/GameRecordings3/epoch_" + simNumber + "_game_0.json";
                        break;
                    case CurrentGeneration.GEN4:
                        filepath = "/Resources/GameRecordings4/epoch_" + simNumber + "_game_0.json";
                        break;
                    case CurrentGeneration.NULL:
                        break;
                    default:
                        break;
                }
                if (File.Exists(Application.dataPath + filepath) && states == PlayStates.Menu)
                {
                    Destroy(demo);
                    string json = File.ReadAllText(Application.dataPath + filepath);
                    sims = (Simulations.CreateFromJSON(json));
                    demo = Instantiate(simulationPrefab, this.transform);
                    demo.GetComponent<SimulationGameObject>().currentGeneration = currentGeneration;
                    demo.GetComponent<SimulationGameObject>().map = sims.map;
                    demo.GetComponent<SimulationGameObject>().players = sims.players;
                    changeSimInfo(simNumber);
                }
                break;
            case CurrentGeneration.NULL:
                break;
            default:
                break;
        }


    }

    public void ChangeGeneration(int gen)
    {
        currentGeneration = (CurrentGeneration)gen;
        changeSim("0");
    }

    private void changeSimInfo(string simNumber)
    {
        simInformation.simNumber = simNumber;
        int numberOfTurnsTaken = 0;        
        int numberOfAttackers = 0;
        int numberOfDefenders = 0;        
        int numberOfGoals = 0;
        switch (currentGeneration)
        {
            case CurrentGeneration.GEN1:
                simInformation.attackerPoints = 0;
                foreach (var item in oldSims.simulations[int.Parse(simNumber)].players)
                {
                    if(item.type == "attacker")
                    {
                        numberOfAttackers++;
                    }
                    if(item.type == "defender")
                    {
                        numberOfDefenders++;
                    }
                    if (item.xs.Length > numberOfTurnsTaken)
                    {
                        numberOfTurnsTaken = item.xs.Length;
                        maxTurns = numberOfTurnsTaken;
                    }
                       
                }
                foreach (var item in oldSims.simulations[int.Parse(simNumber)].map)
                {
                    foreach (var item2 in item)
                    {
                        if (item2 == 2)
                        {
                            numberOfGoals++;
                        }
                    }
                }
                break;
            case CurrentGeneration.GEN2:
            case CurrentGeneration.GEN3:        
            case CurrentGeneration.GEN4:        
                if(sims.teams[0].scores != null)
                    simInformation.attackerPoints = (int)sims.teams[0].scores[sims.teams[0].scores.Length - 1];
                if (sims.teams[0].goals != null)
                    simInformation.attackerPoints = (int)sims.teams[0].goals[sims.teams[0].goals.Length - 1];
                foreach (var item in sims.players)
                {
                    if (item.team == 0)
                    {
                        numberOfAttackers++;

                    }
                    if (item.team == 1)
                    {
                        numberOfDefenders++;
                    }
                    if (item.coords.Length > numberOfTurnsTaken)
                    {
                        numberOfTurnsTaken = item.coords.Length;
                        maxTurns = numberOfTurnsTaken;
                    }

                }        
                foreach(var item in sims.map)
                {
                    foreach(var item2 in item)
                    {
                        if(item2 == 2)
                        {
                            numberOfGoals++;
                        }
                    }
                }
                break;
            case CurrentGeneration.NULL:
                break;
            default:
                break;
        }

        simInformation.numberOfAttackers = numberOfAttackers;
        simInformation.numberOfDefenders = numberOfDefenders;
        simInformation.turnsTaken = numberOfTurnsTaken;
        simInformation.numberOfGoals = numberOfGoals;
    }

    public void changeState()
    {
        if(states == PlayStates.InSim)
        {
            states = PlayStates.Menu;

            inMenuUI.SetActive(true);
            inSimUI.SetActive(false);            
            gameTime = 0;
            demo.GetComponent<SimulationGameObject>().animatorStop(false);
            demo.GetComponent<SimulationGameObject>().animationChange(false);
        }
        else
        {
            demo.GetComponent<SimulationGameObject>().animationChange(true);
            states = PlayStates.InSim;
            timeRunning = true;

            inMenuUI.SetActive(false);
            inSimUI.SetActive(true);
            speedText.text = "";
        }
    }

    public void Pause(bool pause)
    {
        if(pause)
        {
            Playing(false);
            demo.GetComponent<SimulationGameObject>().animatorStop(true);
        }
        else
        {
            Playing(true);
            demo.GetComponent<SimulationGameObject>().animatorStop(false);
        }
    }

    public void SetGameTime(float time)
    {
        gameTime = time;
    }

    public void SetGameSpeed(string time)
    {
        float result;
        if(float.TryParse(time, out result))
        {
            GameSpeed = float.Parse(time);
        }

    }
}
