using ASCIIArtLibrary;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIArtContentPipelineExtension
{
    [ContentTypeWriter]
    class ASCIIArtWriter : ContentTypeWriter<ASCIIArtLibrary.ASCIIArt>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "ASCIIArtLibrary.ASCIIArtReader, ASCIIArtLibrary";
        }

        protected override void Write(ContentWriter output, ASCIIArt value)
        {
            output.Write(value.Art);
        }
    }
}
