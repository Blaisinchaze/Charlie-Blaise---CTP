using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerType
{
    attacker,
    defender,
    NULL
}
public class PlayersMovement : MonoBehaviour
{
    public CurrentGeneration currentGeneration;
    private int playerType;
    public GameObject knightModel;
    public GameObject SkeletonModel;
    public Players thisPlayer;
    public OldPlayers thisOldPlayer;
    private int gameTimer;
    private float decimalTimer;
    public float speed;
    SimulationManager manager;
    public GameObject AttackerIndicator;
    public GameObject DefenderIndicator;
    public Camera cam;
    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("SimulationManager").GetComponent<SimulationManager>();
        cam = Camera.main;
        switch (currentGeneration)
        {
            case CurrentGeneration.GEN1:
                if(thisOldPlayer.type == "attacker")
                {
                    playerType = 0;
                }
                else
                {
                    playerType = 1;
                }
                transform.position = new Vector3(thisOldPlayer.xs[0], 0, thisOldPlayer.ys[0]);
                break;
            case CurrentGeneration.GEN2:
            case CurrentGeneration.GEN3:        
            case CurrentGeneration.GEN4:        
                playerType = (thisPlayer.team);        
                transform.position = new Vector3(thisPlayer.coords[0][0], 0, thisPlayer.coords[0][1]);
                break;
            case CurrentGeneration.NULL:
                break;
            default:
                break;
        }

        switch (playerType)
        {
            case 0:
                if (currentGeneration == CurrentGeneration.GEN4)
                {
                    SkeletonModel.SetActive(true);
                    knightModel.SetActive(false);
                    DefenderIndicator.SetActive(true);
                    AttackerIndicator.SetActive(false);
                }
                else
                {
                    DefenderIndicator.SetActive(false);
                    AttackerIndicator.SetActive(true);
                    SkeletonModel.SetActive(false);
                    knightModel.SetActive(true);
                }

                break;
            case 1:
                if (currentGeneration == CurrentGeneration.GEN4)
                {
                    SkeletonModel.SetActive(false);
                    knightModel.SetActive(true);
                    DefenderIndicator.SetActive(false);
                    AttackerIndicator.SetActive(true);
                }
                else
                {
                    SkeletonModel.SetActive(true);
                    knightModel.SetActive(false);
                    DefenderIndicator.SetActive(true);
                    AttackerIndicator.SetActive(false);
                }
                break;
            default:
                break;
        }

    }
    private void Update()
    {
        float step = speed * Time.deltaTime ;
        decimalTimer += 1 * step;
        gameTimer = Mathf.RoundToInt(manager.gameTime * 10);
        if (DefenderIndicator.activeSelf)
            DefenderIndicator.transform.LookAt(cam.transform);        
        if (AttackerIndicator.activeSelf)
            AttackerIndicator.transform.LookAt(cam.transform);
        switch (currentGeneration)
        {
            case CurrentGeneration.GEN1:
                if (gameTimer < thisOldPlayer.xs.Length)
                {
                    Vector3 nextLocation = new Vector3(thisOldPlayer.xs[gameTimer], 0, thisOldPlayer.ys[gameTimer]);
                    transform.LookAt(nextLocation);
                    transform.position = Vector3.Lerp(transform.position, nextLocation, step);
                }
                break;
            case CurrentGeneration.GEN2:
            case CurrentGeneration.GEN3:            
            case CurrentGeneration.GEN4:            
                if(gameTimer<thisPlayer.coords.Length)
                {
                    Vector3 nextLocation = new Vector3(thisPlayer.coords[gameTimer][1], 0, thisPlayer.coords[gameTimer][0]);
                    Vector3 previousLocation = nextLocation;
                    if (gameTimer < thisPlayer.coords.Length -1)
                        previousLocation = new Vector3(thisPlayer.coords[gameTimer+1][1], 0, thisPlayer.coords[gameTimer+1][0]);
                    if(manager.GameSpeed > 0)
                    {
                        transform.LookAt(nextLocation);
                    }
                    if(manager.GameSpeed < 0)
                    {
                        transform.LookAt(previousLocation);
                    }
                    transform.position = Vector3.Lerp(transform.position, nextLocation, step);
                }
                break;
            case CurrentGeneration.NULL:
                break;
            default:
                break;
        }
    }

}
