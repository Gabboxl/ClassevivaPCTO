using System.Text.RegularExpressions;

namespace ClassevivaPCTO.Utils
{
    public class CvUtils
    {
        public string GetCode(string userId)
        {
            return Regex.Replace(userId, @"[A-Za-z]+", "");

        }


    }

}
