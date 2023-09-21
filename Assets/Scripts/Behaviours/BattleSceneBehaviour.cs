using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;
using DG.Tweening;

public class BattleSceneBehaviour : MonoBehaviour
{
    public GameObject main_cam;
    public GameObject cursor;
    public GameObject ui;
    public Vector3 attacker_offset;
    Vector3 attacker_original_position;
    Vector3 attacker_original_rotation;
    public Vector3 tile_offset;
    public Vector3 target_offset;
    public List<Vector3> target_original_position;
    List<Vector3> target_original_rotation;
    List<Vector3> tile_original_position;
    public Vector3 target_side_offset;
    public Vector3 target_back_offset;
    Vector3 desired_attacker_position;
    Vector3 desired_target_position;
    public List<GameObject> targets;
    public List<GameObject> killed;
    public List<EnabledTile> tiles;
    public List<int> damage;
    public float motion_duration = 1f;
    public Ease motion_type = Ease.OutCubic;
    public bool control_scene_player;
    public bool control_scene_enemy;
    public bool ending_scene;
    public bool battle_ended;
    bool end_scene;
    bool tween;
    public GameObject exp_bar;

    //Battle scene orientations
    
    Vector3 original_target_tile_position = new Vector3(0, 0, 0);
    void Start()
    {
        DOTween.SetTweensCapacity(10000, 100);
        targets = new List<GameObject>();
        killed = new List<GameObject>();
        tiles = new List<EnabledTile>();
        damage = new List<int>();
        target_original_position = new List<Vector3>();
        target_original_rotation = new List<Vector3>();
        tile_original_position = new List<Vector3>();
        
        exp_bar = Resources.Load<GameObject>("Prefabs/CoreGame/UI/ExpBar");
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            add_target(cursor.GetComponent<CursorBehaviour>().Characters[1]);
            invoke_battle(true);
        }
        if (control_scene_player == true)
        {
            desired_attacker_position = main_cam.transform.position + attacker_offset;
            desired_target_position = main_cam.transform.position + target_offset;
            battle_scene_control(targets);
        }
        if (control_scene_enemy == true)
        {

        }
    }
    public void add_target(GameObject target)
    {
        if (!targets.Contains(target))
        {
            targets.Add(target);
        }
    }
    public void invoke_battle(bool player)
    {
        if (player)
        {
            //Desactivating cursor
            cursor.GetComponent<CursorBehaviour>().on = false;
            //Setting player scene control true
            control_scene_player = true;
            //Saving player original position
            attacker_original_position = cursor.GetComponent<CursorBehaviour>().Characters[0].transform.position;
            attacker_original_rotation = cursor.GetComponent<CursorBehaviour>().Characters[0].transform.rotation.eulerAngles;
            //Saving targets original postions and tile original positions
            foreach(GameObject target in targets)
            {
                target_original_position.Add(target.transform.position);
                target_original_rotation.Add(target.transform.rotation.eulerAngles);
                tiles.Add(target.GetComponent<CharacterBehaviour>().current_tile);
                tile_original_position.Add(target.GetComponent<CharacterBehaviour>().current_tile.transform.position);
            }
            //Ending attacker skill selection, directioning and transitioning FE and FFT animators
            cursor.GetComponent<CursorBehaviour>().attack_directing = false;
            cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().performing_attack = true;
            cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponentInChildren<FEAnimator>().appear = true;
            cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponentInChildren<FFTAnimator>().appear = false;
            foreach(GameObject target in targets)
            {
                target.GetComponentInChildren<FEAnimator>().appear = true;
                Vector3 target_scale = target.GetComponentInChildren<FEAnimator>().transform.localScale;
                target.GetComponentInChildren<FEAnimator>().transform.localScale = new Vector3(target_scale.x * -1, target_scale.y, target_scale.z);
                target.GetComponentInChildren<FFTAnimator>().appear = false;
            }
            //Destroying buttons and skill boxes
            GameObject[] buttons = GameObject.FindGameObjectsWithTag("FirstLayerButton");
            foreach(GameObject SkillGemButton in buttons)
            {
                Destroy(SkillGemButton);
            }
            GameObject[] boxes = GameObject.FindGameObjectsWithTag("SkillBox");
            foreach(GameObject SkillBox in boxes)
            {
                Destroy(SkillBox);
            }
            tween = true;
        }
        else
        {
            //Setting enemy scene control true
            control_scene_enemy = true;
        }
        //Starting timer to end scene
        StartCoroutine(time_to_end_scene());
    }
    //Time before ending a scene
    IEnumerator time_to_end_scene()
    {
        yield return new WaitUntil(() => battle_ended == true);
        DOTween.KillAll(true);
        foreach (GameObject target in targets)
        {
            target.GetComponent<CharacterBehaviour>().last_damager = cursor.GetComponent<CursorBehaviour>().Characters[0];
        }
        //Spawn Exp Bar
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(ui.transform, false);
        panel.AddComponent<CanvasRenderer>();
        Image panel_image = panel.AddComponent<Image>();
        Color panel_color = panel_image.color;
        panel_color = new Color(0, 0, 0);
        panel_color.a = 0.80f;
        panel_image.color = panel_color;
        panel.GetComponent<RectTransform>().localScale = new Vector3(22, 11, 1);
        for (int i = 0; i < 2; i++)
        {
            GameObject new_exp_bar = Instantiate(exp_bar) as GameObject;
            new_exp_bar.transform.SetParent(ui.transform, false);
            new_exp_bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(new_exp_bar.GetComponent<RectTransform>().anchoredPosition.x, 170 - (200 * i));
            new_exp_bar.GetComponentInChildren<ExpBar>().exp_before = cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().exp;
            if (i == 0) 
            {
                new_exp_bar.GetComponentInChildren<ExpBar>().add_listener(cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>(), "Normal");
                new_exp_bar.GetComponentInChildren<ExpBar>().stats_before = cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().stats;
            }
            
            if (i == 1)
            {
                new_exp_bar.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
                new_exp_bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(133, new_exp_bar.GetComponent<RectTransform>().anchoredPosition.y);
                new_exp_bar.GetComponentInChildren<ExpBar>().exp_before = cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().class_exp;
                new_exp_bar.GetComponentInChildren<ExpBar>().add_listener(cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>(), "Class");
                RecolorUI[] recolor_objects = new_exp_bar.GetComponentsInChildren<RecolorUI>();
                foreach(RecolorUI recolor_object in recolor_objects)
                {
                    if (recolor_object == recolor_objects[0])
                    {
                        recolor_object.GetComponent<Image>().color = new Color(0.23f, 0.41f, 1f);
                    }
                    else recolor_object.GetComponent<Image>().color = new Color(0.30f, 0.39f, 0.49f);
                }
            }
        }
        //Check if hit killed someone
        foreach(GameObject character in cursor.GetComponent<CursorBehaviour>().Characters)
        {
            character.GetComponent<CharacterBehaviour>().death_check();
        }
        for (int i = 0; i < killed.Count; i++)
        {
            cursor.GetComponent<CursorBehaviour>().Characters.Remove(killed[i]);
            targets.Remove(killed[i]);
            Destroy(killed[i]);
        }
        killed.Clear();
        yield return new WaitForSeconds(1000);

        yield return new WaitForSeconds(3);
        
        ending_scene = true;
        cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponentInChildren<FEAnimator>().appear = false;
        cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponentInChildren<FFTAnimator>().appear = true;
        foreach(GameObject target in targets)
        {
            target.GetComponentInChildren<FEAnimator>().appear = false;
            Vector3 target_scale = target.GetComponentInChildren<FEAnimator>().transform.localScale;
            target.GetComponentInChildren<FEAnimator>().transform.localScale = new Vector3(target_scale.x * -1, target_scale.y, target_scale.z);
            target.GetComponentInChildren<FFTAnimator>().appear = true;
        }
        yield return new WaitForSeconds(2);
        cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().performing_attack = false;
        end_scene = true;
    }
    void battle_scene_control(List<GameObject> targets)
    {
        int i = 0;
        if (!ending_scene)
        {
            //Controlling attacker and attacker tile to correct battle position
            if (tween)
            {
                cursor.GetComponent<CursorBehaviour>().Characters[0].transform.DOMove(desired_attacker_position, motion_duration).SetEase(motion_type).OnComplete(()=> cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponentInChildren<FEAnimator>().update_animation_order());
                tween = false;
            }
            else 
            {
                cursor.GetComponent<CursorBehaviour>().Characters[0].transform.DOMove(desired_attacker_position, motion_duration).SetEase(motion_type);
            }
            cursor.GetComponent<CursorBehaviour>().Characters[0].transform.DORotateQuaternion(Quaternion.Euler(0, -45, -25), motion_duration).SetEase(motion_type);

            cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().current_tile.transform.DOMove(desired_attacker_position + tile_offset, motion_duration).SetEase(motion_type);

            cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().current_tile.transform.DOScale(new Vector3(2, 0.5f, 2), motion_duration).SetEase(motion_type);

            cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().current_tile.transform.DORotateQuaternion(Quaternion.Euler(0, -45, -25), motion_duration).SetEase(motion_type);
            //Controlling targets and target tile to correct battle position
            i = 0;
            foreach(GameObject target in targets)
            {
                switch(i)
                {
                    case 0:
                    target_to_battle(target, i);
                    break;
                    case 1:
                    target_to_battle(target, i);
                    break;
                    case 2:
                    target_to_battle(target, i);
                    break;
                }
                i++;
            }
        }

        if (ending_scene)
        {
            //Controlling attacker and attacker tile to correct original position
            cursor.GetComponent<CursorBehaviour>().Characters[0].transform.DOMove(attacker_original_position, motion_duration).SetEase(motion_type);
            
            cursor.GetComponent<CursorBehaviour>().Characters[0].transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), motion_duration).SetEase(motion_type);

            cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().current_tile.transform.DOMove(attacker_original_position + new Vector3(0, -0.75f, 0), motion_duration).SetEase(motion_type);

            cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().current_tile.transform.DOScale(new Vector3(1, 0.5f, 1), motion_duration).SetEase(motion_type);

            cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().current_tile.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), motion_duration).SetEase(motion_type);

            if (cursor.GetComponent<CursorBehaviour>().Characters[0].transform.position.y <= attacker_original_position.y + 0.01f)
            {
                cursor.GetComponent<CursorBehaviour>().Characters[0].transform.position = attacker_original_position;
            }
            if (cursor.GetComponent<CursorBehaviour>().Characters[0].transform.rotation.eulerAngles.x <= attacker_original_rotation.x + 0.01f)
            {
                cursor.GetComponent<CursorBehaviour>().Characters[0].transform.rotation = Quaternion.Euler(attacker_original_rotation);
            }
            if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().current_tile.transform.position.y <= attacker_original_position.y - 0.74f)
            {
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().current_tile.transform.position = attacker_original_position + new Vector3(0, -0.75f, 0);
            }
            if (cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().current_tile.transform.localScale.x <= 1.01f)
            {
                cursor.GetComponent<CursorBehaviour>().Characters[0].GetComponent<CharacterBehaviour>().current_tile.transform.localScale = new Vector3(1, 0.5f, 1);
            }
            
            //Controlling targets and target tile to correct original position
            i = 0;
            if (targets.Count > 0)
            {
                foreach(GameObject target in targets)
                {
                    switch(i)
                    {
                        case 0: 
                        target_to_origin(target, i);
                        target_reset_position(target, i);
                        break;
                        case 1:
                        target_to_origin(target, i);
                        target_reset_position(target, i);
                        break;
                        case 2:
                        target_to_origin(target, i);
                        target_reset_position(target, i);
                        break;
                    }
                    i++;
                }
            }
            i = 0;
            if (tiles.Count > 0)
            {
                foreach(EnabledTile tile in tiles)
                {
                    switch(i)
                    {
                        case 0:
                        tile_to_origin(tile, i);
                        tile_reset_position(tile, i);
                        break;
                        case 1:
                        tile_to_origin(tile, i);
                        tile_reset_position(tile, i);
                        break;
                        case 2:
                        tile_to_origin(tile, i);
                        tile_reset_position(tile, i);
                        break;
                    }
                    i++;
                }
            }
        }
        if (end_scene == true)
        {
            //Setting control over scene to false
            control_scene_player = false;
            ending_scene = false;
            end_scene = false;
            //Resetting target list and target position list of battle manager
            targets.Clear();
            target_original_position.Clear();
            tiles.Clear();
            tile_original_position.Clear();
            damage.Clear();
            battle_ended = false;
            cursor.GetComponent<CursorBehaviour>().turn_end();
        }
    }
    void target_reset_position(GameObject target, int i)
    {
        if (target.transform.position.y <= target_original_position[i].y + 0.01f)
        {
            target.transform.position = target_original_position[i];
        }
        if (target.transform.rotation.eulerAngles.x <= target_original_rotation[i].x + 0.01f)
        {
            target.transform.rotation = Quaternion.Euler(target_original_rotation[i]);
        }
    }
    void target_to_battle(GameObject target, int i)
    {
        if (i == 0) original_target_tile_position = target.GetComponent<CharacterBehaviour>().current_tile.transform.position;

        switch(i)
        {
            case 0: target.transform.DOMove(desired_target_position, motion_duration).SetEase(motion_type);; break;
            case 1: target.transform.DOMove(desired_target_position + target_side_offset, motion_duration).SetEase(motion_type);; break;
            case 2: target.transform.DOMove(desired_target_position - target_side_offset, motion_duration).SetEase(motion_type);; break;
        }
        target.transform.DORotateQuaternion(Quaternion.Euler(0, -45, -25), motion_duration).SetEase(motion_type);
        
        if (i != 0)
        {
            target.GetComponent<CharacterBehaviour>().current_tile.transform.DOMove(desired_target_position + tile_offset, motion_duration).SetEase(motion_type).ChangeStartValue(original_target_tile_position);
        }
        else target.GetComponent<CharacterBehaviour>().current_tile.transform.DOMove(desired_target_position + tile_offset, motion_duration).SetEase(motion_type);

        target.GetComponent<CharacterBehaviour>().current_tile.transform.DOScale(new Vector3(2, 0.5f, 2), motion_duration).SetEase(motion_type);
        
        target.GetComponent<CharacterBehaviour>().current_tile.transform.DORotateQuaternion(Quaternion.Euler(0, -45, -25), motion_duration).SetEase(motion_type);
    }
    void target_to_origin(GameObject target, int i)
    {

        target.transform.DOMove(target_original_position[i], motion_duration).SetEase(motion_type);
        
        target.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), motion_duration).SetEase(motion_type);
    }
    void tile_to_origin(EnabledTile tile, int i)
    {
        tile.transform.DOMove(tile_original_position[i], motion_duration).SetEase(motion_type);

        tile.transform.DOScale(new Vector3(1, 0.5f, 1), motion_duration).SetEase(motion_type);
        
        tile.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), motion_duration).SetEase(motion_type);
    }
    void tile_reset_position(EnabledTile tile, int i)
    {
        if (tile.transform.position.y <= tile_original_position[i].y + 0.01f)
        {
            tile.transform.position = tile_original_position[i];
        }
        if (tile.transform.localScale.x <= 1.01)
        {
            tile.transform.localScale = new Vector3(1, 0.5f, 1);
        }
    }
    public void add_damage(int dmg)
    {
        damage.Add(dmg);
    }
    public void deal_damage(int hits)
    {
        int i = 0;
        foreach(GameObject target in targets)
        {
            target.GetComponent<CharacterBehaviour>().take_damage(damage[i]/hits);
            i++;
        }
    }

}
