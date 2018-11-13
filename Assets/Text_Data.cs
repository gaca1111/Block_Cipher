using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Text_Data {

    private static string raw_text = "Raw data";
    private static string round_text = "Round - ";
    private static string block_text = " Block - ";
    private static string encrypted_end_text = "Encrypted message";
    private static string encrypted_text = "Encryption: ";
    private static string decrypted_text = "Decryption: ";
    private static string decrypted_end_text = "Decrypted message";

    public static string Get_raw_text() {

        return raw_text;
    }

    public static string Get_round_text() {

        return round_text;
    }

    public static string Get_block_text() {

        return block_text;
    }

    public static string Get_encrypted_end_text() {

        return encrypted_end_text;
    }

    public static string Get_encrypted_text() {

        return encrypted_text;
    }

    public static string Get_decrypted_text() {

        return decrypted_text;
    }

    public static string Get_decrypted_end_text() {

        return decrypted_end_text;
    }
}
