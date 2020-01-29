﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System;
using System.Text;
using System.IO;

public class GetHandPosition : MonoBehaviour
{

    Controller controller = new Controller();
    public int count = 0; //計測回数
    public int rightHandCount = 0;  //右手を書き込んだ瞬間
    public int leftHandCount = 0;  //左手の書き込んだ瞬間
    public static int file_num = 0;  //書き込みファイルナンバー

    // 座標値保存用
    // 右手
    public static Leap.Vector R_hand_pos = new Leap.Vector();
    public static Leap.Vector R_hand_vec = new Leap.Vector();
    public static Leap.Vector R_thumb = new Leap.Vector();
    public static Leap.Vector R_index = new Leap.Vector();
    public static Leap.Vector R_middle = new Leap.Vector();
    public static Leap.Vector R_ring = new Leap.Vector();
    public static Leap.Vector R_pinky = new Leap.Vector();
    // 左手
    public static Leap.Vector L_hand_pos = new Leap.Vector();
    public static Leap.Vector L_hand_vec = new Leap.Vector();
    public static Leap.Vector L_thumb = new Leap.Vector();
    public static Leap.Vector L_index = new Leap.Vector();
    public static Leap.Vector L_middle = new Leap.Vector();
    public static Leap.Vector L_ring = new Leap.Vector();
    public static Leap.Vector L_pinky = new Leap.Vector();
    // 手判別フラグ
    public static String identifyHandFlag;

    // Start is called before the first frame update
    void Start()
    {
        file_num = 0;
        //ファイル書き出し
        string filePath = string.Format(@"./Assets/Data/data{0}.csv", file_num);
        StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("Shift_JIS"));
        //ヘッダー出力
        string[] s1 = { "R_hand : x", "R_hand : y", "R_hand : z", 
                        "R_thumb : x", "R_thumb : y", "R_thumb : z", 
                        "R_index : x", "R_index : y", "R_index : zx", 
                        "R_middle : x", "R_middle : y", "R_middle : z",
                        "R_ring : x", "R_ring : y", "R_ring : z", 
                        "R_pinky : x", "R_pinky : y", "R_pinky : z", 
                        "R_hand_vec : x", "R_hand_vec : y", "R_hand_vec : z",
                        "R_fin_to_fin :x", "R_fin_to_fin :y", "R_fin_to_fin :z",
                        "R_dis_thumb", "R_dis_index", "R_dis_middle", "R_dis_ring", "R_dis_pinky"};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        sw.Close();
    }




    // Update is called once per frame
    void Update()
    {
        Frame frame = controller.Frame();
        HandList hands = frame.Hands;
        Hand leftHand = null, rightHand = null;
        identifyHandFlag = "";

        // 右手、左手を取得
        if (hands.Count == 1){
            rightHand = hands[0].IsRight ? hands[0] : null;
            leftHand = hands[0].IsLeft ? hands[0] : null;
            count++;
        }
        else if (1 < hands.Count & hands.Count < 3){
            rightHand = hands[0].IsRight ? hands[0] : hands[1];
            leftHand = hands[0].IsLeft ? hands[0] : hands[1];
            count++;
        }
        else {
            rightHand = null;
            leftHand = null;
        }

        if ( 1 <= count && count < 201){
            // csv出力処理
            // 右手のみの場合
            if (rightHand != null && leftHand == null){
                outputRightHand(rightHand);
                rightHandCount++;
            }
            else if (rightHand == null && leftHand != null){
                outputLeftHand(leftHand);
                leftHandCount++;
            }
            // // 両手の場合
            else if (rightHand != null && leftHand != null){
                outputBothHands(rightHand, leftHand);
                rightHandCount++;
                leftHandCount++;
            }
            // Debug.Log("計測中 file_num :" + file_num + ", count" + count);
        }else if (201 <= count){
            count = 0;
            rightHandCount = 0;
            leftHandCount = 0;
            file_num++;
            Create_csv();
        }
    }





    // 右手のみの検出処理
    public void outputRightHand(Hand rightHand)
    {
        foreach (Finger finger in rightHand.Fingers){
            identifyHandFlag = "RightHand";
            setFingerPos(identifyHandFlag, finger.Id, finger.TipPosition);
            // Debug.Log("id :" + finger.Id +  ", 右手指 :" + finger.Type());
        }
        float[] vector_data = new float[29];
        vector_data = SetData(rightHand.PalmPosition, R_thumb, R_index, R_middle, R_ring, R_pinky, rightHand.PalmNormal);
        Write_csv(vector_data);
        // Debug.Log("____________");

    }

    // 左手のみの検出処理
    public void outputLeftHand(Hand leftHand)
    {
        foreach (Finger finger in leftHand.Fingers){
            Debug.Log("id :" + finger.Id +  ", 左手指 :" + finger.Type());
        }
        Debug.Log("____________");

    }

    // 両手のみの検出処理
    public void outputBothHands(Hand rightHand, Hand leftHand)
    {
        Debug.Log ("両手を認知");
        foreach (Finger finger in rightHand.Fingers){
            Debug.Log("id :" + finger.Id +  ", 右手指 :" + finger.Type());
        }
        
        foreach (Finger finger in leftHand.Fingers){
            Debug.Log("id :" + finger.Id +  ", 左手指 :" + finger.Type());
        }

        Debug.Log("____________");
    }

    // 座標セット
    public void setFingerPos(String hand, int flag, Leap.Vector finger){
        if (hand == "RightHand"){
            switch(flag % 10)
            {
                case 0:
                    R_thumb.x = finger.x;
                    R_thumb.y = finger.y;
                    R_thumb.z = finger.z;
                    break;
                case 1: 
                    R_index.x = finger.x;
                    R_index.y = finger.y;
                    R_index.z = finger.z;
                    break;
                case 2:
                    R_middle.x = finger.x;
                    R_middle.y = finger.y;
                    R_middle.z = finger.z;
                    break;
                case 3:
                    R_ring.x = finger.x;
                    R_ring.y = finger.y;
                    R_ring.z = finger.z;
                    break;
                case 4:
                    R_pinky.x = finger.x;
                    R_pinky.y = finger.y;
                    R_pinky.z = finger.z;
                    break;
                default:
                    break;
            }
        }else if(hand == "LeftHand"){
            switch(flag % 10)
            {
                case 0: 
                    L_thumb.x = finger.x;
                    L_thumb.y = finger.y;
                    L_thumb.z = finger.z;
                    break;
                case 1: 
                    L_index.x = finger.x;
                    L_index.y = finger.y;
                    L_index.z = finger.z;
                    break;
                case 2:
                    L_middle.x = finger.x;
                    L_middle.y = finger.y;
                    L_middle.z = finger.z;
                    break;
                case 3:
                    L_ring.x = finger.x;
                    L_ring.y = finger.y;
                    L_ring.z = finger.z;
                    break;
                case 4:
                    L_pinky.x = finger.x;
                    L_pinky.y = finger.y;
                    L_pinky.z = finger.z;
                    break;
                default:
                    break;
            }
        }
    }

    //取得したベクトルを全て配列に格納
    float [] SetData(Leap.Vector hand_pos, Leap.Vector thumb, Leap.Vector index, Leap.Vector middle, Leap.Vector ring, Leap.Vector pinky, Leap.Vector vec_hand){
        //毎フレームごとに必要なデータを格納する配列
        float[] data = new float[29];
        //手の中心座標
        data[0] = hand_pos.x;
        data[1] = hand_pos.y;
        data[2] = hand_pos.z;
        //親指ベクトル
        data[3] = thumb.x;
        data[4] = thumb.y;
        data[5] = thumb.z;
        //人差し指ベクトル
        data[6] = index.x - hand_pos.x;
        data[7] = index.y - hand_pos.y;
        data[8] = index.z - hand_pos.z;
        //中指ベクトル
        data[9] = middle.x - hand_pos.x;
        data[10] = middle.y - hand_pos.y;
        data[11] = middle.z - hand_pos.z;
        //薬指ベクトル
        data[12] = ring.x - hand_pos.x;
        data[13] = ring.y - hand_pos.y;
        data[14] = ring.z - hand_pos.z;
        //小指ベクトル
        data[15] = pinky.x - hand_pos.x;
        data[16] = pinky.y - hand_pos.y;
        data[17] = pinky.z - hand_pos.z;
        //手の平法線
        data[18] = vec_hand.x;
        data[19] = vec_hand.y;
        data[20] = vec_hand.z;
        // 人差し指から中指
        data[21] = middle.x - index.x;
        data[22] = middle.y - index.y;
        data[23] = middle.z - index.z;
        // 指先の距離
        data[24] = (float)Math.Sqrt(System.Math.Pow((double)data[3], (double)2) + System.Math.Pow((double)data[4], (double)2) + System.Math.Pow((double)data[5], (double)2));
        data[25] = (float)Math.Sqrt(System.Math.Pow((double)data[6], (double)2) + System.Math.Pow((double)data[7], (double)2) + System.Math.Pow((double)data[8], (double)2));
        data[26] = (float)Math.Sqrt(System.Math.Pow((double)data[9], (double)2) + System.Math.Pow((double)data[10], (double)2) + System.Math.Pow((double)data[11], (double)2));
        data[27] = (float)Math.Sqrt(System.Math.Pow((double)data[12], (double)2) + System.Math.Pow((double)data[13], (double)2) + System.Math.Pow((double)data[14], (double)2));
        data[28] = (float)Math.Sqrt(System.Math.Pow((double)data[15], (double)2) + System.Math.Pow((double)data[16], (double)2) + System.Math.Pow((double)data[17], (double)2));
        return data;
    }

    //csvにデータを書き込む
    public static void Write_csv(float[] data)
    {
        try
        {
            //受け取ったfloat型配列をstring型に変換
            string[] data_csv = new string[29];
            for (int i = 0; i <= 28; ++i)
            {
                data_csv[i] = data[i].ToString();
            }

            //appendをtrueにすると、既存のファイルに追記
            //appendをfalseにすると、新規ファイルを作成
            var append = true;
            //出力用のファイルを開く
            Debug.Log("書き込み");
            string filePath = string.Format(@"./Assets/Data/data{0}.csv", file_num);
            StreamWriter sw = new StreamWriter(filePath, append, Encoding.GetEncoding("Shift_JIS"));
            //print("%s", % i)
            //ヘッダー出力
            string[] s1 = data_csv;
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
            sw.Close();
        //}
        }
        catch (System.Exception){
            Debug.Log("csv書き込みエラー");
        }
    }

    //２個目以降のcsvファイルの作成
    public static void Create_csv(){
        //csvファイル名の作成
        string filePath = string.Format(@"./Assets/Data/data{0}.csv", file_num);
        StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("Shift_JIS"));
        //ヘッダー出力
        string[] s1 = { "R_hand : x", "R_hand : y", "R_hand : z", 
                        "R_thumb : x", "R_thumb : y", "R_thumb : z", 
                        "R_index : x", "R_index : y", "R_index : zx", 
                        "R_middle : x", "R_middle : y", "R_middle : z",
                        "R_ring : x", "R_ring : y", "R_ring : z", 
                        "R_pinky : x", "R_pinky : y", "R_pinky : z", 
                        "R_hand_vec : x", "R_hand_vec : y", "R_hand_vec : z",
                        "R_fin_to_fin :x", "R_fin_to_fin :y", "R_fin_to_fin :z",
                        "R_dis_thumb", "R_dis_index", "R_dis_middle", "R_dis_ring", "R_dis_pinky"};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        sw.Close();
    }
}
