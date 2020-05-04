using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TImport = System.String;

namespace ASCIIArtContentPipelineExtension
{

    [ContentImporter(".asciiart", DisplayName = "ASCIIArt Importer", DefaultProcessor = "ASCIIArtProcessor")]
    class ASCIIArtImporter : ContentImporter<TImport>
    {
        public override string Import(string filename, ContentImporterContext context)
        {
            return System.IO.File.ReadAllText(filename);
        }
    }
}
