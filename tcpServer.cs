using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class MyTcpListener
{
  public static void Main()
  {
    TcpListener server=null;
    try
    {
      // Set the TcpListener on port 13000.
      Int32 port = 13000;
      IPAddress localAddr = IPAddress.Parse("0.0.0.0");

      // TcpListener server = new TcpListener(port);
      server = new TcpListener(localAddr, port);

      // Start listening for client requests.
      server.Start();

      // Buffer for reading data
      Byte[] bytes = new Byte[256];
      String data = null;

      // Enter the listening loop.
      while(true)
      {
        Console.Write("Waiting for a connection... ");

        // Perform a blocking call to accept requests.
        // You could also use server.AcceptSocket() here.
        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("Connected!");

        data = null;

        // Get a stream object for reading and writing
        NetworkStream stream = client.GetStream();

        int i, b;
        long fileSize;
        string fileName;

        // Loop to receive all the data sent by the client.
        while((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
          Console.WriteLine("i={0}", i);

          // Translate data bytes to a ASCII string.
          data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

          if(data.StartsWith("@FILE@")){
            fileName = data.Split('@')[2];
            fileSize = int.Parse(data.Split('@')[3]);
            Console.WriteLine("File Size: {0}, File Name: {1}", fileSize, fileName);
            byte[] fileAccept = System.Text.Encoding.ASCII.GetBytes("@ACCEPTED@");
            stream.Write(fileAccept, 0, fileAccept.Length);

            FileStream file = new FileStream("./hello.jpg", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            byte[] filePacket = new Byte[256];
            while((b = stream.Read(filePacket, 0, filePacket.Length)) != 0){
                Console.WriteLine("b={0}", b);

                data = System.Text.Encoding.ASCII.GetString(filePacket, 0, b);
                if(data == "@SENT@"){
                  Console.WriteLine("File Received!");
                }else {
                  file.Write(filePacket, 0, b);
                }
            }
            fileSize = file.Length;
            file.Close();

            Console.WriteLine("File Received!, File Size:{0}", fileSize);

          }
        }

        // Shutdown and end connection
        client.Close();
      }
    }
    catch(SocketException e)
    {
      Console.WriteLine("SocketException: {0}", e);
    }
    finally
    {
       // Stop listening for new clients.
       server.Stop();
    }

    Console.WriteLine("\nHit enter to continue...");
    Console.Read();
  }
}