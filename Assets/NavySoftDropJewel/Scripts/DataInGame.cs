using System;
using UnityEngine;

public static class DataInGame
{
	public static int BestScore8x8;

    public static int BestScore10x10;

    public static int CurrentScore;


	public static float IsSoundSave;

    public static float IsMusicSave;



	public static void  GetData()
	{
        IsSoundSave = 1f;
        IsMusicSave = 1f;
        BestScore8x8 = PlayerPrefs.GetInt("BestScore8x8", 0);
        BestScore10x10 = PlayerPrefs.GetInt("BestScore10x10", 0);
		IsSoundSave = PlayerPrefs.GetFloat("IsSoundSave", 1f);
        IsMusicSave = PlayerPrefs.GetFloat("IsMusicSave", 1f);
    }
}
