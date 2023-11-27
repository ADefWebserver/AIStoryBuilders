﻿namespace AIStoryBuilders.Models.JSON
{
    public class JSONMasterStory
    {
        public string StoryTitle { get; set; }
        public string StorySynopsis { get; set; }
        public List<Character> CharacterList { get; set; }
        public List<Paragraphs> PreviousParagraphs { get; set; }
        public List<Paragraphs> RelatedParagraphs { get; set; }
        public Locations CurrentLocation { get; set; }
        public JSONChapter CurrentChapter { get; set; }
        public Paragraphs CurrentParagraph { get; set; }
    }
}
