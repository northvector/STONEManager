using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stone_Manager.Classes
{
    internal class DeviceRGB
    {
        public static void TurnOnDeviceRGB(int color_R = 255, int color_G = 255, int color_B = 255, int brightness = 100)
        {
            Bluetooth.SendCommand(Bluetooth.VENDOR_PT, 514, Utils.ConvertByteArray(brightness));
            Bluetooth.SendCommand(Bluetooth.VENDOR_PT, 515, Utils.ConvertByteArray(1));
            Bluetooth.SendCommand(Bluetooth.VENDOR_PT, 530, Utils.ConvertByteArray(brightness, 1, color_R & 255, color_G & 255, color_B & 255));
        }
        public static void SetRGBColor(int color_R = 255, int color_G = 255, int color_B = 255, int brightness = 100)
        {
            Bluetooth.SendCommand(Bluetooth.VENDOR_PT, 514, Utils.ConvertByteArray(brightness));
            Bluetooth.SendCommand(Bluetooth.VENDOR_PT, 515, Utils.ConvertByteArray(1));
            Bluetooth.SendCommand(Bluetooth.VENDOR_PT, 516, Utils.ConvertByteArray(color_R & 255, color_G & 255, color_B & 255));
        }
        public static void SetMood(int mood = 2, int color_R = 255, int color_G = 255, int color_B = 255, int brightness = 100)
        {
            Bluetooth.SendCommand(Bluetooth.VENDOR_PT, 530, Utils.ConvertByteArray(brightness, mood, color_R & 255, color_G & 255, color_B & 255));
        }
        public static void TurnOFFRGB()
        {
            Bluetooth.SendCommand(Bluetooth.VENDOR_PT, 531);
        }
    }
}
