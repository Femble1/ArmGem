using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExpBar : MonoBehaviour
{
    public int exp_before;
    [SerializeField]
    int exp_after;
    float screen_proportion;
    [SerializeField]
    GameObject lvl_up_window;
    [SerializeField]
    GameObject exp_bar_parent;
    bool exp_lvl_up;
    bool main_exp;
    public CharacterSO stats_before;
    // Start is called before the first frame update
    void OnEnable()
    {
        
    }
    void Start()
    {
        screen_proportion = Screen.width/1920f;
        float inverse_proportion_position = (633f - (633f * (exp_before/100f))) * screen_proportion;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(inverse_proportion_position, GetComponent<RectTransform>().anchoredPosition.y);
        StartCoroutine(calculate_exp());
    }

    IEnumerator calculate_exp()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log(exp_after);
        if (exp_after < exp_before)
        {
            GetComponent<RectTransform>().DOAnchorPosX(0f, 1.5f, true).SetEase(Ease.OutQuint).OnComplete(() => exp_lvl_up = true);
        }
        else
        {
            GetComponent<RectTransform>().DOAnchorPosX((633f - (633f * (exp_after/100f))) * screen_proportion, 1.5f, true).SetEase(Ease.OutQuint);
        }
        yield return new WaitForSeconds(1.6f);
        if (exp_lvl_up == false) 
        {
            yield break;
        }
        deactivate_particles();
        exp_bar_parent.GetComponent<RectTransform>().DOAnchorPosX(-400 * screen_proportion, 0.15f, true).SetEase(Ease.OutQuint);
        exp_bar_parent.GetComponent<RectTransform>().DOScaleX(0, 0.15f).SetEase(Ease.OutQuint).OnComplete(() => create_lvl_up_window());
        GetComponent<RectTransform>().DOAnchorPosX(633f * screen_proportion, 0.02f, true).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(0.02f);
        GetComponent<RectTransform>().DOAnchorPosX((633f - (633f * (exp_after/100f))) * screen_proportion, 1.5f, true).SetEase(Ease.OutQuint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void add_listener(CharacterBehaviour character, string exp_type)
    {
        Debug.Log("Listener");
        switch(exp_type)
        {
            case "Normal": character.ExpChange.AddListener(exp_change); main_exp = true;  break;
            case "Class": character.ClassExpChange.AddListener(class_exp_change); main_exp = false; break;
        }
    }

    void exp_change(int exp)
    {
        exp_after = exp;
    }

    void class_exp_change(int class_exp)
    {
        exp_after = class_exp;
    }

    void create_lvl_up_window()
    {
        if (main_exp)
        {
            GameObject new_lvl_up_window = Instantiate(lvl_up_window) as GameObject;
            new_lvl_up_window.transform.SetParent(GetComponentInParent<UI>().transform, false);
            new_lvl_up_window.GetComponent<RectTransform>().localScale = new Vector2 (0, new_lvl_up_window.GetComponent<RectTransform>().localScale.y);
            new_lvl_up_window.GetComponent<RectTransform>().DOScaleX(1, 0.15f).SetEase(Ease.OutQuint);
            new_lvl_up_window.GetComponent<RectTransform>().DOAnchorPosX(0, 0.15f, true).SetEase(Ease.OutQuint);
        }
    }

    void deactivate_particles()
    {
        ParticleSystem[] particles = exp_bar_parent.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem particle in particles)
        {
            particle.Clear();
            particle.Stop();
        }
    }
}
