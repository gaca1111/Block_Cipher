using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Math_Functions { F1, F2, F3, F4 };
public enum Cipher_State { Encryption, Decryption };
public enum Cipher_Type { Txt, Img };

public static class Static_Data {

    private static int max_iteration = 32;

    private static int max_txt_file = 1024;

    private static int double_segment_size = 64;
    private static int segment_size = 32;

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
}
