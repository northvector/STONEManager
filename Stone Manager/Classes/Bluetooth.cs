using InTheHand.Net.Sockets;
using InTheHand.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using InTheHand.Net.Bluetooth;

namespace Stone_Manager.Classes
{
    internal class Bluetooth
    {
        public enum MoodColorType
        {
            SOLID,
            CANDLE,
            AURORA,
            SEA_WAVE,
            FireFly
        }

        public const int VENDOR_PT = 0x5054;
        public const byte DEFAULT_FLAGS = 0;
        public static string VENDOR_ID = "2C:30:68";
        public static string VENDOR_ID_WS = "00:02:5B";
        public static string DEVICE_NAME = "STONE";
        public static string deviceAddress = "D0:95:C7:04:8C:78";
        public static string DeviceName { get; set; }

        public static BluetoothClient bluetoothClient = new BluetoothClient();
        public static BluetoothAddress DeviceAddress { get; set; }
        public static BluetoothDeviceInfo deviceInfo;
        public static Stream bluetoothStream; 

        public static void Connect()
        {
            if (string.IsNullOrEmpty(deviceAddress))
            {
                MessageBox.Show("Please enter a Bluetooth device address.");
                return;
            }
            Main.mainform.Invoke((MethodInvoker)(() =>
            {
                Main.mainform.label_status.Text = "Searching";
            }));
            Thread connectionThread = new Thread(() =>
            {
                try
                {
                    // Parse the device address
                    BluetoothAddress address = BluetoothAddress.Parse(deviceAddress);

                    // Create a BluetoothClient instance
                    bluetoothClient = new BluetoothClient();

                    // Discover devices (optional)
                    var devices = bluetoothClient.DiscoverDevices();

                    // Find the device by address
                    deviceInfo = devices.FirstOrDefault(d => d.DeviceAddress == address);

                    if (deviceInfo == null)
                    {
                        Main.mainform.Invoke((MethodInvoker)(() =>
                        {
                            MessageBox.Show("Device not found.");
                        }));
                        return;
                    }
                    Main.mainform.Invoke((MethodInvoker)(() =>
                    {
                        Main.mainform.label_status.Text = "Connecting";
                    }));
                    // Connect to the device using RFCOMM (Serial Port Profile)
                    bluetoothClient.Connect(deviceInfo.DeviceAddress, BluetoothService.RFCommProtocol);
                    bluetoothStream = bluetoothClient.GetStream();

                    // Update UI on successful connection
                    Main.mainform.Invoke((MethodInvoker)(() =>
                    {
                        Main.mainform.label_status.Text = "Connected";
                        Main.mainform.label_status.ForeColor = Color.Green;
                    }));
                    SendCommand(VENDOR_PT, 1);
                    SendCommand(VENDOR_PT, 16);
                    Random rand = new Random();
                    SendCommand(VENDOR_PT, 578, Utils.ConvertByteArray(rand.Next(1, 3)));

                }
                catch (Exception ex)
                {
                    Main.mainform.Invoke((MethodInvoker)(() =>
                    {
                        MessageBox.Show($"Error connecting to Bluetooth device: {ex.Message}");
                    }));
                }
            })
            {
                IsBackground = true
            };
            connectionThread.Start();
        }
        public static async Task<byte[]> ReadResponseAsync()
        {
            byte[] buffer = new byte[256];
            try
            {
                int bytesRead = await bluetoothStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    byte[] response = new byte[bytesRead];
                    Array.Copy(buffer, response, bytesRead);
                    return response;
                }
                else
                {
                    MessageBox.Show("No response from device.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while reading response: {ex.Message}");
                return null;
            }
        }

        public static async Task ReadResponseAsyncString()
        {
            byte[] buffer = new byte[256];
            try
            {
                int bytesRead = await bluetoothStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string response = BitConverter.ToString(buffer, 0, bytesRead);
                    MessageBox.Show(response);
                }
                else
                {
                    MessageBox.Show("No response from device.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while reading response: {ex.Message}");
            }
        }
        public static bool IsConnected()
        {
            return bluetoothClient.Connected;
        }

        public static void Disconnect()
        {
            if (bluetoothClient.Connected)
            {
                bluetoothClient.Close();
            }
        }
        public static void SendCommand(int vendorId, int commandId, byte[] payload = null)
        {
            if (!bluetoothClient.Connected && commandId != 4096 && commandId != 1 && commandId != 16)
            {
                MessageBox.Show("Speak not available. Unable to connect to Device");
                return;
            }

            if (bluetoothStream == null || !bluetoothStream.CanWrite)
            {
                MessageBox.Show("Bluetooth RFCOMM stream not available.");
                return;
            }

            // Check payload length
            int payloadLength = payload?.Length ?? 0;
            if (payloadLength > 254)
            {
                MessageBox.Show("Payload length too long.");
                return;
            }

            byte flags = 0;
            bool useCheck = (flags & 1) != 0;

            int packetLength = payloadLength + 8 + (useCheck ? 1 : 0);
            var command = new List<byte>(packetLength)
            {
                0xFF,
                1,
                flags,
                (byte)payloadLength,
                (byte)(vendorId >> 8),
                (byte)vendorId,
                (byte)(commandId >> 8),
                (byte)commandId
            };
            if (payload != null)
            {
                command.AddRange(payload);
            }
            if (useCheck)
            {
                byte check = 0xFF;
                for (int idx = 0; idx < command.Count; idx++)
                {
                    check ^= command[idx];
                }
                command.Add(check);
            }

            try
            {
                bluetoothStream.Write(command.ToArray(), 0, command.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while sending command: {ex.Message}");
            }
        }

    }
}
