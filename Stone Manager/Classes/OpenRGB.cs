using OpenRGB.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenRGB.NET.Models;
using DrawingColor = System.Drawing.Color;
using OpenRGBColor = OpenRGB.NET.Models.Color;
using OpenRGB.NET.Enums;

namespace Stone_Manager.Classes
{
    internal class OpenRGB
    {
        OpenRGBClient client = new OpenRGBClient();

        public bool Connect()
        {
            client.Connect();
            return client.Connected;
        }
        public DrawingColor GetFirstDeviceColor()
        {
            var devices = client.GetAllControllerData();
            foreach (var device in devices)
            {
                if (device.Type == DeviceType.Motherboard || device.Type == DeviceType.Gpu)
                return ConvertToDrawingColor(device.Colors.FirstOrDefault());

            }
            return DrawingColor.Black;
        }
        private DrawingColor ConvertToDrawingColor(OpenRGBColor openRgbColor)
        {
            return DrawingColor.FromArgb(openRgbColor.R, openRgbColor.G, openRgbColor.B);
        }

    }
}
