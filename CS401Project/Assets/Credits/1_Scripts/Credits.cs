using UnityEngine;
#if NETFX_CORE
using XmlReader = WinRTLegacy.Xml.XmlReader;
#else
using XmlReader = System.Xml.XmlReader;
using System.IO;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Credit line type.
/// </summary>
public enum CreditType
{
    Title,
    SingleColumn,
    TwoColumns,
    Image,
    EmptySpace
}

/// <summary>
/// Represents a single line of the credits
/// </summary>
public class CreditLine
{
    public CreditType type;
    public string data;
    public string data2;

    // Constructor
    public CreditLine(CreditType type, string data, string data2)
    {
        this.type = type; this.data = data; this.data2 = data2;
    }

    // Utils functions for CreditType
    public static string typeToText(CreditType t)
    {
        switch (t)
        {
            case CreditType.Title: return "title";
            case CreditType.SingleColumn: return "singlecolumn";
            case CreditType.Image: return "texture";
            case CreditType.EmptySpace: return "space";
            default: return "twocolumns";
        }
    }
    public static CreditType textToType(string t)
    {
        if (t == "title") return CreditType.Title;
        if (t == "singlecolumn") return CreditType.SingleColumn;
        if (t == "texture") return CreditType.Image;
        if (t == "space") return CreditType.EmptySpace;
        return CreditType.TwoColumns;
    }
}

/// <summary>
/// Main credits class. Get the instance with Credits.getInstance().
/// </summary>
public class Credits : MonoBehaviour
{
    // Settings
    public TextAsset creditsFile;
    public bool playOnAwake = true;
    public int speed = 100;
    public float fadeTime = 1f;

    // Prefabs
    public GameObject prefabSingleColumn;
    public GameObject prefabTwoColumns;
    public GameObject prefabImage;
    public GameObject prefabTitle;
    public GameObject prefabEmptySpace;

    // Private
    private bool started = false;
    private List<CreditLine> lines = new List<CreditLine>();
    private int height = 0;
    private int count = 0;

    // Signals
    public event CreditsEndListener endListeners;
    public delegate void CreditsEndListener(Credits c);

    // Singleton
    private static Credits _instance;
    public static Credits getInstance() { return _instance; }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        if (playOnAwake)
            beginCredits();
    }

    public void showCredits()
    {
        started = false;
        beginCredits();
    }

    public void beginCredits()
    {
        if (!started)
        {
            // Sanitize input
            speed = Mathf.Clamp(speed, 50, 1000);

            // Calculate margin
            height = (int)GetComponent<RectTransform>().rect.height;

            // Load from XML and calculate position
            XmlReader reader = XmlReader.Create(new StringReader(creditsFile.text));
            while (reader.ReadToFollowing("credit"))
            {
                CreditType type = CreditType.EmptySpace;
                string data = "";
                string data2 = "";

                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == "type") type = CreditLine.textToType(reader.Value);
                    else if (reader.Name == "data") data = reader.Value;
                    else if (reader.Name == "data2") data2 = reader.Value;
                }

                if (type == CreditType.EmptySpace)
                {
                    int n = Mathf.Clamp(int.Parse(data), 1, 100);
                    for (int i = 0; i < n; ++i)
                        lines.Add(new CreditLine(CreditType.EmptySpace, "", ""));
                }
                else
                    lines.Add(new CreditLine(type, data, data2));
            }

            count = lines.Count;

            // Start
            started = true;
            spawnNext();
        }
    }

    public void spawnNext()
    {
        if (lines.Count > 0)
        {
            CreditLine line = lines[0];
            lines.RemoveAt(0);// Done here because this will remove recursive bugs that doesn't exists.

            if (line.type == CreditType.EmptySpace)
            {
                GameObject g = Instantiate(prefabEmptySpace);
                g.GetComponent<RectTransform>().SetParent(transform, false);
                g.GetComponent<CreditLineInstanceSpace>().go(line.data, line.data2, height, speed, fadeTime);
            }
            else if (line.type == CreditType.Title)
            {
                GameObject g = Instantiate(prefabTitle);
                g.GetComponent<RectTransform>().SetParent(transform, false);
                g.GetComponent<CreditLineInstanceTitle>().go(line.data, line.data2, height, speed, fadeTime);
            }
            else if (line.type == CreditType.SingleColumn)
            {
                GameObject g = Instantiate(prefabSingleColumn);
                g.GetComponent<RectTransform>().SetParent(transform, false);
                g.GetComponent<CreditLineInstanceSingleColumn>().go(line.data, line.data2, height, speed, fadeTime);
            }
            else if (line.type == CreditType.TwoColumns)
            {
                GameObject g = Instantiate(prefabTwoColumns);
                g.GetComponent<RectTransform>().SetParent(transform, false);
                g.GetComponent<CreditLineInstanceTwoColumns>().go(line.data, line.data2, height, speed, fadeTime);
            }
            else if (line.type == CreditType.Image)
            {
                GameObject g = Instantiate(prefabImage);
                g.GetComponent<RectTransform>().SetParent(transform, false);
                g.GetComponent<CreditLineInstanceImage>().go(line.data, line.data2, height, speed, fadeTime);
            }
        }
    }

    public void callbackDeleted()
    {
        count--;
        if (count <= 0 && endListeners != null)
            endListeners(this);
    }
}
