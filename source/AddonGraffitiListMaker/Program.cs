using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraffitiArtist;
using System.IO;
using System.Xml.Serialization;

namespace AddonGraffitiListMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            string appTitle = "Grand Theft Auto V : Graffiti Mod by stillhere";

            Console.Title = appTitle;

            Console.WriteLine(appTitle + "\n");

            Console.WriteLine("This application will scan all the subdirectories in the 'scripts/Graffiti Mod/Decals' folder to easily generate a list of graffiti art that reside within certain *.ytd files inside your '/update/update.rpf/x64/textures/script_txds.rpf/' directory.");
            Console.WriteLine("The list will then be read by the script in-game. :)" + "\n");

            Console.WriteLine("INSTRUCTIONS:\n" + "The name of each folder must match the name of an existing *.ytd file inside your game's script_txds.rpf archive.\n");
            Console.WriteLine("Launch OpenIV and navigate to script_txds.rpf. Open any *.ytd file (ex: graffiti.ytd) by double-clicking the file.\n");
            Console.WriteLine("OPTIONAL: Add new graffiti to your game by clicking 'Import' and selecting your image. Remember to click 'Save' when you're done!\n");
            Console.WriteLine("At the bottom, click 'Export all textures' and then click 'Portable Network Graphics (*.png)'\n");
            Console.WriteLine("Next, select the folder within 'scripts/Graffiti Mod/Decals' that matches the *.ytd filename (ex: scripts/Graffiti Mod/Decals/graffiti)\n");
            Console.WriteLine("Create a folder if it doesn't exist.\n");
            Console.WriteLine("Your images will now be exported to that folder.\n\n");
            Console.WriteLine("Ready? Press any key to continue.\n");

            Console.ReadKey();

            Console.WriteLine("Generating list, please be patient..." + "\n");

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n");

            GenerateList();

            Console.ReadKey();
        }

        private static void GenerateList()
        {
            string saveDirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); // scripts/Graffiti Mod
            
            List<Graffiti> GraffitiArtList = new List<Graffiti>();
            List<TextureDictionary> TextureDictionaryList = new List<TextureDictionary>();
            List<string> textureDicNamePathList = Directory.GetDirectories(saveDirectoryPath + "\\Decals").ToList();

            int textureDictCount = textureDicNamePathList.Count;
            int artCount = 0;

            foreach (string path in textureDicNamePathList)
            {
                string folderName = path.Remove(0, path.LastIndexOf('\\') + 1);

                string[] filesWithPaths = Directory.GetFiles(@path, "*.png");
                foreach (string imageFilePath in filesWithPaths)
                {
                    Graffiti graf = new Graffiti();
                    graf.TextureDictionary = folderName;
                    graf.TextureName = Path.GetFileNameWithoutExtension(imageFilePath);

                    // https://stackoverflow.com/a/13073341
                    var buff = new byte[32];
                    using (var d = File.OpenRead(imageFilePath))
                    {
                        d.Read(buff, 0, 32);
                    }
                    const int wOff = 16;
                    const int hOff = 20;
                    graf.TextureWidth = BitConverter.ToInt32(new[] { buff[wOff + 3], buff[wOff + 2], buff[wOff + 1], buff[wOff + 0], }, 0);
                    graf.TextureHeight = BitConverter.ToInt32(new[] { buff[hOff + 3], buff[hOff + 2], buff[hOff + 1], buff[hOff + 0], }, 0);

                    GraffitiArtList.Add(graf);

                    artCount++;
                }
            }
            
            XMLHelper.SaveObjectToXML(GraffitiArtList, saveDirectoryPath + "\\" + "GraffitiTextureList.xml");

            Console.WriteLine("List saved to:" + "\n" + saveDirectoryPath);
            Console.WriteLine("\nYou have a total of " + artCount + " graffiti art spread across " + textureDictCount + " texture pack(s).");
            Console.WriteLine("\n" + "Now you can use your newly added graffiti in-game. Enjoy!");
            Console.WriteLine("\nPress any key to exit.");
        }
    }
    
    public class TextureDictionary
    {
        public string DictionaryName { get; set; }

        public List<Graffiti> GraffitiList { get; set; }

        public TextureDictionary(string dictName)
        {
            DictionaryName = dictName;
        }
    }
}
