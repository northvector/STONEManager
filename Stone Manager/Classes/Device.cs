using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stone_Manager.Classes
{
    internal class Device
    {
        public static void SetVolume(int vol = 31)
        {
            Bluetooth.SendCommand(Bluetooth.VENDOR_PT, 513, Utils.ConvertByteArray(vol));
        }

    }
}
