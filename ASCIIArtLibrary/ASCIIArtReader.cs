using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIArtLibrary
{
    class ASCIIArtReader : ContentTypeReader<ASCIIArt>
    {
        protected override ASCIIArt Read(ContentReader input, ASCIIArt existingInstance)
        {
            return new ASCIIArt(input.ReadString());
        }
    }
}
