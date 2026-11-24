using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Unity.VisualScripting;
using System;

[CreateAssetMenu(fileName = "MagicBag", menuName = "Scriptable Objects/MagicBag")]
public class MagicBag : ScriptableObject
{
	public List<float> numberValues;

	public List<string> stringValues;

	public TextAsset propertiesFile;

	[SerializeField] private int column;

	[SerializeField] private bool isStringValues;

	private void OnValidate()
	{
		stringValues.Clear();
		numberValues.Clear();

		//StreamReader reader = new StreamReader(propertiesFile.text);
		int lineNumber = 0;

		//string data = reader.ReadLine();
		string data = propertiesFile.text;

		var lines = data.Split('\n');
		foreach (var line in lines)
		{
            var values = line.Split(',');

            if (lineNumber == 0)
            {
                lineNumber++;
            }
            else if (lineNumber == 1)
            {
                if (values[column] == "Strings") { isStringValues = true; }
                else { isStringValues = false; }
                lineNumber++;
            }
            else
            {
                if (isStringValues)
                {
                    stringValues.Add(values[column]);
                }
                else
                {
                    numberValues.Add(float.Parse(values[column]));
                }
            }

        }
	}
}
