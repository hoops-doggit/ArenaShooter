using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public byte Id { get; set; }

    public static object Deserialize(byte[] data)
    {
        var result = new PlayerStats();
        result.Id = data[0];
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        var c = (PlayerStats)customType;
        return new byte[] { c.Id };
    }


    public int viewID;
    public string playerName;
    public int score = 0;
}
