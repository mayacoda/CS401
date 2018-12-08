using UnityEngine;
using UnityEditor;

public class DeletePlayerPrefsScript : EditorWindow
{
    [MenuItem("PERADLL/Delete PlayerPrefs (All)")]
    static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}