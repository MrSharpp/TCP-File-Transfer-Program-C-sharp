using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

class Client{
    static void Main(String[] args){
        Connect("127.0.0.1", "@FILE@newfile.ts@3900", 13000);
    }

    static void Connect(String server, String message, Int32 port){
        try{
            TcpClient client = new TcpClient(server, port);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);

            data = new Byte[256];

            String responseData = String.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            if(responseData.StartsWith("@ACCEPTED@")){
                Console.WriteLine("Handshaked!");
                byte[] fileStream = File.ReadAllBytes("./img19.jpg");
                stream.Write(fileStream, 0, fileStream.Length);

                byte[] endFile = System.Text.Encoding.ASCII.GetBytes("@SENT@");
                stream.Write(endFile, 0, endFile.Length);
                Console.WriteLine("File Sent!");

            }else {
                Console.WriteLine("Handshake Failed!");
            }

            stream.Close();
            client.Close();
        }catch (ArgumentNullException e){
            Console.WriteLine("ArgumentNullException: {0}", e);
        }catch (SocketException e){
            Console.WriteLine("SocketException: {0}", e);
        }

        Console.WriteLine("\n Press Enter to continue...");
        Console.Read();
    }
}