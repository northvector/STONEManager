using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stone_Manager.Classes
{
    internal class Utils
    {
        public static byte[] ConvertByteArray(params int[] param)
        {
            if (param == null || param.Length == 0)
            {
                return null;
            }
            byte[] payload = new byte[param.Length];
            for (int idx = 0; idx < param.Length; idx++)
            {
                payload[idx] = (byte)param[idx];
            }
            return payload;
        }

    }
}
