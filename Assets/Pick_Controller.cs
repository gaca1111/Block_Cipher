using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pick_Controller : MonoBehaviour {

    public Laundry_Data laundry_data;
    public Menu_Controller menu_controller;
    public Cipher_Controller cipher_controller;

    public Canvas pick_canvas;
    public Button start_button;
    public Image F1;
    public Image F2;
    public Image F3;
    public Image F4;
    public InputField input_field;

    private List<Math_Functions> picked_functions;
    private string iteration_text;

    //=== MANAGE =====================================================================================================================================================

    public void Ready_up() {

        pick_canvas.gameObject.SetActive(true);
        F1.gameObject.SetActive(false);
        F2.gameObject.SetActive(false);
        F3.gameObject.SetActive(false);
        F4.gameObject.SetActive(false);

        picked_functions = new List<Math_Functions>();
        input_field.text = "3";
        iteration_text = input_field.text;

        Check_start_button();
    }

    private void Stand_by() {

        pick_canvas.gameObject.SetActive(false);
    }

    private void Set_data() {

        laundry_data.Set_picked_functions(picked_functions);       
        laundry_data.Set_iterations(Int32.Parse(iteration_text));
    }

    //=== BUTTONS =====================================================================================================================================================

    private void Check_start_button() {

        if (iteration_text != "") {

            if (Int32.Parse(iteration_text) > Static_Data.Get_max_iteration()) {

                input_field.text = Static_Data.Get_max_iteration().ToString();
            }
        }

        if (iteration_text == "0") {

            input_field.text = "1";
        }

        if (picked_functions.Count > 0 && input_field.text != "") {

            start_button.interactable = true;
        }
        else {

            start_button.interactable = false;
        }
    }

    public void On_click_start_cipher() {

        Set_data();
        cipher_controller.Ready_up_encryption();
        Stand_by();
    }

    public void On_click_back() {

        Stand_by();
        menu_controller.Restart();
    }

    public void On_click_F1() {

        if (F1.gameObject.activeSelf == true) {

            F1.gameObject.SetActive(false);
            picked_functions.Remove(Math_Functions.F1);
        }
        else {

            F1.gameObject.SetActive(true);
            picked_functions.Add(Math_Functions.F1);
        }

        Check_start_button();
    }

    public void On_click_F2() {

        if (F2.gameObject.activeSelf == true) {

            F2.gameObject.SetActive(false);
            picked_functions.Remove(Math_Functions.F2);
        }
        else {

            F2.gameObject.SetActive(true);
            picked_functions.Add(Math_Functions.F2);
        }

        Check_start_button();
    }

    public void On_click_F3() {

        if (F3.gameObject.activeSelf == true) {

            F3.gameObject.SetActive(false);
            picked_functions.Remove(Math_Functions.F3);
        }
        else {

            F3.gameObject.SetActive(true);
            picked_functions.Add(Math_Functions.F3);
        }

        Check_start_button();
    }

    public void On_click_F4() {

        if (F4.gameObject.activeSelf == true) {

            F4.gameObject.SetActive(false);
            picked_functions.Remove(Math_Functions.F4);
        }
        else {

            F4.gameObject.SetActive(true);
            picked_functions.Add(Math_Functions.F4);
        }

        Check_start_button();
    }

    public void On_input_text_change(string text) {

        iteration_text = text;
        Check_start_button();
    }

    //=== LOOP =====================================================================================================================================================

    void Start () {
		
	}
	
	void Update () {
		
	}
}
