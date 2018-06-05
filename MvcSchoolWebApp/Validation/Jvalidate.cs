using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace FinalTemplate.source.Validation
{
    public class Jvalidate
    {
        //Checks for any word reserved by sql server.
        public static bool FilterBlackLIstKeywords(string[] AssumedBlackList)
        {
            //Getting physical path of the file in with all sql server keywords are stored.
            string path = HttpContext.Current.Server.MapPath("~") + @"source\Jvalidation\blacklist_keywords.txt";
            //creating file stream which will handle the file operations.     
            FileStream fstream = new FileStream(path, FileMode.Open, FileAccess.Read);
            //Stream reader will read the text from file opened by file streamer.
            StreamReader sreader = new StreamReader(fstream);
            //Counting number of iterations for checking every word present in black list.
            int lines = File.ReadAllLines(path).Length;
            //initialize the reader to reader from start of the file. The line below will start it for the first time.
            sreader.BaseStream.Seek(0, SeekOrigin.Begin);
            //current word read by stream reader will store in str variable for easy access to current word.
            string str = sreader.ReadLine();
            //This loop will check for every letter present in input string.
            //In every iteration it will jump to next word and loop through the black list to compare it with them all.
            for (int i = 0; i < AssumedBlackList.Length; i++)
            {
                //Current word from input string will be compared to every single word of black list.
                for (int j = 0; j < lines; j++)
                {   //If current word of input string matches to any of black listed keywords. It will returns false.
                    if (AssumedBlackList[i].ToUpper() == str)
                        return false;
                    //Present next word for comparision 
                    str = sreader.ReadLine();
                }
                //Again initialize the stream reader from beginning so next word from input string can be compared to every word in black list.
                sreader.BaseStream.Seek(0, SeekOrigin.Begin);
            }
            //Close the stream reader object
            sreader.Close();
            //Close the file streamer object
            fstream.Close();
            //Return true when all text is clear and trust wrothy.
            return true;
        }
        //It removes the html tags from any part of string
        public static string RemoveHtmlTags(string StringValue)
        {
            //Filters string and removes all html tags.
            return Regex.Replace(StringValue, "<.*?>", string.Empty);
        }

    }
}