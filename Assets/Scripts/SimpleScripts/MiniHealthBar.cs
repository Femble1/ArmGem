using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniHealthBar : MonoBehaviour
{
    CharacterBehaviour character;
    Slider healthbar;
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponentInParent<CharacterBehaviour>();
        healthbar = GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        float character_hp = character.hp;
        float character_max_hp = character.max_hp;
        healthbar.value = character_hp/character_max_hp;
    }
}
