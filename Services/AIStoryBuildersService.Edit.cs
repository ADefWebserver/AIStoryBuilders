﻿using AIStoryBuilders.Models;
using AIStoryBuilders.Models.JSON;
using Newtonsoft.Json;
using System.Text.Json;

namespace AIStoryBuilders.Services
{
    public partial class AIStoryBuildersService
    {
        #region public void RestructureParagraphs(Chapter objChapter, int ParagraphNumber, RestructureType RestructureType)
        public void RestructureParagraphs(Chapter objChapter, int ParagraphNumber, RestructureType RestructureType)
        {
            try
            {
                string OldParagraphPath = "";
                string NewParagraphPath = "";
                var ChapterNameParts = objChapter.ChapterName.Split(' ');
                string ChapterName = ChapterNameParts[0] + ChapterNameParts[1];
                var AIStoryBuildersParagraphsPath = $"{BasePath}/{objChapter.Story.Title}/Chapters/{ChapterName}";
                int CountOfParagraphs = CountParagraphs(objChapter);

                // Loop through all remaining paragraphs and rename them
                if (RestructureType == RestructureType.Add)
                {
                    for (int i = CountOfParagraphs; ParagraphNumber <= i; i--)
                    {
                        OldParagraphPath = $"{AIStoryBuildersParagraphsPath}/Paragraph{i}.txt";
                        NewParagraphPath = $"{AIStoryBuildersParagraphsPath}/Paragraph{i + 1}.txt";

                        // Rename file
                        System.IO.File.Move(OldParagraphPath, NewParagraphPath);
                    }
                }
                else if (RestructureType == RestructureType.Delete)
                {
                    for (int i = ParagraphNumber; i <= CountOfParagraphs; i++)
                    {
                        OldParagraphPath = $"{AIStoryBuildersParagraphsPath}/Paragraph{i + 1}.txt";
                        NewParagraphPath = $"{AIStoryBuildersParagraphsPath}/Paragraph{i}.txt";

                        // Rename file
                        System.IO.File.Move(OldParagraphPath, NewParagraphPath);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                LogService.WriteToLog("RestructureParagraphs: " + ex.Message + " " + ex.StackTrace ?? "" + " " + ex.InnerException.StackTrace ?? "");
            }
        }
        #endregion

        #region public void RestructureChapters(int ChapterNumber, RestructureType RestructureType)
        public void RestructureChapters(int ChapterNumber, RestructureType RestructureType)
        {
            try
            {
                if (RestructureType == RestructureType.Add)
                {

                }
                else if (RestructureType == RestructureType.Delete)
                {

                }
            }
            catch (Exception ex)
            {
                // Log error
                LogService.WriteToLog("RestructureChapters: " + ex.Message + " " + ex.StackTrace ?? "" + " " + ex.InnerException.StackTrace ?? "");
            }
        }
        #endregion
    }
}