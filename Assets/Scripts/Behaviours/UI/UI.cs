using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class UI : MonoBehaviour
{
    public GameObject skill_gem_button;
    public GameObject gem_button;
    public GameObject portrait;
    public GameObject portrait_scroll;
    public bool create_skill_buttons;
    public GameObject character_highlighted;
    public GameObject last_button_clicked;
    public List<GameObject> portraits;
    bool portraits_created;
    // Start is called before the first frame update
    void Start()
    {
        portraits = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        create_skill_options();
    }

    public void create_gem_options(Skill.skill_gem gem_type, int skill_number, GameObject button_clicked)
    {
        last_button_clicked = button_clicked;
        float button_distance = 0;
        int button_number = 1;
        float scale_proportion_x = Screen.width/1331f;
        float scale_proportion_y = Screen.height/717f;

        if (gem_type == Skill.skill_gem.ArmorGem)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject new_button = Instantiate(gem_button) as  GameObject;
                new_button.transform.SetParent(transform, false);
                new_button.GetComponent<RectTransform>().anchoredPosition = new Vector2(440 * scale_proportion_x, (-125 * scale_proportion_y) - ((30.01f * scale_proportion_x) * skill_number) - (button_distance * scale_proportion_x) - 10f);
                new_button.GetComponent<GemButton>().button_number = button_number;
                new_button.GetComponent<GemButton>().gem_type = gem_type;
                new_button.GetComponent<GemButton>().skill_number = skill_number;
                if (i == 0)
                {
                    var eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(new_button, new BaseEventData(eventSystem));
                }
                button_number ++;
                button_distance += 17.01f;
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject new_button = Instantiate(gem_button) as  GameObject;
                new_button.transform.SetParent(transform, false);
                new_button.GetComponent<RectTransform>().anchoredPosition3D = new Vector2(440 * scale_proportion_x, (-125 * scale_proportion_y) - ((30.01f * scale_proportion_x) * skill_number) - (button_distance * scale_proportion_x));
                new_button.GetComponent<GemButton>().button_number = button_number;
                new_button.GetComponent<GemButton>().gem_type = gem_type;
                new_button.GetComponent<GemButton>().skill_number = skill_number;
                if (i == 0)
                {
                    var eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(new_button, new BaseEventData(eventSystem));
                }
                button_number ++;
                button_distance += 17.01f;
            }
        }
    }

    public void create_skill_options()
    {
        if (create_skill_buttons == true)
        {
            float button_distance = 0;
            float scale_proportion_x = Screen.width/1331f;
            float scale_proportion_y = Screen.height/717f;

            for (int i = 0; i < 8; i++)
            {
                GameObject new_button = Instantiate(skill_gem_button) as GameObject;
                new_button.transform.SetParent(transform, false);
                new_button.GetComponent<RectTransform>().anchoredPosition = new Vector2(206 * scale_proportion_x, (-143 * scale_proportion_y) - (button_distance * scale_proportion_x));
                new_button.GetComponent<SkillGemButton>().skill_number = i;
                new_button.GetComponent<SkillGemButton>().character_highlighted = character_highlighted;
                if (character_highlighted.GetComponent<CharacterBehaviour>().Skills[i] != null)
                {
                    new_button.GetComponent<SkillGemButton>().gem_type = character_highlighted.GetComponent<CharacterBehaviour>().Skills[i].SkillOrGem;
                }
                if (i == 0)
                {
                    last_button_clicked = new_button;
                    var eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(new_button, new BaseEventData(eventSystem));
                }
                button_distance += 30.01f;
            }
            create_skill_buttons = false;
        }
    }

    public void create_portrait_turn(List<GameObject> characters)
    {
        float scale_proportion_x = Screen.width/1331f;
        float scale_proportion_y = Screen.height/717f;
        if (!portraits_created)
        {
            float horizontal_distance = 150;
            
            GameObject new_scroll = Instantiate(portrait_scroll) as GameObject;
            new_scroll.transform.SetParent(transform, false);
            new_scroll.GetComponent<RectTransform>().anchoredPosition = new Vector2((600 * scale_proportion_x), (75 * scale_proportion_y));
            new_scroll.GetComponent<RectTransform>().sizeDelta = new Vector2((500 * scale_proportion_x), (100 * scale_proportion_y));
            

            for (int i = 0; i < characters.Count; i++)
            {
                GameObject new_portrait = Instantiate(portrait) as GameObject;
                new_portrait.transform.SetParent(transform, false);
                new_portrait.GetComponent<RectTransform>().anchoredPosition = new Vector2((150 * scale_proportion_x), (100 * scale_proportion_y));
                new_portrait.GetComponent<RectTransform>().localScale = new Vector3(scale_proportion_x, scale_proportion_x, scale_proportion_x);
                new_portrait.GetComponent<ScrollPortrait>().portrait_number = i;
                new_portrait.GetComponent<ScrollPortrait>().portrait_stats = characters[i].GetComponent<CharacterBehaviour>().stats;
                new_portrait.GetComponent<ScrollPortrait>().add_listener("health", characters[i].GetComponent<CharacterBehaviour>());
                new_portrait.GetComponent<ScrollPortrait>().add_listener("ep", characters[i].GetComponent<CharacterBehaviour>());
                new_portrait.GetComponent<ScrollPortrait>().add_listener("gems", characters[i].GetComponent<CharacterBehaviour>());
                portraits.Add(new_portrait);

                if (i > 0)
                {
                    new_portrait.transform.SetParent(new_scroll.transform, false);
                    new_portrait.GetComponent<RectTransform>().anchoredPosition = new Vector2((150 * scale_proportion_x) + (horizontal_distance * scale_proportion_x) - (350 * scale_proportion_x), (75 * scale_proportion_y) - (25 * scale_proportion_y));
                }

                portraits_created = true;
                
                horizontal_distance += 100;
            } 
        }
        else
        {
            Vector2 last = new Vector2(0,0);
            Vector2 first = new Vector2(0,0);
            Vector3 last_scale = new Vector3(0,0);
            Vector3 first_scale = new Vector3(0,0);

            foreach(GameObject portrait in portraits)
            {
                if (portrait.GetComponent<ScrollPortrait>().portrait_number == 0)
                {
                    first = portrait.GetComponent<RectTransform>().anchoredPosition;
                    first_scale = portrait.GetComponent<RectTransform>().localScale;
                }
                else if (portrait.GetComponent<ScrollPortrait>().portrait_number == portraits.Count - 1)
                {
                    last = portrait.GetComponent<RectTransform>().anchoredPosition;
                    last_scale = portrait.GetComponent<RectTransform>().localScale;
                }
            }
            

            foreach(GameObject portrait in portraits)
            {
                if (portrait.GetComponent<ScrollPortrait>().portrait_number == 0)
                {
                    portrait.transform.SetParent(GameObject.FindGameObjectWithTag("PortraitScroll").transform);
                    portrait.GetComponent<RectTransform>().anchoredPosition = last;
                    portrait.GetComponent<RectTransform>().localScale = last_scale;
                    portrait.GetComponent<ScrollPortrait>().portrait_number = portraits.Count - 1;
                }
                else if (portrait.GetComponent<ScrollPortrait>().portrait_number == 1)
                {
                    portrait.transform.SetParent(transform);
                    portrait.GetComponent<RectTransform>().anchoredPosition = first;
                    portrait.GetComponent<RectTransform>().localScale = first_scale;
                    portrait.GetComponent<ScrollPortrait>().portrait_number -= 1;
                }
                else
                {
                    portrait.GetComponent<RectTransform>().anchoredPosition = new Vector2(portrait.GetComponent<RectTransform>().anchoredPosition.x - (100 * scale_proportion_x), portrait.GetComponent<RectTransform>().anchoredPosition.y);
                    portrait.GetComponent<ScrollPortrait>().portrait_number -= 1;
                }
            }
        }
    }
}