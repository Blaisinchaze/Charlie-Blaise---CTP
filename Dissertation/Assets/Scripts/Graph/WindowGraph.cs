using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class WindowGraph : MonoBehaviour
{
    public CurrentGeneration currentGeneration;
    public class FloatEvent : UnityEvent<float> { } //empty class; just needs to exist
    private SimulationManager simulationManager;
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform simInfo;
    List<float> attackersResults = new List<float>();
    List<float> defendersResults = new List<float>();            
    List<float> turnTimerResults = new List<float>();            
    [SerializeField] private Color attackerColor;
    [SerializeField] private Color defenderColor;
    [SerializeField] private Color turnTimerColor;
    float averageAttackerScore;
    float averageDefenderScore;
    float averageTurnTimer;

    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;    
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private RectTransform dotsHolder;
    private RectTransform linesHolder;
    private RectTransform labelsHolder;

    private int yMaximum =0;
    private int yMinimum =0;
    float graphHeight;

    float xSize = 20f;

    [SerializeField] private int xIntervals;
    [SerializeField] private int lastEpoch;
    [SerializeField] private int startingEpoch;
    [SerializeField] private int endingEpoch;

    private OldSimulations gen1Sims;
    private List<Simulations> gen2Sims = new List<Simulations>();
    private List<Simulations> gen3Sims = new List<Simulations>();
    private List<Simulations> gen4Sims = new List<Simulations>();


    private int[] gen1Intervals = new int[] {1};
    private int[] gen2Intervals = new int[] { 20, 1 };
    private int[] gen3Intervals = new int[] { 20, 1 };
    private int[] gen4Intervals = new int[] { 20, 1 };

    public int currentXinterval = 0;
    public static DirectoryInfo directory;
    private string dataPath;

    public GameObject loadingText;
    public TextMeshProUGUI counter;

    public int currentlyLoaded = 0;
    public int maxLoaded = 0;
    public void ChangeGeneration(int gen)
    {
        currentGeneration = (CurrentGeneration)gen;
        CreateStartingGraph();
        StartCoroutine(LoadAndGenerateGraph(true));
    }

    private void Awake()
    {

        directory = new DirectoryInfo(Application.dataPath);
        simulationManager = GameObject.Find("SimulationManager").GetComponent<SimulationManager>();

        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        simInfo = transform.Find("SimInfo").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();        
        dashTemplateX = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();
        dotsHolder = graphContainer.Find("Dots").GetComponent<RectTransform>();
        linesHolder = graphContainer.Find("Lines").GetComponent<RectTransform>();
        labelsHolder = graphContainer.Find("Axis").GetComponent<RectTransform>();
        CountGames();
        StartCoroutine(LoadAndGenerateGraph(true));


    }

    private void CountGames()
    {
        string json = File.ReadAllText(directory + "/Resources/GameRecordings1/recordings_tabbed.json");
        gen1Sims = (OldSimulations.CreateFromJSON(json));
        maxLoaded += gen1Sims.simulations.Length;

        DirectoryInfo dir = new DirectoryInfo(directory + "/Resources/GameRecordings2");
        FileInfo[] info = dir.GetFiles("*.json");
        maxLoaded += info.Length;

        dir = new DirectoryInfo(directory + "/Resources/GameRecordings3");
        info = dir.GetFiles("*.json");
        maxLoaded += info.Length;        
        
        dir = new DirectoryInfo(directory + "/Resources/GameRecordings4");
        info = dir.GetFiles("*.json");
        maxLoaded += info.Length;
    }

    public void resetGraph()
    {
        StartCoroutine(LoadAndGenerateGraph(true));
    }

    public void zoomOut()
    {
        currentXinterval--;
        startingEpoch = lastEpoch;
        switch (currentGeneration)
        {
            case CurrentGeneration.GEN1:
                endingEpoch = gen1Sims.simulations.Length - 1;
                break;
            case CurrentGeneration.GEN2:
                endingEpoch = gen2Sims.Count - 1;
                break;
            case CurrentGeneration.GEN3:
                endingEpoch = gen3Sims.Count - 1;
                break;
            case CurrentGeneration.GEN4:
                endingEpoch = gen4Sims.Count - 1;
                break;
            case CurrentGeneration.NULL:
                break;
            default:
                break;
        }
        StartCoroutine(LoadAndGenerateGraph(false));
    }

    IEnumerator LoadAndGenerateGraph(bool reset)
    {
        //StopCoroutine(LoadAllData());
        loadingText.SetActive(true);
        yield return StartCoroutine(LoadSpecificData());
        loadingText.SetActive(false);
        if(reset)
        {
            CreateStartingGraph();
            reset = false;
        }
        CreateWholeGraph(startingEpoch, endingEpoch);        
        //yield return LoadAllData();
    }

    IEnumerator LoadSpecificData()
    {

        bool done = false;
        new Thread(() => {
            DirectoryInfo dir;
            FileInfo[] info;                    
            int startingGame = 0;
            //int game = 0;
            switch (currentGeneration)
            {
                case CurrentGeneration.GEN1:
                    break;
                case CurrentGeneration.GEN2:            
                    dir = new DirectoryInfo(directory + "/Resources/GameRecordings2");
                    info = dir.GetFiles("*.json");
                    startingGame = 0;
                    if (gen2Sims != null)
                        startingGame = gen2Sims.Count;
                    for (int i = startingGame; i < info.Length - 1; i++)
                    {
                        gen2Sims.Add(Simulations.CreateFromJSON(
                            File.ReadAllText(directory + "/Resources/GameRecordings2/epoch_" + (i) + ".json")));
                    }
                    break;
                case CurrentGeneration.GEN3:            
                    dir = new DirectoryInfo(directory + "/Resources/GameRecordings3");
                    info = dir.GetFiles("*.json");
                    startingGame = 0;
                    //game = 0;
                    if (gen3Sims != null)
                    {
                        //startingGame = gen3Sims.Count;
                        //float magicNumber = startingGame / 20;
                        //float epoch = Mathf.Floor(magicNumber);

                        //game = (int)((magicNumber % 1) * 20);
                        startingGame = gen3Sims.Count;
                    }
                    for (int i = startingGame; i < info.Length - 1; i++)
                    {
                            gen3Sims.Add(Simulations.CreateFromJSON(File.ReadAllText(
                                directory + "/Resources/GameRecordings3/epoch_" + (i) + "_game_0.json")));
                    }
                    break;
                case CurrentGeneration.GEN4:            
                    dir = new DirectoryInfo(directory + "/Resources/GameRecordings4");
                    info = dir.GetFiles("*.json");
                    startingGame = 0;
                    //game = 0;
                    if (gen4Sims != null)
                    {
                        //startingGame = gen4Sims.Count;
                        //float magicNumber = startingGame / 20;
                        //float epoch = Mathf.Floor(magicNumber);

                        //game = (int)((magicNumber % 1) * 20);
                        startingGame = gen4Sims.Count;
                    }
                    for (int i = startingGame; i < info.Length - 1; i++)
                    {
                            gen4Sims.Add(Simulations.CreateFromJSON(File.ReadAllText(
                                directory + "/Resources/GameRecordings4/epoch_" + (i) + "_game_0.json")));
                    }

                    break;
                case CurrentGeneration.NULL:
                    break;
                default:
                    break;                    

            }                    
            done = true;
        }).Start();

        // Do nothing on each frame until the thread is done
        while (!done)
        {
            yield return null;
        }

    }

    IEnumerator LoadAllData()
    {

        bool done = false;
        new Thread(() => {
            DirectoryInfo dir;
            FileInfo[] info;
            int startingGame = 0;
            int game = 0;
            dir = new DirectoryInfo(directory + "/Resources/GameRecordings2");
            info = dir.GetFiles("*.json");
            startingGame = 0;
            if (gen2Sims != null)
                startingGame = gen2Sims.Count;
            for (int i = startingGame; i < info.Length - 1; i++)
            {
                gen2Sims.Add(Simulations.CreateFromJSON(
                    File.ReadAllText(directory + "/Resources/GameRecordings2/epoch_" + (i) + ".json")));
            }

         
            dir = new DirectoryInfo(directory + "/Resources/GameRecordings3");
            info = dir.GetFiles("*.json");
            startingGame = 0;
            game = 0;
            if (gen3Sims != null)
            {
                startingGame = gen3Sims.Count;
                float magicNumber = startingGame / 20;
                float epoch = Mathf.Floor(magicNumber);

                game = (int)((magicNumber % 1) * 20);
                startingGame = (int)epoch;
            }
            for (int i = startingGame; i < (info.Length / 20) - 1; i++)
            {
                for (int k = game; k < 20; k++)
                {
                    gen3Sims.Add(Simulations.CreateFromJSON(File.ReadAllText(
                        directory + "/Resources/GameRecordings3/epoch_" + (i) + "_game_" + (k) + ".json")));
                }
            }           
                    dir = new DirectoryInfo(directory + "/Resources/GameRecordings4");
            info = dir.GetFiles("*.json");
            startingGame = 0;
            game = 0;
            if (gen4Sims != null)
            {
                startingGame = gen4Sims.Count;
                float magicNumber = startingGame / 20;
                float epoch = Mathf.Floor(magicNumber);

                game = (int)((magicNumber % 1) * 20);
                startingGame = (int)epoch;
            }
            for (int i = startingGame; i < ((info.Length - 275) / 20) - 1; i++)
            {
                for (int k = game; k < 20; k++)
                {
                    gen4Sims.Add(Simulations.CreateFromJSON(File.ReadAllText(
                        directory + "/Resources/GameRecordings4/epoch_" + (i) + "_game_" + (k) + ".json")));
                }
            }
            done = true;

        }).Start();

        // Do nothing on each frame until the thread is done
        while (!done)
        {
            yield return null;
        }

    }

    private void Update()
    {
        if (counter.IsActive())
        {
            counter.text = (50 + gen2Sims.Count + gen3Sims.Count + gen4Sims.Count) + " / " + maxLoaded; 
        }

    }

    private void ClearGraph()
    {
        int children = dotsHolder.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            Destroy(dotsHolder.transform.GetChild(i).gameObject);
        }
        children = linesHolder.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            Destroy(linesHolder.transform.GetChild(i).gameObject);
        }
        children = labelsHolder.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            Destroy(labelsHolder.transform.GetChild(i).gameObject);
        }
    }

    public void CreateStartingGraph()
    {
        currentXinterval = 0;
        startingEpoch = 0;
        switch (currentGeneration)
        {
            case CurrentGeneration.GEN1:
                endingEpoch = gen1Sims.simulations.Length - 1;
                break;
            case CurrentGeneration.GEN2:
                endingEpoch = gen2Sims.Count - 1;
                break;
            case CurrentGeneration.GEN3:
                endingEpoch = gen3Sims.Count - 1;
                break;
            case CurrentGeneration.GEN4:
                endingEpoch = gen4Sims.Count - 1;
                break;
            case CurrentGeneration.NULL:
                break;
            default:
                break;
        }
    }

    private void CreateWholeGraph(int firstEpoch, int lastEpoch)
    {
        //lastEpoch = startingEpoch;
        startingEpoch = firstEpoch;
        switch (currentGeneration)
        {
            case CurrentGeneration.GEN1:
                xIntervals = gen1Intervals[currentXinterval];
                break;
            case CurrentGeneration.GEN2:
                xIntervals = gen2Intervals[currentXinterval];
                break;
            case CurrentGeneration.GEN3:
                xIntervals = gen3Intervals[currentXinterval];
                break;
            case CurrentGeneration.GEN4:
                xIntervals = gen4Intervals[currentXinterval];
                break;
            case CurrentGeneration.NULL:
                break;
            default:
                break;
        }
        currentXinterval++;
        ClearGraph();
        GenerateResults(firstEpoch, lastEpoch, xIntervals);
        ShowGraph(turnTimerResults, turnTimerColor);
        ShowGraph(attackersResults, attackerColor);
        ShowGraph(defendersResults, defenderColor);
        ShowGraphAxis(attackersResults);
    }

    private void GenerateResults(int firstEpoch, int lastEpoch, int intervals)
    {
        attackersResults.Clear();
        defendersResults.Clear();
        turnTimerResults.Clear();
        int j =1;
        yMaximum = 0;
        yMinimum = 100;
        averageAttackerScore = 0;
        averageDefenderScore = 0;
        averageTurnTimer = 0;
        for (int i = firstEpoch; i < lastEpoch; i++)
        {

            switch (currentGeneration)
            {
                case CurrentGeneration.GEN1:

                    for (int k = 0; k < 2; k++)
                    {
                        if(Mathf.RoundToInt(gen1Sims.simulations[k].players[0].xs[gen1Sims.simulations[k].players[0].xs.Length - 1]) == 1 &&
                            Mathf.RoundToInt(gen1Sims.simulations[k].players[0].ys[gen1Sims.simulations[k].players[0].ys.Length - 1]) == 5)
                        {
                            averageAttackerScore++;
                        }
                        else
                        {
                            averageDefenderScore++;
                        }
                    }
                    break;
                case CurrentGeneration.GEN2:
                case CurrentGeneration.GEN3:
                case CurrentGeneration.GEN4:
                    List<Simulations> simulations = null;
                    if(currentGeneration == CurrentGeneration.GEN2)
                    {
                        simulations = gen2Sims;

                    }
                    if (currentGeneration == CurrentGeneration.GEN3)
                    {
                        simulations = gen3Sims;
                    }
                    if (currentGeneration == CurrentGeneration.GEN4)
                    {
                        simulations = gen4Sims;
                    }
                    int goalCount = 0;
                    foreach (var item in simulations[i].map)
                    {
                        foreach (var item2 in item)
                        {
                            if (item2 == 2)
                                goalCount++;
                        }

                    } 
                    if(simulations[i].teams[0].scores != null)
                    {
                        averageAttackerScore += simulations[i].teams[0].scores[simulations[i].teams[0].scores.Length - 1] / goalCount;
                        averageDefenderScore += 1 - simulations[i].teams[0].scores[simulations[i].teams[0].scores.Length - 1] / goalCount;
                    }
                    if(simulations[i].teams[0].goals != null)
                    {
                        averageAttackerScore += simulations[i].teams[0].goals[simulations[i].teams[0].goals.Length - 1] / goalCount;
                        averageDefenderScore += 1 - simulations[i].teams[0].goals[simulations[i].teams[0].goals.Length - 1] / goalCount ;
                    }
                    int highestTurns = 0;
                    foreach (var item in simulations[i].players)
                    {
                        if (item.coords.Length > highestTurns)
                            highestTurns = item.coords.Length;
                    }
                    Debug.Log("Calculation = " + ((simulations[i].map.Length + simulations[i].map[0].Length) * (goalCount + 1)) + " : Highest turns =" + highestTurns);
                    if(((simulations[i].map.Length + simulations[i].map[0].Length) * (goalCount + 1))+1 == highestTurns)
                    {
                        averageTurnTimer += 1;
                    }
                    break;
        
                case CurrentGeneration.NULL:
                    break;
                default:
                    break;
            }              
            
            if (j == intervals)
            {
                j = WorkOutAverage(intervals);
            }
            else
                j++;

        }
    }

    private int WorkOutAverage(int intervals)
    {
        int j;
        attackersResults.Add(Mathf.Clamp(averageAttackerScore / intervals * 100,0f,100f));
        defendersResults.Add(Mathf.Clamp(averageDefenderScore / intervals * 100, 0f, 100f));
        turnTimerResults.Add(Mathf.Clamp(averageTurnTimer / intervals * 100, 0f, 100f));

        if ((averageAttackerScore / intervals) * 100 > yMaximum)
            yMaximum = Mathf.RoundToInt(Mathf.Clamp((averageAttackerScore / intervals) *100,0f,100f));
        if ((averageDefenderScore / intervals) * 100 > yMaximum)
            yMaximum = Mathf.RoundToInt(Mathf.Clamp((averageDefenderScore / intervals) * 100,0f,100f));
        if ((averageAttackerScore / intervals) * 100 < yMinimum)
            yMinimum  = Mathf.RoundToInt(Mathf.Clamp((averageAttackerScore / intervals) * 100,0f,100f));        
        if ((averageDefenderScore / intervals) * 100 < yMinimum)
            yMinimum  = Mathf.RoundToInt(Mathf.Clamp((averageDefenderScore / intervals) * 100,0f,100f));
        j = 1;
        averageAttackerScore = 0;
        averageDefenderScore = 0;
        averageTurnTimer = 0;
        return j;
    }

    private GameObject CreateCircle(Vector2 anchoredPosition, Color color, int epochNumber)
    {
        GameObject gameObject = new GameObject("circle",typeof(CanvasRenderer), typeof(Image), typeof(Button));        

        gameObject.transform.SetParent(dotsHolder, false);
        Image image = gameObject.GetComponent<Image>();
        image.sprite = circleSprite;
        image.color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
 
        Button button = gameObject.GetComponent<Button>();        
        button.transition = Selectable.Transition.ColorTint;
        var colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.black;
        button.colors = colors;

        if(xIntervals == 1)
        {
    
            button.onClick.AddListener(() => simulationManager.changeSim(epochNumber.ToString()));
            button.onClick.AddListener(() => simInfo.GetComponent<SimInformation>().showSimInfo = true);
        }
        else
        {
            button.onClick.AddListener(() => startingEpoch = epochNumber);
            button.onClick.AddListener(() => endingEpoch = epochNumber + xIntervals);
            button.onClick.AddListener(() => StartCoroutine(LoadAndGenerateGraph(false)));
        }

        return gameObject;
    }


    private void ShowGraph(List<float> valueList, Color color)
    {
        float graphWidth = graphContainer.sizeDelta.x;

        float graphHeight = graphContainer.sizeDelta.y;
        float tenPercent = graphHeight / 10;

        xSize = graphWidth / attackersResults.Count;
        GameObject lastCircleCameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = ((valueList[i]/100) * (graphHeight - tenPercent * 2) + tenPercent);
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), color, ((i * xIntervals) + startingEpoch));
            if (lastCircleCameObject != null)
            {
                CreateDotConnection(lastCircleCameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, color);
            }
            lastCircleCameObject = circleGameObject;


        }
    }

    private void ShowGraphAxis(List<float> valueList)
    {

        float graphHeight = graphContainer.sizeDelta.y;
        float tenPercent = graphHeight / 10;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(labelsHolder);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -8.4f);
            labelX.GetComponent<TextMeshProUGUI>().text = ((i * xIntervals) + startingEpoch).ToString();
            labelX.localScale = new Vector3(1f, 1f, 1f);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(labelsHolder);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, 0f);
            dashX.localScale = new Vector3(1f, 1f, 1f);
        }
        int seperatorCount = 10;
        for (float i = 0; i <= 100; i += 10)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(labelsHolder);
            labelY.gameObject.SetActive(true);
            //float normalisedValue = (100/i);
            //Debug.LogError((i-yMinimum) + " " + (seperatorCount - yMinimum) + " " + normalisedValue);
            labelY.anchoredPosition = new Vector2(-9f, (i/100) * (graphHeight - tenPercent * 2) + tenPercent);
            labelY.GetComponent<TextMeshProUGUI>().text = ((i).ToString() + "%");

            labelY.localScale = new Vector3(1f, 1f, 1f);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(labelsHolder);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(0f, (i/100) * (graphHeight - tenPercent * 2) + tenPercent);
            dashY.localScale = new Vector3(1f, 1f, 1f);
        }

    }
    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(linesHolder, false);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        Image image = gameObject.GetComponent<Image>();
        image.color = new Color(color.r, color.g, color.b, color.a / 2);
        image.raycastTarget = false;
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0,(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));

    }

    public void QuitProgram() { Application.Quit(); }
}
