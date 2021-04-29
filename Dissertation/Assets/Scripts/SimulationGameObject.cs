using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationGameObject : MonoBehaviour
{
    public CurrentGeneration currentGeneration;
    public float[][] map;
    public Players[] players;
    public OldPlayers[] oldPlayers;
    public GameObject MapTile;
    public GameObject GoalTile;
    public GameObject PlayerPrefab;
    public GameObject floor;
    public List<GameObject> playerModels = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < map.Length; i++)
        {
            float[] item = map[i];
            for (int j = 0; j < item.Length; j++)
            {
                switch (item[j])
                {
                    case 0f:
                        if (currentGeneration == CurrentGeneration.GEN1)
                            Instantiate(MapTile, new Vector3(i, 0, j), Quaternion.identity, this.transform);
                        break;
                    case 1f:               
                        if(currentGeneration != CurrentGeneration.GEN1 )
                            Instantiate(MapTile, new Vector3(i, 0, j), Quaternion.identity, this.transform);
                        break;
                    case 2f:
                            Instantiate(GoalTile, new Vector3(i, 0, j), Quaternion.identity, this.transform);
                        break;

                    default:
                        break;
                }

            }
        }

        floor.transform.localScale = new Vector3(0.035f * map.Length, 0.4f, 0.035f * map[0].Length);
        floor.transform.position = new Vector3(map.Length / 2, -0.5f, map[0].Length / 2);

        switch (currentGeneration)
        {
            case CurrentGeneration.GEN1:            
                foreach (var item in oldPlayers)
                {
                    GameObject player;
                    player = Instantiate(PlayerPrefab, this.transform);
                    player.GetComponent<PlayersMovement>().currentGeneration = currentGeneration;
                    player.GetComponent<PlayersMovement>().thisOldPlayer = item;
                    playerModels.Add(player);
                }
                break;
            case CurrentGeneration.GEN2:
            case CurrentGeneration.GEN3:
            case CurrentGeneration.GEN4:
                foreach (var item in players)
                {
                    GameObject player;
                    player = Instantiate(PlayerPrefab, this.transform);
                    player.GetComponent<PlayersMovement>().currentGeneration = currentGeneration;
                    player.GetComponent<PlayersMovement>().thisPlayer = item;
                    playerModels.Add(player);
                }
                break;
            case CurrentGeneration.NULL:
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void animationChange(bool walking)
    {
        foreach (var item in playerModels)
        {
            if (item.GetComponentsInChildren<Animator>() != null)
            {
                foreach (var item2 in item.GetComponentsInChildren<Animator>())
                {
                    item2.SetBool("inGame", walking);
                }
            }
        }

    }

    public void animatorStop(bool stop)
    {
        foreach (var item in playerModels)
        {
            if (item.GetComponentsInChildren<Animator>() != null)
            {
                foreach (var item2 in item.GetComponentsInChildren<Animator>())
                {
                    if(stop)
                    item2.SetFloat("speed", 0);
                    else
                    item2.SetFloat("speed", 1);
                }
            }
        }

    }
}
