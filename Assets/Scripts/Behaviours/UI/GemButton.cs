using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GemButton : MonoBehaviour
{
    public int button_number;
    public int skill_number;
    public Text text;
    public Skill.skill_gem gem_type;
    GameObject cursor;
    RectTransform screen_position;
    // Start is called before the first frame update
    void Start()
    {  
        screen_position = GetComponent<RectTransform>();
        cursor = GameObject.Find("Cursor");
        gem_button();
    }

    // Update is called once per frame
    void Update()
    {
        adjust();
        gem_button();
        if (Input.GetKeyUp(KeyCode.D))
        {
            go_back();
        }
    }

    void gem_button()
    {
        if (button_number == 1)
        {  
            if (gem_type == Skill.skill_gem.WeaponGem)
            {
                text.text = "Attack";
                if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().weapon_gem_evoking == true)
                {
                    GetComponent<Button>().interactable = true;
                    text.color = new Color(1, 1, 1);
                }
                else 
                {
                    GetComponent<Button>().interactable = false;
                    text.color = new Color(0.5f, 0.5f, 0.5f);
                }
            }
            if (gem_type == Skill.skill_gem.ArmorGem)
            {
                text.text = "Evoke";
                if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().armor_gem_evoking == false)
                {
                    GetComponent<Button>().interactable = true;
                    text.color = new Color(1, 1, 1);
                    //Set transformation animation ** Create if gem last turn
                }
                else
                {
                    //Desset transformation animation
                    GetComponent<Button>().interactable = false;
                    text.color = new Color(0.5f, 0.5f, 0.5f);
                }
            }
        }
        else if (button_number == 2)
        {
            if (gem_type == Skill.skill_gem.WeaponGem)
            {
                text.text = "Evoke";
                if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().weapon_gem_evoking == false)
                {
                    GetComponent<Button>().interactable = true;
                    text.color = new Color(1, 1, 1);
                    var eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(gameObject, new BaseEventData(eventSystem));
                }
                else 
                {
                    GetComponent<Button>().interactable = false;
                    text.color = new Color(0.5f, 0.5f, 0.5f);
                }
            }
            if (gem_type == Skill.skill_gem.ArmorGem)
            {
                text.text = "Restore";
                if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().armor_gem_evoking == true)
                {
                    GetComponent<Button>().interactable = true;
                    text.color = new Color(1, 1, 1);
                    var eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(gameObject, new BaseEventData(eventSystem));
                }
                else
                {
                    GetComponent<Button>().interactable = false;
                    text.color = new Color(0.5f, 0.5f, 0.5f);
                }
            }
        }
        else 
        {
            text.text = "Restore";
            if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().weapon_gem_evoking == true)
            {
                GetComponent<Button>().interactable = true;
                text.color = new Color(1, 1, 1);
            }
            else
            {
                GetComponent<Button>().interactable = false;
                text.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    void adjust()
    {
        float scale_proportion_x = Screen.width/1331f;

        screen_position.localScale = new Vector3(scale_proportion_x, scale_proportion_x, scale_proportion_x);
    }

    void go_back()
    {
        if (button_number == 1)
        {
            GetComponentInParent<UI>().last_button_clicked.GetComponent<SkillGemButton>().select = true;
        }
        Destroy(gameObject);
    }

    public void gem_button_click()
    {
        GameObject cursor = GameObject.Find("Cursor");
        
        if (gem_type == Skill.skill_gem.WeaponGem)
        {
            switch(button_number)
            {
                case 1:
                cursor.GetComponent<CursorBehaviour>().on = true;
                cursor.GetComponent<CursorBehaviour>().create_skill_box(GetComponentInParent<UI>().last_button_clicked.GetComponent<SkillGemButton>().skill_number);
                break;
                case 2:
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().weapon_gem_evoking = true;
                if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().weapon_gem_evoked == false)
                {
                    cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().ep ++;
                }
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().gem_evoke_restore(GetComponentInParent<UI>().last_button_clicked.GetComponent<SkillGemButton>().skill_number, true);
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().EpChange.Invoke(cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().ep);
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().GemChange.Invoke("WeaponEvoke");
                break;
                case 3:
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().weapon_gem_evoking = false;
                if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().weapon_gem_evoked == false)
                {
                    cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().ep --;
                }
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().gem_evoke_restore(GetComponentInParent<UI>().last_button_clicked.GetComponent<SkillGemButton>().skill_number, false);
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().EpChange.Invoke(cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().ep);
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().GemChange.Invoke("WeaponRestore");
                break;
            }
        }
        else if (gem_type == Skill.skill_gem.ArmorGem)
        {
            switch(button_number)
            {
                case 1:
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().armor_gem_evoking = true;
                if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().armor_gem_evoked == false)
                {
                    cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().ep ++;
                }
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().gem_evoke_restore(GetComponentInParent<UI>().last_button_clicked.GetComponent<SkillGemButton>().skill_number, true);
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().EpChange.Invoke(cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().ep);
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().GemChange.Invoke("ArmorEvoke");
                break;
                case 2:
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().armor_gem_evoking = false;
                if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().armor_gem_evoked == false)
                {
                    cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().ep --;
                }
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().gem_evoke_restore(GetComponentInParent<UI>().last_button_clicked.GetComponent<SkillGemButton>().skill_number, false);
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().EpChange.Invoke(cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().ep);
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().GemChange.Invoke("ArmorRestore");
                break;
            }
        }
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("SecondLayerButton");
        foreach(GameObject GemButton in buttons)
        {
            GemButton.GetComponent<GemButton>().go_back();
        }
        go_back();
    }
}
