using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimInformation : MonoBehaviour
{
    public Animator animator;
    public bool showSimInfo;
    public string simNumber;
    public int attackerPoints;
    public int numberOfDefenders;
    public int numberOfAttackers; 
    public int turnsTaken;
    public int numberOfGoals;
    [SerializeField] RectTransform informationholder;

    private TextMeshProUGUI simNumberText;
    private TextMeshProUGUI attackerPointsText;
    private TextMeshProUGUI numberOfDefendersText;
    private TextMeshProUGUI numberOfAttackersText;
    private TextMeshProUGUI turnsTakenText;
    private TextMeshProUGUI numberOfGoalsText;    

    void Start()
    {
        animator = GetComponent<Animator>();
        informationholder = transform.Find("Information").GetComponent<RectTransform>();
        simNumberText = informationholder.Find("simNumberText").GetComponent<TextMeshProUGUI>();
        attackerPointsText = informationholder.Find("attackerPointsText").GetComponent<TextMeshProUGUI>();
        numberOfDefendersText = informationholder.Find("numberOfDefendersText").GetComponent<TextMeshProUGUI>();
        numberOfAttackersText = informationholder.Find("numberOfAttackersText").GetComponent<TextMeshProUGUI>();
        turnsTakenText = informationholder.Find("turnsTakenText").GetComponent<TextMeshProUGUI>();
        numberOfGoalsText = informationholder.Find("numberOfGoalsText").GetComponent<TextMeshProUGUI>();

    }
    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Show", showSimInfo);
        simNumberText.text = simNumber;
        attackerPointsText.text = attackerPoints.ToString();
        numberOfDefendersText.text = numberOfDefenders.ToString();
        numberOfAttackersText.text = numberOfAttackers.ToString();
        turnsTakenText.text = turnsTaken.ToString();
        numberOfGoalsText.text = numberOfGoals.ToString();
    }

    public void CloseInfo()
    {
        showSimInfo = false;
    }
}
