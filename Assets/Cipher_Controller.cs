using Crosstales.FB;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Cipher_Controller : MonoBehaviour {

    public Laundry_Data laundry_data;
    public Menu_Controller menu_controller;

    public Canvas cipher_canvas;
    public Button txt_download_button;
    public GameObject left_main;
    public GameObject left_txt;
    public InputField left_txt_input_field;
    public GameObject left_img;
    public UnityEngine.UI.Image left_img_img;
    public Text title_text;
    public UnityEngine.UI.Image image;

    private Bitmap bitmap;
    private int full_sqtr_size;
    private int currnet_sqtr_size;

    private BitArray work_bits;
    private BitArray current_bits;
    private int number_of_iterations;
    private int current_iteration;
    private bool end_of_iteration;
    private int number_of_blocks;
    private int current_block;
    private bool end_of_blocks;

    private List<Math_Functions> picked_functions;  
    private Cipher_State cipher_state;
    private Cipher_Type cipher_type;

    private string left_txt_output;

    //=== MANAGE =====================================================================================================================================================

    public void Ready_up_encryption() {

        cipher_canvas.gameObject.SetActive(true);
        left_main.SetActive(true);
        left_txt.SetActive(false);
        left_img.SetActive(false);

        work_bits = laundry_data.Get_raw_bits();
        picked_functions = laundry_data.Get_picked_functions();
        number_of_iterations = laundry_data.Get_iterations();
        current_iteration = 0;
        end_of_iteration = false;
        number_of_blocks = work_bits.Length / Static_Data.Get_double_segment_size();
        current_block = 0;
        end_of_blocks = false;
        cipher_state = Cipher_State.Encryption;
        cipher_type = laundry_data.Get_cipher_type();
        Set_current(work_bits, full_sqtr_size);

        full_sqtr_size = Calculate_sqrt_floor(work_bits.Length);
        Create_img(work_bits, full_sqtr_size);
        title_text.text = Text_Data.Get_raw_text();
    }

    private void Ready_up_dectyption() {

        work_bits = laundry_data.Get_work_bits();
        Set_current(work_bits, Calculate_sqrt_floor(work_bits.Length));
        laundry_data.Set_end_bits(work_bits);
        current_iteration = 0;
        end_of_iteration = false;
        current_block = 0;
        end_of_blocks = false;
        cipher_state = Cipher_State.Decryption;

        // klucz tyl do przodu dac
    }

    private int Calculate_sqrt_floor(int number) {

        return Mathf.FloorToInt(Mathf.Sqrt(number));
    }

    private void Set_current(BitArray bits, int sqtr_size) {

        current_bits = bits;
        currnet_sqtr_size = sqtr_size;

        if (bits.Length > Static_Data.Get_max_txt_file()) {

            txt_download_button.interactable = false;
        }
        else {

            txt_download_button.interactable = true;
        }
    }

    private void Create_img(BitArray bits, int sqtr_size) {

        bitmap = new Bitmap(sqtr_size, sqtr_size);

        int counter = 0;

        for (int i = 0; i < sqtr_size; i++) {

            for (int j = 0; j < sqtr_size; j++) {

                if (bits[counter] == true) {

                    bitmap.SetPixel(j, i, System.Drawing.Color.White);
                }
                else {

                    bitmap.SetPixel(j, i, System.Drawing.Color.Black);
                }

                counter++;
            }
        }

        bitmap.Save(Application.persistentDataPath + "/Output.png");
        WWW www = new WWW("file:///" + Application.persistentDataPath + "/Output.png");
        Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        sprite.texture.filterMode = FilterMode.Point;
        image.sprite = sprite;
    }

    private void Stand_by() {

        File.Delete(Application.persistentDataPath + "/Output.png");
        File.Delete(Application.persistentDataPath + "/img.jpeg");

        cipher_canvas.gameObject.SetActive(false);
    }

    //=== BUTTONS =====================================================================================================================================================

    public void On_click_back() {

        Stand_by();
        menu_controller.Restart();
    }

    public void On_click_download_txt() {
        
        string path = FileBrowser.SaveFile("Save as txt", string.Empty, "txt_result", "txt");
        Converter_Helper.String_to_txt(Converter_Helper.Bit_to_string(current_bits, currnet_sqtr_size), path);
    }

    public void On_click_download_img() {

        string path = FileBrowser.SaveFile("Save as img", string.Empty, "img_result", "jpeg");
        Converter_Helper.Bitmap_to_img(bitmap, path);
    }

    public void On_click_block_button() {

        if (!end_of_iteration) {

            if (!end_of_blocks) {

                Show_block();
            }
            else {

                Show_iteration();
            }
        }
        else {

            Show_end();
        }
    }

    public void On_click_iteration_button() {

        if (!end_of_iteration) {

            Calculate_iteration();
        }
        else {

            Show_end();
        }
    }

    public void On_click_end_button() {

        Calculate_end();
    }

    public void On_click_left_txt_refresh() {

        left_txt_input_field.text = left_txt_output;
    }

    public void On_click_left_txt_download_txt() {

        string path = FileBrowser.SaveFile("Save as txt", string.Empty, "txt_result", "txt");
        Converter_Helper.String_to_txt(left_txt_output, path);
    }

    public void On_click_left_img_download_img() {

        string path = FileBrowser.SaveFile("Save as img", string.Empty, "img_result", "jpeg");
        File.Copy(Application.persistentDataPath + "/img.jpeg", path);
    }

    //=== CIPHER =====================================================================================================================================================

    private void Show_block() {

        int temp_round = current_iteration + 1;
        int temp_block = current_block + 1;

        if (cipher_state == Cipher_State.Encryption) {

            title_text.text = Text_Data.Get_encrypted_text() + Text_Data.Get_round_text() + temp_round.ToString() + Text_Data.Get_block_text() + temp_block.ToString();
        }
        else {

            title_text.text = Text_Data.Get_decrypted_text() + Text_Data.Get_round_text() + temp_round.ToString() + Text_Data.Get_block_text() + temp_block.ToString();
        }

        BitArray bits_temp = Calculate_block();
        Set_current(bits_temp, Calculate_sqrt_floor(bits_temp.Length));
        Create_img(current_bits, currnet_sqtr_size);
    }

    private BitArray Calculate_block() {

        var list_block = laundry_data.Get_bit_block(current_block);
        BitArray block1 = list_block[0];
        BitArray block2 = list_block[1];

        BitArray double_block = new BitArray(Static_Data.Get_double_segment_size());

        // === FUNKCJE + KLUCZ ===

        if (picked_functions.Contains(Math_Functions.F1)) {

        }

        if (picked_functions.Contains(Math_Functions.F2)) {

        }

        if (picked_functions.Contains(Math_Functions.F3)) {

        }

        if (picked_functions.Contains(Math_Functions.F4)) {

        }

        // === XOR ===

        for (int i = 0; i < block1.Length; i++) {

            //block1[i] = block1[i] ^ block2[i];        
        }

        // === ZAMIANA MIEJSCAMI

        for (int i = 0; i < Static_Data.Get_segment_size(); i++) {

            double_block[i] = block2[i];
        }

        int counter = 0;

        for (int i = Static_Data.Get_segment_size(); i < double_block.Length; i++) {

            double_block[i] = block1[counter];
            counter++;
        }

        // === DOPISANIE GOTOWEGO BLOKU DO PELNIEJ WIADOMOSCI ===

        laundry_data.Set_bit_block(double_block, current_block);

        current_block++;

        if (current_block >= number_of_blocks) {

            end_of_blocks = true;
        }
        
        return double_block;
    }

    private void Show_iteration() {

        int temp_iteration = current_iteration + 1;

        if (cipher_state == Cipher_State.Encryption) {

            title_text.text = Text_Data.Get_encrypted_text() + Text_Data.Get_round_text() + temp_iteration.ToString();
        }
        else {

            title_text.text = Text_Data.Get_decrypted_text() + Text_Data.Get_round_text() + temp_iteration.ToString();
        }
            
        work_bits = laundry_data.Get_work_bits();
        end_of_blocks = false;
        current_block = 0;
        current_iteration++;

        if (current_iteration >= number_of_iterations) {

            end_of_iteration = true;
        }
    
        Set_current(work_bits, Calculate_sqrt_floor(work_bits.Length));
        Create_img(current_bits, currnet_sqtr_size);
    }

    private void Calculate_iteration() {

        for (int i = current_block; i < number_of_blocks; i++) {

            Calculate_block();
        }

        Show_iteration();
    }

    private void Show_end() {

        if (cipher_state == Cipher_State.Encryption) {

            Encryption_end();
        }
        else {

            Decryption_end();
        }
    }

    private void Encryption_end() {

        title_text.text = Text_Data.Get_encrypted_end_text();

        work_bits = laundry_data.Get_work_bits();

        Set_current(work_bits, Calculate_sqrt_floor(work_bits.Length));
        Create_img(current_bits, currnet_sqtr_size);

        Ready_up_dectyption();
    }

    private void Decryption_end() {

        title_text.text = Text_Data.Get_decrypted_end_text();

        work_bits = laundry_data.Get_work_bits();

        Set_current(work_bits, Calculate_sqrt_floor(work_bits.Length));
        Create_img(current_bits, currnet_sqtr_size);

        left_main.SetActive(false);
        var bytes = Converter_Helper.Binary_to_byte(current_bits);

        if (cipher_type == Cipher_Type.Txt) {

            left_txt.SetActive(true);

            var txt = Converter_Helper.Bytes_to_string(bytes);
            left_txt_output = txt;
            left_txt_input_field.text = left_txt_output;
        }
        else {

            left_img.SetActive(true);

            var img = Converter_Helper.Bytes_to_img(bytes);
            img.Save(Application.persistentDataPath + "/img.jpeg");
            WWW www = new WWW("file:///" + Application.persistentDataPath + "/img.jpeg");
            Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            left_img_img.sprite = sprite;
        }
    }

    private void Calculate_end() {

        for (int i = current_iteration; i < number_of_iterations; i++) {

            Calculate_iteration();
        }

        Show_end();
    }

    //=== LOOP =====================================================================================================================================================

    void Start () {
		
	}

	void Update () {
		
	}
}
