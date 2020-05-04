using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using ASCIIArtLibrary;

namespace ASCIIArtContentPipelineExtension
{
    [ContentProcessor(DisplayName = "ASCIIArt Processor")]
    class ASCIIArtProcessor : ContentProcessor<System.String, ASCIIArtLibrary.ASCIIArt>
    {
        public override ASCIIArt Process(string input, ContentProcessorContext context)
        {
            return new ASCIIArtLibrary.ASCIIArt(input);
        }
    }
}
