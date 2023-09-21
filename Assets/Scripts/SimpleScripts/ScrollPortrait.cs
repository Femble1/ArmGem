using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollPortrait : MonoBehaviour
{
    public int portrait_number;
    public CharacterSO portrait_stats;
    int position = 0;
    bool right;
    public float motion_duration = 1f;
    public Ease motion_type = Ease.OutCubic;
    public GameObject health_bar;
    public Text health_text;
    Image[] healthbar_components;
    public bool destroy;
    public SpriteRenderer[] portrait_images;
    RectTransform screen_position;
    Vector2 target_position;
    bool command;
    // Start is called before the first frame update
    void Start()
    {
        screen_position = GetComponent<RectTransform>();
        target_position = screen_position.anchoredPosition;
        portrait_images = GetComponentsInChildren<SpriteRenderer>();
        healthbar_components = health_bar.GetComponentsInChildren<Image>();
        GetComponentInChildren<Text>().text = portrait_stats.max_hp.ToString() + "/" + portrait_stats.max_hp.ToString();

        if (portrait_number == 0)
        {
            screen_position.localScale = new Vector3(screen_position.localScale.x * 1.5f, screen_position.localScale.y * 1.5f, screen_position.localScale.z * 1.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (position <= 0) position = 0;

        show_stats();

        if (move())
        {
            movement();
        }
    }

    void movement()
    {
        if (portrait_number != 0 && position > 0 && !right)
        {
            float scale_proportion_x = Screen.width/1331f;
            float scale_proportion_y = Screen.height/717f;

            target_position += new Vector2((100 * scale_proportion_x), 0);

            screen_position.DOAnchorPos(target_position, motion_duration, true).SetEase(motion_type);

            position --;
        }

        GameObject[] portraits = GameObject.FindGameObjectsWithTag("Portrait");
        int i = 0;
        foreach(GameObject portrait in portraits)
        {
            if (portrait.GetComponent<ScrollPortrait>().portrait_number > i)
            {
                i = portrait.GetComponent<ScrollPortrait>().portrait_number;
            }
        }

        if (portrait_number != 0 && i > 6 && right)
        {
            if (position < i - 5)
            {
                float scale_proportion_x = Screen.width/1331f;
                float scale_proportion_y = Screen.height/717f;

                target_position -= new Vector2((100 * scale_proportion_x), 0);

                screen_position.DOAnchorPos(target_position, motion_duration, true).SetEase(motion_type);

                position ++;
            }
        }
    }

    bool move()
    {
        if(Input.GetKeyUp(KeyCode.E))
        {
            right = true;
            return true;
        }

        if(Input.GetKeyUp(KeyCode.Q))
        {
            right = false;
            return true;
        }
        
        return false;
    }

    void show_stats()
    {
        if (portrait_number == 0)
        {
            health_bar.GetComponent<Image>().enabled = true;
            health_text.enabled = true;
            foreach(Image component in healthbar_components)
            {
                component.enabled = true;
            }
        }
        else 
        {
            health_bar.GetComponent<Image>().enabled = false;
            health_text.enabled = false;
            foreach(Image component in healthbar_components)
            {
                component.enabled = false;
            }
        }

        foreach(SpriteRenderer portrait_image in portrait_images)
        {
            if (portrait_number == 0)
            {
                portrait_image.enabled = true;
            }
            else
            {
                portrait_image.enabled = false;
            }
        }
    }

    public void add_listener(string type, CharacterBehaviour character)
    {
        switch(type)
        {
            case "health": character.HealthChange.AddListener(portrait_hp_change); break;
            case "ep": character.EpChange.AddListener(portrait_ep_change); break;
            case "gems": character.GemChange.AddListener(portrait_gem_change); break;
        }
    }

    void portrait_hp_change(int hp)
    {
        float max_healthpoints = portrait_stats.max_hp;
        float healthpoints = hp;
        GetComponentInChildren<PortraitHP>().update_health_bar(healthpoints/max_healthpoints);
        GetComponentInChildren<Text>().text = healthpoints.ToString() + "/" + max_healthpoints.ToString();
        if (healthpoints/max_healthpoints <= 0) destroy = true;
    }

    void portrait_ep_change(int ep)
    {
        EpPortrait[] ep_portraits = GetComponentsInChildren<EpPortrait>();
        foreach(EpPortrait ep_portrait in ep_portraits)
        {
            if (ep_portrait.ep_number <= ep)
            {
                ep_portrait.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            }
            else
            {
                ep_portrait.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f);
            }
        }
    }

    void portrait_gem_change(string gem)
    {
        GemPortrait[] gem_portraits = GetComponentsInChildren<GemPortrait>();
        foreach(GemPortrait gem_portrait in gem_portraits)
        {
            switch(gem)
            {
                case "WeaponEvoke":
                switch(gem_portrait.gem_type)
                {
                    case "Weapon": gem_portrait.GetComponent<SpriteRenderer>().color = new Color(0.52f, 0.73f, 1f);
                    break;
                    case "Armor": 
                    break;
                }
                break;
                case "WeaponRestore":
                switch(gem_portrait.gem_type)
                {
                    case "Weapon": gem_portrait.GetComponent<SpriteRenderer>().color = new Color(0.15f, 0.15f, 0.15f);
                    break;
                    case "Armor":
                    break;
                }
                break;
                case "ArmorEvoke":
                switch(gem_portrait.gem_type)
                {
                    case "Weapon": 
                    break;
                    case "Armor": gem_portrait.GetComponent<SpriteRenderer>().color = new Color(0.52f, 0.73f, 1f);
                    break;
                }
                break;
                case "ArmorRestore":
                switch(gem_portrait.gem_type)
                {
                    case "Weapon":
                    break;
                    case "Armor": gem_portrait.GetComponent<SpriteRenderer>().color = new Color(0.15f, 0.15f, 0.15f);
                    break;
                }
                break;
            }
        }
    }
}
