﻿
#nullable disable
using System;
using System.Collections.Generic;

namespace AIStoryBuilders.Models;

public partial class ParagraphLocation
{
    public int Id { get; set; }

    public int ParagraphId { get; set; }

    public int LocationId { get; set; }

    public virtual Location Location { get; set; }

    public virtual Paragraph Paragraph { get; set; }
}