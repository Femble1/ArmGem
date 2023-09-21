using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class LvlUpWindow : MonoBehaviour
{
    CharacterSO stats;
    [SerializeField]
    List<Text> drew_stats;
    List<Text> undesirable_texts;

    // Start is called before the first frame update
    void Start()
    {
        drew_stats = new List<Text>();
        undesirable_texts = new List<Text>();
        undesirable_texts.AddRange(GetComponentsInChildren<Text>());
        foreach(Text undesirable_text in undesirable_texts)
        {
            if (undesirable_text.text == "0")
            {
                drew_stats.Add(undesirable_text);
            }
        }
        undesirable_texts.Clear();
        Debug.Log(drew_stats);
        //correct_stats(drew_stats);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void correct_stats(List<Text> stats_text)
    {
        for (int i = 0; i < 8; i++)
        {
            switch(i)
            {
                case 0: stats_text[i].text = stats.lvl.ToString(); break;
                case 1: stats_text[i].text = stats.max_hp.ToString(); break;
                case 2: stats_text[i].text = stats.force.ToString(); break;
                case 3: stats_text[i].text = stats.spirit.ToString(); break;
                case 4: stats_text[i].text = stats.defense.ToString(); break;
                case 5: stats_text[i].text = stats.resistance.ToString(); break;
                case 6: stats_text[i].text = stats.mastership.ToString(); break;
                case 7: stats_text[i].text = stats.agility.ToString(); break;
            }
        }
    }
}
