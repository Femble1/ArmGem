using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class FEAnimator : MonoBehaviour
{
    public int total_hits = 0;
    public int hits_dealed = 0;
    public bool appear;
    Color tmp;
    Material material;
    AnimatorOverrideController animator_override_controller;
    CharacterBehaviour origin_body;
    BattleSceneBehaviour battle_scene_manager;
    // Start is called before the first frame update
    void Start()
    {
        origin_body = GetComponentInParent<CharacterBehaviour>();
        battle_scene_manager = GameObject.FindGameObjectWithTag("BattleSceneManager").GetComponent<BattleSceneBehaviour>();
        Animator animator = GetComponent<Animator>();
        material = GetComponent<SpriteRenderer>().material;
        animator_override_controller = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animator_override_controller;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            animator_override_controller["attack"] = Resources.Load<AnimationClip>("Sprites/Uma Animations/idle");
        }
        alpha_control();
    }

    public void update_animation_order()
    {
        Animator animator = GetComponent<Animator>();
        if (origin_body.gem_user)
        {
            if (origin_body.weapon_gem_evoking == true && origin_body.weapon_gem_evoked == false && origin_body.armor_gem_evoking == false && origin_body.armor_gem_evoked == false)
            {
                animator.SetBool("no_armor_1_gem", true);
                animator.SetBool("no_armor_2_gem", false);
                animator.SetBool("armor_1_gem", false);
                animator.SetBool("armor_2_gem", false);
                animator.SetBool("just_attack", false);
            }
            if (origin_body.weapon_gem_evoking == true && origin_body.weapon_gem_evoked == false && origin_body.armor_gem_evoking == true && origin_body.armor_gem_evoked == false)
            {
                animator.SetBool("no_armor_1_gem", false);
                animator.SetBool("no_armor_2_gem", true);
                animator.SetBool("armor_1_gem", false);
                animator.SetBool("armor_2_gem", false);
                animator.SetBool("just_attack", false);
            }
            if (origin_body.weapon_gem_evoking == true && origin_body.weapon_gem_evoked == false && origin_body.armor_gem_evoking == true && origin_body.armor_gem_evoked == true)
            {
                animator.SetBool("no_armor_1_gem", false);
                animator.SetBool("no_armor_2_gem", false);
                animator.SetBool("armor_1_gem", true);
                animator.SetBool("armor_2_gem", false);
                animator.SetBool("just_attack", false);
            }
            if (origin_body.weapon_gem_evoking == true && origin_body.weapon_gem_evoked == false && origin_body.armor_gem_evoking == false && origin_body.armor_gem_evoked == true)
            {
                animator.SetBool("no_armor_1_gem", false);
                animator.SetBool("no_armor_2_gem", false);
                animator.SetBool("armor_1_gem", false);
                animator.SetBool("armor_2_gem", true);
                animator.SetBool("just_attack", false);
            }
            if (origin_body.weapon_gem_evoking == true && origin_body.weapon_gem_evoked == true)
            {
                animator.SetBool("no_armor_1_gem", false);
                animator.SetBool("no_armor_2_gem", false);
                animator.SetBool("armor_1_gem", false);
                animator.SetBool("armor_2_gem", false);
                animator.SetBool("just_attack", true);
            }
        }
        else
        {
            animator.SetBool("attacking", true);
        }
    }
    void alpha_control()
    {
        if (tmp.a < 1)
        {
            material = Resources.Load<Material>("Materials/SpriteTransparent");
        }
        else
        {
            material = Resources.Load<Material>("Materials/SpriteOpaque");
        }

        if (appear)
        {
            tmp = GetComponent<SpriteRenderer>().color;
            tmp.a += 0.02f;
            if (tmp.a >= 1) tmp.a = 1;
            GetComponent<SpriteRenderer>().color = tmp;
        }
        else
        {
            tmp = GetComponent<SpriteRenderer>().color;
            tmp.a -= 0.05f;
            if (tmp.a <= 0) tmp.a = 0;
            GetComponent<SpriteRenderer>().color = tmp;
        }

        if (tmp.a < 0.04)
        {
            GetComponent<Animator>().enabled = false;
        }
        else GetComponent<Animator>().enabled = true;
    }
    public void reset_animation_order()
    {
        Animator animator = GetComponent<Animator>();
        if (origin_body.gem_user)
        {
            animator.SetBool("no_armor_1_gem", false);
            animator.SetBool("no_armor_2_gem", false);
            animator.SetBool("armor_1_gem", false);
            animator.SetBool("armor_2_gem", false);
            animator.SetBool("just_attack", false);
        }
        else
        {
            animator.SetBool("attacking", false);
        }
        battle_scene_manager.battle_ended = true;
    }
    public void add_hit()
    {
        total_hits ++;
    }

    public void damage_frame()
    {
        battle_scene_manager.deal_damage(total_hits);
        hits_dealed ++;
        if (hits_dealed >= total_hits)
        {
            total_hits = 0;
            hits_dealed = 0;
        }
    }
}
