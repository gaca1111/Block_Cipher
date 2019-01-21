using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laundry_Data : MonoBehaviour {

    private BitArray raw_bits;
    private BitArray end_bits;
    private BitArray work_bits;
    private byte[] work_bytes;
    private List<Math_Functions> picked_functions;
    private int iterations;
    private Cipher_Type cipher_type;
    private string data;

    //=== BITS =====================================================================================================================================================

    public void Set_data(string data)
    {
        this.data = data;
    }

    public string Get_data()
    {
        return data;
    }

    public void Set_raw_bits(BitArray bits) {

        raw_bits = bits;
        Fill_up_bits();
    }

    public void Set_work_bytes(byte[] bytes)
    {
        work_bytes = bytes;
    }

    public byte[] Get_work_bytes()
    {
        return work_bytes;
    }

    public BitArray Get_raw_bits() {

        return raw_bits;
    }

    private void Fill_up_bits() {

        int segments = Mathf.CeilToInt((float)raw_bits.Length / (float)Static_Data.Get_double_segment_size());

        BitArray bits = new BitArray(segments * Static_Data.Get_double_segment_size());

        for (int i = 0; i < raw_bits.Length; i++) {

            bits[i] = raw_bits[i];
        }

        raw_bits = bits;
        work_bits = bits;
    }

    public List<BitArray> Get_bit_block(int number) {

        List<BitArray> bit_block = new List<BitArray>();
        BitArray bits = new BitArray(Static_Data.Get_segment_size());

        int counter = 0;

        for (int i = Static_Data.Get_double_segment_size() * number; i < Static_Data.Get_double_segment_size() * number + Static_Data.Get_segment_size(); i++) {

            bits[counter] = work_bits[i];

            counter++;
        }

        bit_block.Add(bits);
        bits = new BitArray(Static_Data.Get_segment_size());
        counter = 0;

        for (int i = Static_Data.Get_double_segment_size() * number + Static_Data.Get_segment_size(); i < Static_Data.Get_double_segment_size() * (number + 1); i++) {

            bits[counter] = work_bits[i];

            counter++;
        }

        bit_block.Add(bits);

        return bit_block;
    }

    public void Set_bit_block(BitArray bits, int number) {

        int counter = 0;

        for (int i = Static_Data.Get_double_segment_size() * number; i < Static_Data.Get_double_segment_size() * (number + 1); i++) {

            work_bits[i] = bits[counter];
            counter++;
        }
    }

    public void Set_work_bits(BitArray bits) {

        work_bits = bits;
    }

    public BitArray Get_work_bits() {

        return work_bits;
    }

    public void Set_end_bits(BitArray bits) {

        work_bits = bits;
    }

    public BitArray Get_end_bits() {

        return work_bits;
    }

    //=== OTHER =====================================================================================================================================================

    public void Set_picked_functions(List<Math_Functions> functions) {

        picked_functions = functions;
    }

    public List<Math_Functions> Get_picked_functions() {

        return picked_functions;
    }

    public void Set_iterations(int number) {

        iterations = number;
    }

    public int Get_iterations() {

        return iterations;
    }

    public void Set_cipher_type(Cipher_Type type) {

        cipher_type = type;
    }

    public Cipher_Type Get_cipher_type() {

        return cipher_type;
    }
}
