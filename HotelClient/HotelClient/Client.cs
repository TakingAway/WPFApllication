﻿using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace HotelClient
{
    static class Client
    {
        static private int port = 8050;
        static private string address = "127.0.0.1";

        static private DataTable GetDataTable(byte[] dtData)
        {
            DataTable dt = null;
            BinaryFormatter bFormat = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(dtData))
            {
                dt = (DataTable)bFormat.Deserialize(ms);
            }
            return dt;
        }

        static public string SendRequestToServer(string message, string command = "")
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            byte[] data = Encoding.Unicode.GetBytes(message);
            socket.Send(data);

            data = new byte[10000];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;

            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            return builder.ToString();
        }

        static public DataTable SendSelectRequestToServer(string message)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            byte[] data = Encoding.Unicode.GetBytes(message);
            socket.Send(data);

            data = new byte[500000];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;

            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);
            DataTable dataTable = GetDataTable(data);

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            return dataTable;

        }
    }
}