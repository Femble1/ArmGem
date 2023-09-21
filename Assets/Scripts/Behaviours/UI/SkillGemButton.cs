using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class SkillGemButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public int skill_number = 0;
    public Text text;
    public Image shader;
    public Image icon;
    public Material ui_hdr;
    public GameObject character_highlighted;
    Button button;
    Skill skill;
    RectTransform screen_position;
    public Skill.skill_gem gem_type;
    public bool select = false;
    void Awake()
    {
        screen_position = GetComponent<RectTransform>();
        shader.enabled = false;
        icon.enabled = false;
        icon.material = null;
    }
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        skill = character_highlighted.GetComponent<CharacterBehaviour>().Skills[skill_number];
        if (skill == null)
        {
            text.text = "";
            GetComponent<Image>().material = null;
            return;
        }
        deactivate_button();
        icon.enabled = true;
        if (gem_type == Skill.skill_gem.Skill)
        {
            icon.sprite = Resources.Load<Sprite>("Sprites/UI/skill_icon");
            icon.color = new Color(1, 0.3f, 0.29f);
        }
        text.text = skill.SkillName;
        button.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        deactivate_button();
        select_button();
        adjust();
    }

    public void use_skill()
    {
        GameObject go_cursor = GameObject.Find("Cursor");
        CursorBehaviour cursor = go_cursor.GetComponent<CursorBehaviour>();
        CharacterBehaviour skill_user = character_highlighted.GetComponent<CharacterBehaviour>();
        if (skill_user.Skills[skill_number].SkillOrGem == Skill.skill_gem.Skill)
        {
            cursor.on = true;
            cursor.create_skill_box(skill_number);
        }
        else
        {
            GetComponentInParent<UI>().create_gem_options(gem_type, skill_number, gameObject);
        }
    }

    public void deactivate_button()
    {
        if (skill != null)
        {
            if (GameObject.FindGameObjectWithTag("SecondLayerButton") || GameObject.Find("Cursor").GetComponent<CursorBehaviour>().attack_directing == true || skill.ep_cost > character_highlighted.GetComponent<CharacterBehaviour>().ep)
            {
                button.interactable = false;
                text.color = new Color(0.5f, 0.5f, 0.5f);
            }
            else if (skill.SkillOrGem == Skill.skill_gem.Skill && skill.Power > 0 && !character_highlighted.GetComponent<CharacterBehaviour>().weapon_gem_evoking)
            {
                button.interactable = false;
                text.color = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                button.interactable = true;
                text.color = new Color(1, 1, 1);
            }
            
        }
    }

    public void select_button()
    {
        if (select == true && button.interactable == true)
        {
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(gameObject, new BaseEventData(eventSystem));
            select = false;
        }
    }

    void adjust()
    {
        float scale_proportion_x = Screen.width/1331f;

        screen_position.localScale = new Vector3(scale_proportion_x, scale_proportion_x, scale_proportion_x);
    }

    public void OnSelect(BaseEventData eventData)
    {
        screen_position.DOAnchorPos3D(new Vector3(186 * (Screen.width/1331f), screen_position.anchoredPosition3D.y, screen_position.anchoredPosition3D.z), 0.2f, true).SetEase(Ease.OutQuint);
        shader.enabled = true;
        icon.material = ui_hdr;
        switch(gem_type)
        {
            case Skill.skill_gem.Skill: shader.material.SetColor("_Color", new Color (1 * 7, 0.1f * 7, 0.1f * 7)); break;
            case Skill.skill_gem.ArmorGem: shader.material.SetColor("_Color", new Color(0.17f * 8, 0.55f * 8, 1 * 8)); break;
            case Skill.skill_gem.WeaponGem: shader.material.SetColor("_Color", new Color(0.17f * 8, 0.55f * 8, 1 * 8)); break;
        }
        FindObjectOfType<SoundBehaviour>().play("skill_menu");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        shader.enabled = false;
        icon.material = null;
        screen_position.DOAnchorPos3D(new Vector3(206 * (Screen.width/1331f), screen_position.anchoredPosition3D.y, screen_position.anchoredPosition3D.z), 0.2f, true).SetEase(Ease.OutQuint);
    }
}
