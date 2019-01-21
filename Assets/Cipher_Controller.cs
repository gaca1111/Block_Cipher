using System;
using Crosstales.FB;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text;

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

    private ulong[] encrypted;
    private ulong[] decrypted;
    private string message;
    private string emessage;
    private string dmessage;
    private byte[] steps;
    //private BitArray current_key;

    //=== MANAGE =====================================================================================================================================================

    public static byte[] BitArrayToByteArray(BitArray bits)
    {
        byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
        bits.CopyTo(ret, 0);
        return ret;
    }


    public void Ready_up_encryption()
    {


        message = laundry_data.Get_data();
        Russian();

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
        //current_key = Static_Data.Get_key();

        steps = laundry_data.Get_work_bytes();
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
        //current_key = Static_Data.Get_decryption_key();
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

    private BitArray Calculate_block()
    {

        var list_block = laundry_data.Get_bit_block(current_block);
        BitArray block1 = list_block[0];
        BitArray block2 = list_block[1];

        BitArray block2t = new BitArray(block2.Length);

        for (int i = 0; i < block2.Length; i++)
        {



            block2t[i] = block2[i];

            Debug.Log(block2[i] + " " + block2t[i]);

        }

        BitArray double_block = new BitArray(Static_Data.Get_double_segment_size());

        // === FUNKCJE + KLUCZ ===

        int[] perm_tab = {16,   7,  20,  21, 29,  12,  28,  17, 1,  15,  23,  26,
                5,  18,  31,  10, 2,   8,  24,  14, 32,  27,   3,   9, 19,  13,  30,   6,
                22,  11,   4,  25};






        if (picked_functions.Contains(Math_Functions.F1))
        {



            for (int i = 0; i < Static_Data.Get_segment_size(); i++)
            {

                // Debug.Log(block2[i]);



                block2[i] = block2[perm_tab[i] - 1];
            }
        }

        if (picked_functions.Contains(Math_Functions.F2))
        {

        }

        if (picked_functions.Contains(Math_Functions.F3))
        {

        }

        if (picked_functions.Contains(Math_Functions.F4))
        {

        }




        // === XOR ===

        for (int i = 0; i < block1.Length; i++)
        {

            block2[i] = block1[i] ^ block2[i];        
        }

        // === ZAMIANA MIEJSCAMI

        for (int i = 0; i < Static_Data.Get_segment_size(); i++)
        {

            //Debug.Log(block2t[i]);

            double_block[i] = block2t[i];
        }

        int counter = 0;

        for (int i = Static_Data.Get_segment_size(); i < double_block.Length; i++)
        {

            double_block[i] = block2[counter];
            counter++;
        }

        // === DOPISANIE GOTOWEGO BLOKU DO PELNIEJ WIADOMOSCI ===

        laundry_data.Set_bit_block(double_block, current_block);

        current_block++;

        if (current_block >= number_of_blocks)
        {

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

        //work_bits = laundry_data.Get_work_bits();

        BitArray bits = null;

        for (int i = 0; i < encrypted.Length; i++)
        {
            if (i == 0)
            {
                bits=new BitArray(BitConverter.GetBytes(encrypted[i]));
            }
            else
            {
                bits.Append(new BitArray(BitConverter.GetBytes(encrypted[i])));
            }
        }

        
        Set_current(bits, Calculate_sqrt_floor(bits.Length));
        Create_img(current_bits, currnet_sqtr_size);

        Ready_up_dectyption();
    }

    private void Decryption_end() {

        title_text.text = Text_Data.Get_decrypted_end_text();

        BitArray bits = null;

        for (int i = 0; i < decrypted.Length; i++)
        {
            if (i == 0)
            {
                bits = new BitArray(BitConverter.GetBytes(decrypted[i]));
            }
            else
            {
                bits.Append(new BitArray(BitConverter.GetBytes(decrypted[i])));
            }
        }

        Set_current(bits, Calculate_sqrt_floor(bits.Length));
        Create_img(current_bits, currnet_sqtr_size);

        left_main.SetActive(false);
        var bytes = Converter_Helper.Binary_to_byte(current_bits);

        if (cipher_type == Cipher_Type.Txt) {

            left_txt.SetActive(true);

            var txt = dmessage;
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

    private void Russian()
    {
        byte rounds = Convert.ToByte(laundry_data.Get_iterations());
        var key64 = RandomKey();
        //Console.WriteLine($"Key64: {key64:X}");
        var iv = RandomKey();
        //Console.WriteLine($"IV: {iv:X}");
       // Console.Write("Message: ");
        //var message = Padding(Console.ReadLine());
        var mesgC = ToBlocks(Padding(message));
        //Console.WriteLine("Encrypted: ");
        for (var i = 0; i < mesgC.Length; i++)
        {
            mesgC[i] = Encrypt(i == 0 ? mesgC[i] ^ iv : mesgC[i] ^ mesgC[i - 1], key64, rounds);
            //Console.WriteLine($"{mesgC[i]:X}");
        }

        encrypted = mesgC;
        emessage = MessageToString(mesgC);
        //Console.WriteLine($"ENCRYPTED MESSAGE: {message}");

        var mesgP = new ulong[mesgC.Length];
        mesgC.CopyTo(mesgP, 0);
        //Console.WriteLine("Decrypted: ");
        for (var i = 0; i < mesgP.Length; i++)
        {
            mesgP[i] = i == 0 ? iv ^ Decrypt(mesgP[i], key64, rounds) : mesgC[i - 1] ^ Decrypt(mesgP[i], key64, rounds);
            //Console.WriteLine($"{mesgP[i]:X}");
        }

        decrypted = mesgP;
        dmessage = MessageToString(mesgP);
        //Console.WriteLine($"DECRYPTED MESSAGE: {message}");
        //Console.ReadKey();
        //Console.ReadKey();
        //Console.ReadKey();
        //Console.ReadKey();
    }

    private static string Padding(string input)
    {
        var n = input.Length * 16 % 64;
        if (n == 0) return input;
        var sb = new StringBuilder(input);
        var k = (64 - n) / 16;
        sb.Append(new char(), k);
        return sb.ToString();
    }

    private static ulong[] ToBlocks(string input)
    {
        var result = new ulong[input.Length / 4];
        var temp = new uint[2];
        for (int i = 0, j = 0; i < input.Length; i += 4, j++)
        {
            temp[0] = (uint)input[i] << 16 | input[i + 1];
            temp[1] = (uint)input[i + 2] << 16 | input[i + 3];

            result[j] = (ulong)temp[0] << 2 * 16 | temp[1];
        }
        return result;
    }

    private static ulong RandomKey()
    {
        var rand = new System.Random((int)(DateTime.Now.Ticks & 0xFFFFFFFF));
        var buffer = new byte[sizeof(ulong)];
        rand.NextBytes(buffer);
        ulong res = 0;
        for (var i = 0; i < sizeof(ulong); i++)
        {
            ulong temp = buffer[i];
            temp = temp << 8 * (7 - i);
            res = res | temp;
        }
        return res;
    }

    private ulong Encrypt(ulong msg, ulong key64, uint rounds)
    {
        var right = (uint)(msg << 2 * 16 >> 2 * 16);
        var left = (uint)(msg >> 2 * 16);
        for (var i = 0; i < rounds; i++)
        {
            var key32I = KeyGenerator(i, key64);
            
            uint function = 0;
            if (laundry_data.Get_picked_functions().Contains(Math_Functions.F1))
            {
                function = F1(right, key32I);
            }

            if (laundry_data.Get_picked_functions().Contains(Math_Functions.F2))
            {
                function = F2(right, key32I);
            }

            if (laundry_data.Get_picked_functions().Contains(Math_Functions.F3))
            {
                function = F3(right, key32I);
            }

            if (laundry_data.Get_picked_functions().Contains(Math_Functions.F4))
            {
                function = F4(right, key32I);
            }
            var tmp = right;
            right = left ^ function;
            left = tmp;
        }
        var tmp1 = (ulong)left << 2 * 16;
        var tmp2 = (ulong)right;
        return tmp1 | tmp2;
    }

    private static string MessageToString(IEnumerable<ulong> msg)
    {
        var result = string.Empty;
        var tmp = new ushort[4];
        foreach (var item in msg)
        {
            tmp[0] = (ushort)(item >> 3 * 16);
            tmp[1] = (ushort)(item >> 2 * 16 << 3 * 16 >> 3 * 16);
            tmp[2] = (ushort)(item << 2 * 16 >> 3 * 16);
            tmp[3] = (ushort)(item << 3 * 16 >> 3 * 16);
            result = tmp.Aggregate(result, (current, t) => current + Convert.ToChar(t));
        }
        return result;
    }

    #region cycleMove
    private static ulong CycleMoveRight(ulong number, byte offset) => number >> offset | number << 64 - offset;

    private static uint CycleMoveRight(uint number, byte offset) => number >> offset | number << 32 - offset;

    private static uint CycleMoveLeft(uint number, byte offset) => number << offset | number >> 32 - offset;
    #endregion

    private static uint F1(uint right, uint key) => CycleMoveLeft(right, 2) ^ ~(CycleMoveRight(key, 11) + right);

    private static uint F2(uint right, uint key)
    {
        byte[] bytes = BitConverter.GetBytes(right);

        string s = ByteArrayToString(bytes);
        s = s.Replace('0', 'c');
        s = s.Replace('1', '5');
        s = s.Replace('2', '6');
        s = s.Replace('3', 'b');
        s = s.Replace('4', '9');
        s = s.Replace('5', '0');
        s = s.Replace('6', 'a');
        s = s.Replace('7', 'd');
        s = s.Replace('8', '3');
        s = s.Replace('9', 'e');
        s = s.Replace('a', 'f');
        s = s.Replace('b', '8');
        s = s.Replace('c', '4');
        s = s.Replace('d', '7');
        s = s.Replace('e', '1');
        s = s.Replace('f', '2');

        byte[] resultByteArray = StringToByteArray(s);
        uint result = BitConverter.ToUInt32(resultByteArray, 0) ^ key;

        return result;
    }

    private static uint KeyGenerator(int round, ulong key64) => (uint)(CycleMoveRight(key64, (byte)(round * 4)) << 5 >> 37);

    private  ulong Decrypt(ulong msg, ulong key64, int iteration)
    {
        var right = (uint)(msg << 2 * 16 >> 2 * 16);
        var left = (uint)(msg >> 2 * 16);
        for (var i = iteration - 1; i >= 0; i--)
        {
            var key32I = KeyGenerator(i, key64);
            uint function = 0;
            if (laundry_data.Get_picked_functions().Contains(Math_Functions.F1))
            {
                function = F1(left, key32I);
            }

            if (laundry_data.Get_picked_functions().Contains(Math_Functions.F2))
            {
                function = F2(left, key32I);
            }

            if (laundry_data.Get_picked_functions().Contains(Math_Functions.F3))
            {
                function = F3(left, key32I);
            }

            if (laundry_data.Get_picked_functions().Contains(Math_Functions.F4))
            {
                function = F4(left, key32I);
            }
            var tmp = left;
            left = right ^ function;
            right = tmp;
        }
        var tmp1 = (ulong)left << 2 * 16;
        var tmp2 = (ulong)right;
        return tmp1 | tmp2;
    }

    public static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    public static byte[] StringToByteArray(String hex)
    {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    public static byte[] XOR(byte[] buffer1, byte[] buffer2)
    {
        for (int i = 0; i < buffer1.Length; i++)
            buffer1[i] ^= buffer2[i];
        return buffer1;
    }
    private static uint F3(uint right, uint key)
    {
        byte[] tab = BitConverter.GetBytes(right);
        byte[] keyTab = BitConverter.GetBytes(key);
        byte[] tabtemp = XOR(keyTab, tab);

        byte tone = tabtemp[0];
        byte ttwo = tabtemp[1];
        byte tthr = tabtemp[2];
        byte tfou = tabtemp[3];

        tabtemp[0] = tthr;
        tabtemp[1] = tone;
        tabtemp[2] = tfou;
        tabtemp[3] = ttwo;

        tabtemp = XOR(keyTab, tabtemp);

        return BitConverter.ToUInt32(tabtemp, 0);
    }

    private static uint F4(uint right, uint key)
    {
        byte[] input = BitConverter.GetBytes(right);
        byte[] inputKey = BitConverter.GetBytes(key);

        BitArray tab = new BitArray(input);
        BitArray tabKey = new BitArray(inputKey);

        tab = tab.Xor(tabKey);
        int[] bitPerm = { 0, 6, 11, 13, 19, 20, 25, 20, 3, 4, 9, 14, 16, 22, 27, 29, 2, 5, 8, 15, 17, 23, 26, 28, 1, 7, 9, 10, 18, 21, 24, 31 };
        for (int i = 0; i < tab.Length; i++)
        {
            tab[i] = tab[bitPerm[i]];
        }
        byte[] tempu = BitArrayToByteArray(tab);
        return BitConverter.ToUInt32(tempu, 0);
    }
}




