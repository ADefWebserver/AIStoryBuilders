﻿using AIStoryBuilders.Models;
using AIStoryBuilders.Models.JSON;
using Newtonsoft.Json;
using OpenAI.Files;
using System.IO.Compression;
using System.Text.Json;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace AIStoryBuilders.Services
{
    public partial class AIStoryBuildersService
    {
        #region public byte[] ExportWordDocument(Story objStory)
        public byte[] ExportWordDocument(Story objStory)
        {
            try
            {
                string TempPath =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/AIStoryBuilders/_Temp";

                // Create the directory if it doesn't exist
                if (!Directory.Exists(TempPath))
                {
                    Directory.CreateDirectory(TempPath);
                }

                string WordFileName = $"{objStory.Title}.docx";
                string WordFilePath = $"{TempPath}/{WordFileName}";

                using (DocX document = DocX.Create(WordFilePath))
                {
                    // Create Cover Page
                    document.InsertParagraph(objStory.Title).FontSize(25d).SpacingAfter(50d).Alignment = Alignment.center;

                    var colChapters = GetChapters(objStory);

                    foreach (var objChapter in colChapters)
                    {
                        document.InsertSectionPageBreak();

                        // Create Chapter Title
                        document.InsertParagraph(objChapter.ChapterName).FontSize(20d).SpacingAfter(25d).Alignment = Alignment.center;

                        var colParagraphs = GetParagraphs(objChapter);

                        foreach (var objParagraph in colParagraphs)
                        {
                            // Break up objParagraph.ParagraphContent by \n
                            string[] sections = objParagraph.ParagraphContent.Split('\n');

                            // Create a new paragraph for each line
                            foreach (string section in sections)
                            {
                                var p = document.InsertParagraph(section);
                                p.IndentationFirstLine = 28f;
                            }
                        }
                    }

                    // Save the document.
                    document.Save();
                }

                // Read the document and convert to base64
                byte[] WordFileBytes = File.ReadAllBytes(WordFilePath);

                // Delete the temp directory
                Directory.Delete(TempPath, true);

                return WordFileBytes;
            }
            catch (Exception ex)
            {
                // Log error
                LogService.WriteToLog("ExportWordDocument: " + ex.Message + " " + ex.StackTrace ?? "" + " " + ex.InnerException.StackTrace ?? "");

                return null;
            }
        }
        #endregion

        #region public byte[] ExportProject(Story objStory)
        public byte[] ExportProject(Story objStory)
        {
            try
            {
                #region Create Temp Directories

                // Create _Temp
                string TempPath =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/AIStoryBuilders/_Temp";

                if (!Directory.Exists(TempPath))
                {
                    Directory.CreateDirectory(TempPath);
                }
                else
                {
                    // Delete the temp directory
                    Directory.Delete(TempPath, true);

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(TempPath))
                    {
                        Directory.CreateDirectory(TempPath);
                    }
                }

                // Create _TempZip
                string TempZipPath =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/AIStoryBuilders/_TempZip";

                if (!Directory.Exists(TempZipPath))
                {
                    Directory.CreateDirectory(TempZipPath);
                }
                else
                {
                    // Delete the temp directory
                    Directory.Delete(TempZipPath, true);

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(TempZipPath))
                    {
                        Directory.CreateDirectory(TempZipPath);
                    }
                }
                #endregion

                // Create the manifest file
                string ManifestFilePath = Path.Combine(TempPath, "Manifest.config");

                // Create JSON from objStory
                JSONManifest objJSONManifest = new JSONManifest
                {
                    Version = _appMetadata.Version,
                    Title = objStory.Title,
                    Style = objStory.Style,
                    Theme = objStory.Theme,
                    Synopsis = objStory.Synopsis,
                    ExportedDate = DateTime.Now.ToString()
                };

                string JSONManifest = JsonConvert.SerializeObject(objJSONManifest);

                using (var streamWriter = new StreamWriter(ManifestFilePath))
                {
                    streamWriter.WriteLine(JSONManifest);
                }

                // Get the Story
                var StoriesPath = $"{BasePath}/{objStory.Title}";
                string ExportFileName = $"{objStory.Title}.stybld";
                string ExportFilePath = $"{TempZipPath}/{ExportFileName}";

                // Copy the story folder to the temp directory
                CopyDirectory(StoriesPath, TempPath);

                // Zip the files
                ZipFile.CreateFromDirectory(TempPath, ExportFilePath);

                // Read the Zip file into a byte array
                byte[] ExportFileBytes = File.ReadAllBytes(ExportFilePath);

                // Delete the temp directories
                Directory.Delete(TempPath, true);
                Directory.Delete(TempZipPath, true);

                return ExportFileBytes;
            }
            catch (Exception ex)
            {
                // Log error
                LogService.WriteToLog("ExportFile: " + ex.Message + " " + ex.StackTrace ?? "" + " " + ex.InnerException.StackTrace ?? "");

                return null;
            }
        }
        #endregion

        #region public string ImportProject(byte[] stybldFile)
        public string ImportProject(byte[] stybldFile)
        {
            string strResponse = string.Empty;

            try
            {
                #region Create Temp Directories

                // Create _Temp
                string TempPath =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/AIStoryBuilders/_Temp";

                if (!Directory.Exists(TempPath))
                {
                    Directory.CreateDirectory(TempPath);
                }
                else
                {
                    // Delete the temp directory
                    Directory.Delete(TempPath, true);

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(TempPath))
                    {
                        Directory.CreateDirectory(TempPath);
                    }
                }

                // Create _TempZip
                string TempZipPath =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/AIStoryBuilders/_TempZip";

                if (!Directory.Exists(TempZipPath))
                {
                    Directory.CreateDirectory(TempZipPath);
                }
                else
                {
                    // Delete the temp directory
                    Directory.Delete(TempZipPath, true);

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(TempZipPath))
                    {
                        Directory.CreateDirectory(TempZipPath);
                    }
                }
                #endregion

                // Save the file to the _TempZip directory
                string ImportFilePath = $"{TempZipPath}/Import.stybld";
                File.WriteAllBytes(ImportFilePath, stybldFile);

                // Extract the files to the _Temp directory
                ZipFile.ExtractToDirectory(ImportFilePath, TempPath);

                // Read the manifest file
                string ManifestFilePath = Path.Combine(TempPath, "Manifest.config");

                // Read the file
                string JSONManifest = File.ReadAllText(ManifestFilePath);

                // Convert to JSONManifest
                JSONManifest objJSONManifest = JsonConvert.DeserializeObject<JSONManifest>(JSONManifest);

                // Check the version
                if (ConvertToInteger(objJSONManifest.Version) > ConvertToInteger(_appMetadata.Version))
                {
                    strResponse = $"The version of the file you are trying to import is not compatible with this version of AI Story Builders. Please update AI Story Builders to the latest version and try again.";
                    return strResponse;
                }

                // Create the story folder
                string StoryPath = $"{BasePath}/{objJSONManifest.Title}";

                // Check if the story already exists
                if (Directory.Exists(StoryPath))
                {
                    strResponse = $"The story {objJSONManifest.Title} already exists. Backup and delete it before trying to import a new version.";
                    return strResponse;
                }

                // Create the directory if it doesn't exist (it shouldn't exist)
                if (!Directory.Exists(StoryPath))
                {
                    Directory.CreateDirectory(StoryPath);
                }

                // Copy the files from the _Temp directory to the story folder
                CopyDirectory(TempPath, StoryPath);

                // Delete the temp directories
                Directory.Delete(TempPath, true);
                Directory.Delete(TempZipPath, true);

                // Add Story to file
                var AIStoryBuildersStoriesPath = $"{BasePath}/AIStoryBuildersStories.csv";
                string[] AIStoryBuildersStoriesContent = ReadCSVFile(AIStoryBuildersStoriesPath);

                // Remove all empty lines
                AIStoryBuildersStoriesContent = AIStoryBuildersStoriesContent.Where(line => line.Trim() != "").ToArray();

                // Trim all lines
                AIStoryBuildersStoriesContent = AIStoryBuildersStoriesContent.Select(line => line.Trim()).ToArray();

                // Add Story to file
                string newStory = $"{AIStoryBuildersStoriesContent.Count() + 1}|{objJSONManifest.Title}|{objJSONManifest.Style}|{objJSONManifest.Theme}|{objJSONManifest.Synopsis}";
                AIStoryBuildersStoriesContent = AIStoryBuildersStoriesContent.Append(newStory).ToArray();
                File.WriteAllLines(AIStoryBuildersStoriesPath, AIStoryBuildersStoriesContent);

                // Log
                LogService.WriteToLog($"Story imported {objJSONManifest.Title}");

                strResponse = $"The project '{objJSONManifest.Title}' was successfully imported.";
            }
            catch (Exception ex)
            {
                // Log error
                LogService.WriteToLog("ImportProject: " + ex.Message + " " + ex.StackTrace ?? "" + " " + ex.InnerException.StackTrace ?? "");

                return ex.Message;
            }

            return strResponse;
        }
        #endregion

        // Utility

        #region public void CopyDirectory(string sourceDir, string targetDir)
        public void CopyDirectory(string sourceDir, string targetDir)
        {
            // Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceDir, targetDir));
            }

            // Copy all the files & Replaces any files with the same name
            foreach (string filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                string targetFilePath = filePath.Replace(sourceDir, targetDir);
                CopyFileToAppDataDirectory(filePath, targetFilePath);
            }
        }

        private void CopyFileToAppDataDirectory(string sourceFilePath, string targetFilePath)
        {
            // Check if the target file already exists, and if so, delete it to allow overwriting
            if (File.Exists(targetFilePath))
            {
                File.Delete(targetFilePath);
            }

            // Copy the file from the source to the target
            // The 'true' parameter allows the method to overwrite the file if it already exists
            File.Copy(sourceFilePath, targetFilePath, true);
        }
        #endregion

        #region private int ConvertToInteger(string strParamVersion)
        private int ConvertToInteger(string strParamVersion)
        {
            int intVersionNumber = 0;
            string strVersion = strParamVersion;

            // Split into parts seperated by periods
            char[] splitchar = { '.' };
            var strSegments = strVersion.Split(splitchar);

            // Process the segments
            int i = 0;
            List<int> colMultiplyers = new List<int> { 10000, 100, 1 };
            foreach (var strSegment in strSegments)
            {
                int intSegmentNumber = Convert.ToInt32(strSegment);
                intVersionNumber = intVersionNumber + (intSegmentNumber * colMultiplyers[i]);
                i++;
            }

            return intVersionNumber;
        }
        #endregion
    }
}
