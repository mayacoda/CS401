using UnityEngine;

public enum GAMES
{
    STACK,
    ZIG_ZAG,
    FLAPPY_BIRD
}

public class SaveLoad
{

    public static int get(GAMES game, string additionalParameter, bool isUpperBetter)
    {

        string dodatakChanger = additionalParameter == null ? "" : additionalParameter;

        if (PlayerPrefs.HasKey(game.ToString() + dodatakChanger))
            return PlayerPrefs.GetInt(game.ToString() + dodatakChanger);
        else if (isUpperBetter)
            return 0;
        else
            return int.MaxValue;

    }

    public static bool isNew(GAMES scene, string additionalParameter, int score, bool isUpperBetter)
    {
        return isUpperBetter ? get(scene, additionalParameter, isUpperBetter) < score : get(scene, additionalParameter, isUpperBetter) > score;

    }

    public static void set(GAMES scene, string additionalParameter, int score)
    {
        string dodatakChanger = additionalParameter == null ? "" : additionalParameter;
        PlayerPrefs.SetInt(scene.ToString() + dodatakChanger, score);
    }



}
