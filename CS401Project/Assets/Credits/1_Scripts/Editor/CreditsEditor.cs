#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class CreditsEditor : EditorWindow
{
	// Current file and its content
	private string currentFile = "";
	private string currentFilename = "";
	private List<CreditLine> lines = new List<CreditLine>();

	// Static GUI
	private static Texture2D iconNew = null;
	private static Texture2D iconOpen = null;
	private static Texture2D iconSave = null;
	private static Texture2D iconAdd = null;
	private static Texture2D iconUp = null;
	private static Texture2D iconDown = null;
	private static Texture2D iconDelete = null;
	private Rect iconNewRect = new Rect(5, 5, 30, 30);
	private Rect iconOpenRect = new Rect(35, 5, 30, 30);
	private Rect iconSaveRect = new Rect(70, 5, 30, 30);
	private Rect iconAddRect = new Rect(100, 5, 30, 30);
	private static GUIStyle stylecenter = null;
	private static GUIStyle styleright = null;
	private static GUIStyle buttonLeft;
	private static GUIStyle buttonRight;
	private static bool init = false;

	// GUI Stuff
	private Vector2 scrollPos;

	// Menu
	[MenuItem("Window/Credits Editor")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(CreditsEditor), false, "Credits Editor");
	}

	/// <summary>
	/// Initializes the GUI.
	/// </summary>
	private void initGUI()
	{
		// Init for textures
		string[] dirs = Directory.GetDirectories("Assets", "CreditsEditorImages", SearchOption.AllDirectories);
		string iconsAssetsPath = "";
		if(dirs.Length > 0)
			iconsAssetsPath = dirs[0];
		else
			Debug.LogWarning("[Credits Editor] Could not found GUI textures for the editor window.");
		
		// Buttons icons
		iconNew = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsAssetsPath+"/new.png");
		iconOpen = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsAssetsPath+"/open.png");
		iconSave = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsAssetsPath+"/save.png");
		iconAdd = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsAssetsPath+"/add.png");
		iconUp = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsAssetsPath+"/up.png");
		iconDown = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsAssetsPath+"/down.png");
		iconDelete = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsAssetsPath+"/delete.png");
		
		// Buttons styles
		buttonLeft = GUI.skin.FindStyle(GUI.skin.button.name + "left");
		buttonRight = GUI.skin.FindStyle(GUI.skin.button.name + "right");
		
		// Styles
		stylecenter = new GUIStyle(EditorStyles.textField);
		stylecenter.alignment = TextAnchor.MiddleCenter;
		styleright = new GUIStyle(EditorStyles.textField);
		styleright.alignment = TextAnchor.MiddleRight;
		
		// Init
		init = true;
	}

	// The GUI
	void OnGUI()
	{
		if(!init) initGUI();

		// Super silly: Create an empty textfield to give focus when we will move lines.
		GUI.SetNextControlName("sillydummytextfield");
		EditorGUI.TextField(new Rect(-10, -10, 0, 0), "");

		// Top
		if(GUI.Button(iconNewRect, iconNew, buttonLeft))
		{
			EditorGUI.FocusTextInControl("sillydummytextfield");
			newFile();
		}
		if(GUI.Button(iconOpenRect, iconOpen, buttonRight))
		{
			EditorGUI.FocusTextInControl("sillydummytextfield");
			openFile();
		}
		if(currentFile != "")
		{
			if(GUI.Button(iconSaveRect, iconSave, buttonLeft))
			{
				EditorGUI.FocusTextInControl("sillydummytextfield");
				saveFile();
			}
			if(GUI.Button(iconAddRect, iconAdd, buttonRight))
			{
				EditorGUI.FocusTextInControl("sillydummytextfield");
				addLine();
			}
		}

		// Central
		if(currentFile != "")
		{
			// Display current line
			GUI.Label(new Rect(165, 12, 200, 20), "File: "+currentFilename);

			// Lines
			scrollPos = GUI.BeginScrollView(new Rect(0, 45, position.width, position.height-45), scrollPos, new Rect(0, 0, position.width-15, 10+(25*lines.Count)));
			for(int i = 0; i < lines.Count; i++)
			{
				lines[i].type = (CreditType)EditorGUI.EnumPopup(new Rect(10, 10+(i*25), 110, 17), "", lines[i].type);

				if(lines[i].type == CreditType.TwoColumns)
				{
					lines[i].data = EditorGUI.TextField(new Rect(130, 10+(i*25), (position.width-298)/2, 17), lines[i].data, styleright);
					lines[i].data2 = EditorGUI.TextField(new Rect(130+((position.width-298)/2)+10, 10+(i*25), (position.width-298)/2, 17), lines[i].data2);
				}
				else if(lines[i].type == CreditType.EmptySpace)
				{
					int v = 1;
					try{v = int.Parse(lines[i].data);}catch{}
					lines[i].data = ""+EditorGUI.IntField(new Rect(130, 10+(i*25), position.width-288, 17), "Empty lines:", v);
				}
				else
				{
					lines[i].data = EditorGUI.TextField(new Rect(130, 10+(i*25), position.width-288, 17), lines[i].data, stylecenter);
				}

				if(i > 0 && GUI.Button(new Rect(position.width-148, 8+(i*25), 27, 21), iconUp))
				{
					EditorGUI.FocusTextInControl("sillydummytextfield");
					CreditLine line = lines[i];
					lines.RemoveAt(i);
					lines.Insert(i-1, line);
					return;
				}
				if(i < lines.Count-1)
				{
					if(GUI.Button(new Rect(position.width-116, 8+(i*25), 27, 21), iconDown))
					{
						EditorGUI.FocusTextInControl("sillydummytextfield");
						CreditLine line = lines[i];
						lines.RemoveAt(i);
						lines.Insert(i+1, line);
						return;
					}
				}
				if(GUI.Button(new Rect(position.width-84, 8+(i*25), 27, 21), iconAdd))
				{
					EditorGUI.FocusTextInControl("sillydummytextfield");
					lines.Insert(i, new CreditLine(CreditType.TwoColumns, "", ""));
					return;
				}
				if(GUI.Button(new Rect(position.width-52, 8+(i*25), 27, 21), iconDelete))
				{
					EditorGUI.FocusTextInControl("sillydummytextfield");
					lines.RemoveAt(i); return;
				}
			}
			GUI.EndScrollView();
		}
	}

	void newFile()
	{
		string file = EditorUtility.SaveFilePanel("Save Credits file", EditorPrefs.GetString("credits_folder", ""), "credits.xml", "xml");
		if(file != "")
		{
			XmlDocument doc = new XmlDocument();
			XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			doc.AppendChild(docNode);
			XmlNode creditsNode = doc.CreateElement("credits");
			doc.AppendChild(creditsNode);
			doc.Save(file);
			openFile(file);
		}
	}

	void openFile(string filename = "")
	{
		// Lazy try-catching :(
		try
		{
			if(filename == "")
				filename =  EditorUtility.OpenFilePanel("Open Credits file", EditorPrefs.GetString("credits_folder", ""), "xml");
			if(filename != "")
			{
				// Clear precedent info
				lines.Clear();
				currentFile = filename;
				currentFilename = Path.GetFileName(currentFile);
				
				// Load XML
				string content = File.ReadAllText(filename);
				XmlDocument root = new XmlDocument();
				root.LoadXml(content);
				
				// Read credits
				foreach(XmlNode node in root.SelectNodes("credits/credit"))
				{
					CreditType type = CreditLine.textToType(node.Attributes.GetNamedItem("type").Value);
					string data = node.Attributes.GetNamedItem("data").Value;
					string data2 = node.Attributes.GetNamedItem("data2").Value;
					lines.Add(new CreditLine(type, data, data2));
				}

				// Save the directory
				EditorPrefs.SetString("credits_folder", Path.GetDirectoryName(filename));
			}
		}
		catch
		{
			lines.Clear();
			currentFile = "";
			this.ShowNotification(new GUIContent("Incorrect file."));
		}
	}

	void saveFile()
	{
		// Lazy try-catching :(
		try
		{
			XmlDocument doc = new XmlDocument();
			XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			doc.AppendChild(docNode);
			XmlNode creditsNode = doc.CreateElement("credits");
			doc.AppendChild(creditsNode);

			XmlNode creditNode;
			XmlAttribute attrType;
			XmlAttribute attrData;
			XmlAttribute attrData2;
			for(int i = 0; i < lines.Count; i++)
			{
				creditNode = doc.CreateElement("credit");
				attrType = doc.CreateAttribute("type");
				attrType.Value = CreditLine.typeToText(lines[i].type);
				attrData = doc.CreateAttribute("data");
				attrData.Value = lines[i].data;
				attrData2 = doc.CreateAttribute("data2");
				attrData2.Value = lines[i].data2;
				creditNode.Attributes.Append(attrType);
				creditNode.Attributes.Append(attrData);
				creditNode.Attributes.Append(attrData2);
				creditsNode.AppendChild(creditNode);
			}

			doc.Save(currentFile);
			AssetDatabase.Refresh();
			this.ShowNotification(new GUIContent("Credits saved."));

			// Save the directory
			EditorPrefs.SetString("credits_folder", Path.GetDirectoryName(currentFile));
		}
		catch
		{
			this.ShowNotification(new GUIContent("Error while saving."));
		}

	}

	void addLine()
	{
		lines.Add(new CreditLine(CreditType.TwoColumns, "", ""));
	}
}
#endif