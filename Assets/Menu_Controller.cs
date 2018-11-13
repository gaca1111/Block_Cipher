using Crosstales.FB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Controller : MonoBehaviour {

    public Laundry_Data laundry_data;
    public Pick_Controller pick_controller;
    
    public Canvas menu_canvas;

    private string input_text;
    public Button input_button;
    public InputField input_field;

    private string txt_path;
    public Button txt_button;
    public Text txt_text;

    private string img_path;
    public Button img_button;
    public Text img_text;

    //=== MANAGE =====================================================================================================================================================

    private void Set_parameters() {

        input_text = "";
        input_field.text = "";
        txt_path = "";
        img_path = "";
    }

    private void Check_buttons() {

        if (input_text != "") {

            input_button.interactable = true;
        }
        else {

            input_button.interactable = false;
        }

        if (txt_path != "") {

            txt_button.interactable = true;
            txt_text.text = txt_path;
        }
        else {

            txt_button.interactable = false;
            txt_text.text = "";
        }

        if (img_path != "") {

            img_button.interactable = true;
            img_text.text = img_path;
        }
        else {

            img_button.interactable = false;
            img_text.text = "";
        }
    }

    private void Stand_by() {

        menu_canvas.gameObject.SetActive(false);
        Set_parameters();
        Check_buttons();
    }

    public void Restart() {

        menu_canvas.gameObject.SetActive(true);
        Set_parameters();
        Check_buttons();
    }

    private void Ready_up() {

        Set_parameters();
        Check_buttons();
    }

    private void Set_data(BitArray bits, Cipher_Type type) {

        laundry_data.Set_raw_bits(bits);
        laundry_data.Set_cipher_type(type);
    }

    //=== BUTTONS =====================================================================================================================================================

    public void On_click_exit_button() {

        Application.Quit();
    }

    public void On_click_start_input() {

        Set_data(Converter_Helper.Bytes_to_binary(Converter_Helper.String_to_bytes(input_text)), Cipher_Type.Txt);
        pick_controller.Ready_up();
        Stand_by();
    }

    public void On_click_start_txt() {

        Set_data(Converter_Helper.Bytes_to_binary(Converter_Helper.String_to_bytes(Converter_Helper.Txt_to_string(txt_path))), Cipher_Type.Txt);
        pick_controller.Ready_up();
        Stand_by();
    }

    public void On_click_start_img() {

        Set_data(Converter_Helper.Bytes_to_binary(Converter_Helper.Img_to_byte(img_path)), Cipher_Type.Img);
        pick_controller.Ready_up();
        Stand_by();
    }

    public void On_input_text_change(string text) {

        input_text = text;
        Check_buttons();
    }

    public void On_click_txt_browse() {

        ExtensionFilter[] extensions = new[] { new ExtensionFilter("Text Files", "txt"), };

        txt_path = FileBrowser.OpenSingleFile("Select txt", string.Empty, extensions);
        Check_buttons();
    }

    public void On_click_img_browse() {
 
        ExtensionFilter[] extensions = new[] { new ExtensionFilter("Image Files", "png", "jpg", "jpeg"), };

        img_path = FileBrowser.OpenSingleFile("Select image", string.Empty, extensions);
        Check_buttons();
    }

    //=== LOOP =====================================================================================================================================================

    void Start() {

        Ready_up();
    }

    void Update() {

    }
}
