﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TrickEmu
{
    class PacketWriter
    {
        public static void SelectServer(byte[] dec, Socket sock)
        {
            // Select server
            // 127.0.0.1

            byte[] msg = new byte[] { 0x1B, 0x00, 0x00, 0x00, 0xF2, 0x2C, 0x00, 0x00, 0x00, 0x31, 0x32, 0x37, 0x2E, 0x30, 0x2E, 0x30, 0x2E, 0x31, 0x00, 0x00, 0x00, 0x20, 0x01, 0x00, 0x00, 0x16, 0x27 };
            sock.Send(msg);
        }

        public static void Login(byte[] dec, Socket sock)
        {
            // Login request
            byte[] msg = new byte[] { };

            string uid = Methods.getString(dec, dec.Length).Substring(0, 12);
            string upw = Methods.getString(dec, dec.Length).Substring(19);

            try
            {
                using (MySqlCommand cmd = Program._MySQLConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM users WHERE username = @userid AND password = @userpw;";
                    cmd.Parameters.AddWithValue("@userid", Methods.cleanString(uid));
                    cmd.Parameters.AddWithValue("@userpw", Methods.cleanString(upw));
                    if (Convert.ToInt32(cmd.ExecuteScalar()) >= 1)
                    {
                        //msg = new byte[] { 0x5F, 0x00, 0x00, 0x00, 0xEE, 0x2C, 0x00, 0x00, 0x00, 0x0B, 0xE1, 0xF5, 0x05, 0x65, 0x04, 0x60, 0x93, 0x3D, 0x8C, 0xF5, 0x0F, 0x01, 0x01, 0x01, 0x00, 0x53, 0x68, 0x61, 0x6E, 0x67, 0x68, 0x6F, 0x69, 0x00, 0xED, 0xCB, 0x01, 0x66, 0xC7, 0x53, 0x4E, 0x00, 0xD9, 0xC2, 0x00, 0xA8, 0xFB, 0xCB, 0x01, 0xAA, 0xDD, 0xC2, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0C, 0x57, 0x4F, 0x52, 0x4C, 0x44, 0x31, 0x00, 0x00, 0xD9, 0xC2, 0x00, 0xA8, 0xFB, 0xCB, 0x01, 0xAA, 0xDD, 0xC2, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAC, 0x0D, 0x00, 0x00 };
                        //sock.Send(msg);
                        // Send server select
                        PacketBuffer data = new PacketBuffer();
                        data.WriteHeaderHexString("EE 2C 00 00 00 0B");
                        //data.WriteHexString("E1 F5 05 A3 A4 C7 14 6E C3 53 0D"); // ?
                        data.WriteHexString("00 00 00 00 00 00 00 00 00 00 00");
                        data.WriteByte(2); // Amount of channels??
                        data.WriteByte(1); // Maybe amount of worlds?

                        data.WriteByte(0x01); // World 1
                        data.WriteByte(0x00); // Padding
                        data.WriteString("Shanghai", 32);
                        data.WriteByte(0x00);
                        data.WriteString("Channel name", 32);
                        data.WriteByte(0x00);
                        data.WriteHexString("AC 0D");
                        data.WriteBytePad(0x00, 4);

                        // World 2
                        data.WriteByte(0x02);
                        data.WriteByte(0x00);
                        data.WriteString("Shanghai", 32);
                        data.WriteByte(0x00);
                        data.WriteString("Channel name", 32);
                        data.WriteByte(0x00);
                        data.WriteHexString("AC 0D");
                        data.WriteBytePad(0x00, 4);

                        sock.Send(data.getPacketDecrypted());
                    }
                    else
                    {
                        msg = new byte[] { 0x0D, 0x00, 0x00, 0x00, 0xEF, 0x2C, 0x00, 0x00, 0x00, 0x63, 0xEA, 0x00, 0x00 };
                        sock.Send(msg);
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database error: " + ex);
            }
        }
    }
}
