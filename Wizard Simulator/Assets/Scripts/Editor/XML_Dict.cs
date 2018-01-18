using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;

[ExecuteInEditMode]
public class XML_Dict : EditorWindow {

	string path;
	string input = " ";
	string spellword_button = "New Spellword (Don't fucking touch this right now)";
	string spellshape_button = "New Spellshape";

	private void OnEnable()
	{
		path = Application.dataPath + "/Grammar/grammar_test.xml";
	}

	[MenuItem("Tools/Spellwords")]
	public static void ShowWindow()
	{
		EditorWindow window = GetWindow<XML_Dict>("Spellword Window");
		window.Show();
	}

	void OnGUI()
	{
		GUILayout.Label("Enter New Spellword");

		//garbage collection is nice, this will make sure everything is properly capitalize. Its messy but eh.
		input = EditorGUILayout.TextField("Spell Word: ", input);
		/*char[] a = input.ToCharArray();
		/a[0] = char.ToUpper(a[0]);
		input = a.ToString(); */ 
		path = EditorGUILayout.TextField("XML Path: ", path);
		if (GUILayout.Button(spellshape_button))
		{
			StreamWriter writer = new StreamWriter(File.Create(Application.dataPath + "/Scripts/Magic/Spells/" + input + "Shape.cs"));
			writer.Write("using UnityEngine;\nusing System.Collections;\nusing System;\n");
			writer.Write("\npublic class " + input + "Shape : SpellShape\n{\n");
			writer.Write("\n\t\t\tpublic override void CastSpell()\n{\n}\n");
			writer.Write("\n\t\t\tpublic override void ShapeStart()\n{\n}\n");
			writer.Write("\n\t\t\tpublic override void ShapeUpdate()\n{\n}\n");
			writer.Write("}\n");
			writer.Dispose();
			//add new word to the dictionary
			NewWord();
		}
		this.Repaint();
	}

	void NewWord()
	{
		string[] text = File.ReadAllLines(path);
		List<string> list = new List<string>();
		list.AddRange(text);
		list.Insert(list.Count - 6, "\t\t <item>" + input.ToLower() + "</item>");
		File.WriteAllLines(path, list.ToArray());

		AssetDatabase.ImportAsset(path);
	}
}
