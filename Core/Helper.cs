using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public static class Helper
    {

        public static void ValidateIPInput(string ip)
        {
            List<string> addressParts = new List<string>();
            addressParts.AddRange(ip.Split(".", 4));
            foreach (string addressPart in addressParts)
            {
                if(string.IsNullOrEmpty(addressPart))
                {
                    throw new Exception();
                }
                int number = int.Parse(addressPart);
                if (number < 0 || number > 255)
                {
                    throw new Exception();
                }
            }
        }
    }
}
