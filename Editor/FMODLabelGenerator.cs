#if UNITY_EDITOR && FMOD

using FMOD.Studio;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FMODLabelGenerator
{
    private const string LastBankPathKey = "FMOD_LastBankPath";

    [MenuItem("FMOD/Generate FMOD Labels Class")]
    public static void GenerateLabelsClass()
    {
        // Alege folderul cu fișiere .bank
        string bankPath = EditorUtility.OpenFolderPanel("Select FMOD Bank Folder", PlayerPrefs.GetString(LastBankPathKey, Application.dataPath), "");
        if (string.IsNullOrEmpty(bankPath))
        {
            Debug.LogWarning("No folder selected. Operation cancelled.");
            return;
        }

        PlayerPrefs.SetString(LastBankPathKey, bankPath); // Salvează calea pentru viitor
        string outputPath = "Assets/Scripts/_FMOD/FMODParameters.cs";

        // Inițializează un nou FMOD Studio System
        FMOD.RESULT result = FMOD.Studio.System.create(out FMOD.Studio.System studioSystem);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError($"FMOD Studio System creation failed: {result}");
            return;
        }

        // Încarcă băncile
        studioSystem.initialize(1024, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, System.IntPtr.Zero);
        DirectoryInfo bankDir = new DirectoryInfo(bankPath);
        foreach (var file in bankDir.GetFiles("*.bank"))
        {
            studioSystem.loadBankFile(file.FullName, LOAD_BANK_FLAGS.NORMAL, out Bank bank);
        }

        // Extrage lista de evenimente și parametri
        HashSet<string> existingParameters = new HashSet<string>();
        List<string> classContent = new List<string> { "public static class FMODParameters\n{" };

        studioSystem.getBankList(out Bank[] banks);
        foreach (var bank in banks)
        {
            bank.getEventList(out EventDescription[] eventDescriptions);

            foreach (var eventDescription in eventDescriptions)
            {
                eventDescription.getPath(out string eventPath);
                eventDescription.getParameterDescriptionCount(out int parameterCount);

                for (int i = 0; i < parameterCount; i++)
                {
                    eventDescription.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION parameterDesc);

                    // Verifică dacă parametrul este duplicat
                    if (!existingParameters.Add(parameterDesc.name))
                        continue;

                    // Adaugă documentația pentru parametrii de tip float
                    if (parameterDesc.type == PARAMETER_TYPE.GAME_CONTROLLED)
                    {
                        string summaryComment = $"\t/// <summary>\n\t/// Min: {parameterDesc.minimum}, Max: {parameterDesc.maximum}\n\t/// </summary>";
                        classContent.Add(summaryComment);
                        classContent.Add($"\tpublic const string {CleanName(parameterDesc.name)} = \"{CleanName(parameterDesc.name)}\";");
                    }

                    // Adaugă parametrii cu label-uri
                    bool hasLabels = false;
                    List<string> labels = new List<string>();

                    for (int labelIndex = 0; labelIndex < parameterDesc.maximum + 1; labelIndex++)
                    {
                        FMOD.RESULT labelResult = eventDescription.getParameterLabelByIndex(i, labelIndex, out string label);
                        if (labelResult == FMOD.RESULT.OK)
                        {
                            hasLabels = true;
                            labels.Add(label);
                        }
                    }

                    if (hasLabels)
                    {
                        string className = $"{CleanName(parameterDesc.name)}_All_Labels";
                        classContent.Add($"\tpublic static class {className}\n\t{{");
                        foreach (var label in labels)
                        {
                            classContent.Add($"\t\tpublic const string {CleanName(label)} = \"{label}\";");
                        }
                        classContent.Add("\t}");
                    }
                }
            }
        }

        classContent.Add("}");

        // Scrie conținutul într-un fișier .cs
        File.WriteAllText(outputPath, string.Join("\n", classContent));
        Debug.Log($"FMOD Labels class generated at: {outputPath}");

        // Eliberează sistemul FMOD
        studioSystem.release();

        // Refresh Asset Database
        AssetDatabase.Refresh();
    }

    // Helper pentru curățarea denumirilor
    private static string CleanName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name.Replace(" ", "_");
    }
}

#endif
