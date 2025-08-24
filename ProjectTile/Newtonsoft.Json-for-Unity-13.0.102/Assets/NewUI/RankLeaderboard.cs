using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using YG.Utils.LB;

public class RankLeaderboard : MonoBehaviour
{
    public GameObject rank1;
    public GameObject rank2;
    public GameObject rank3;
    public GameObject test;

    public LBPlayerDataYG leader;
    private string rank;

    public TextMeshProUGUI textDuplicateRank;
    public TextMeshProUGUI textDuplicateName;
    public TextMeshProUGUI textDuplicateScore;
    public Text textRank;
    public Text textName;
    public Text textScore;

    private void Update()
    {
        textDuplicateName.text = textName.text;
        textDuplicateScore.text = textScore.text;
        textDuplicateRank.text = textRank.text;
        textRank.gameObject.SetActive(false);
        textName.gameObject.SetActive(false);
        textScore.gameObject.SetActive(false);  

        rank = leader.data.rank;
        test.GetComponent<Image>().enabled = true;

        if(rank == "1")
        {
            textDuplicateRank.gameObject.SetActive(false);
            rank2.SetActive(false);
            rank3.SetActive(false);
            rank1.SetActive(true);
        }
        if (rank == "2")
        {
            textDuplicateRank.gameObject.SetActive(false);
            rank1.SetActive(false);
            rank3.SetActive(false);
            rank2.SetActive(true);
        }
        if (rank == "3")
        {
            textDuplicateRank.gameObject.SetActive(false);
            rank2.SetActive(false);
            rank1.SetActive(false);
            rank3.SetActive(true);
        }
    }
}
