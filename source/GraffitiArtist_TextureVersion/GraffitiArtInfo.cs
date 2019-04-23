using System.Xml.Serialization;

namespace GraffitiArtist
{
    public class Graffiti
    {
        public string TextureDictionary { get; set; }

        public string TextureName { get; set; }

        public int TextureWidth { get; set; }

        public int TextureHeight { get; set; }

        [XmlIgnore]
        public int DecalTypeIndex = -1;

        [XmlIgnore]
        public int LoadedCount = 0;

        public Graffiti() { }

        public Graffiti(string dictionary, string name, int width, int height)
        {
            TextureDictionary = dictionary;
            TextureName = name;
            TextureWidth = width;
            TextureHeight = height;
        }
    }
}
