using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;

public class TweenText : MonoBehaviour
{
    int text_max_length;
    int text_actual_length;
    float cooldown;
    string actual_sentence;
    public string wanted_text;
    char[] displayed_sentence;
    // Start is called before the first frame update
    void Start()
    {
        text_max_length = wanted_text.Length;
        text_actual_length = text_max_length - 1;
        actual_sentence = wanted_text;
    }

    // Update is called once per frame
    void Update()
    {
        write();
    }
    void write()
    {
        if (cooldown == 0)
        {
            displayed_sentence = actual_sentence.ToCharArray(0, text_max_length - text_actual_length);
            string text = new string(displayed_sentence);

            GetComponent<Text>().text = text;
            text_actual_length -= 1;
            if (text_actual_length <= 0) text_actual_length = 0;
            cooldown = 1;
        }
        cooldown -= 0.2f;
        if (cooldown <= 0) cooldown = 0;
    }
}
