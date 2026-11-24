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

    public FilePathAttribute propertiesFile;

    [SerializeField] private int column;

    [SerializeField] private bool isStringValues;

    private void OnValidate()
    {
        stringValues.Clear();
        numberValues.Clear();

        
        StreamReader reader = new StreamReader(propertiesFile.ToString());
        int lineNumber = 0;

        while (true)
        {
            string data = reader.ReadLine();
            if (data == null)
            {
                break;
            }

            var values = data.Split(',');

            if (lineNumber == 0)
            {
                lineNumber++;
                continue;
            }
            if (lineNumber == 1)
            {
                if (values[column] == "Strings") { isStringValues = true; }
                else { isStringValues = false; }
                lineNumber++;
                continue;
            }
            if (lineNumber > 1)
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
