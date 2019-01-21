using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Lang;

public enum Math_Functions { F1, F2, F3, F4 };
public enum Cipher_State { Encryption, Decryption };
public enum Cipher_Type { Txt, Img };

public static class Static_Data {

    private static int max_iteration = 32;

    private static int max_txt_file = 1024;

    private static int double_segment_size = 64;
    private static int segment_size = 32;

    private static readonly BitArray key = new BitArray(new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 });
    private static readonly BitArray decrypt_key = Reverse(key);


    public static int Get_max_iteration() {

        return max_iteration;
    }

    public static int Get_max_txt_file() {

        return max_txt_file;
    }

    public static int Get_double_segment_size() {

        return double_segment_size;
    }

    public static int Get_segment_size() {

        return segment_size;
    }

    public static BitArray Get_key()
    {
        return key;
    }

    public static BitArray Get_decryption_key()
    {
        return decrypt_key;
    }

    public static BitArray Reverse(BitArray data_in)
    {
        BitArray array = data_in;

        int length = array.Length;
        int mid = (length / 2);

        for (int i = 0; i < mid; i++)
        {
            bool bit = array[i];
            array[i] = array[length - i - 1];
            array[length - i - 1] = bit;
        }

        return array;
    }
    public static BitArray Append(this BitArray current, BitArray after)
    {
        var bools = new bool[current.Count + after.Count];
        current.CopyTo(bools, 0);
        after.CopyTo(bools, current.Count);
        return new BitArray(bools);
    }
}
