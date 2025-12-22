using javax.swing.text;
using javax.xml.crypto;
using Kolibri.net.C64Sorter.Entities;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using static Kolibri.net.C64Sorter.Controllers.Ultimate64Commands;
using Version = Kolibri.net.C64Sorter.Entities.Version;

// Make sure to install Newtonsoft.Json NuGet package if using older .NET versions, 
// or use the built-in System.Text.Json in modern .NET.
namespace Kolibri.net.C64Sorter.Controllers
{
    internal class UltmateEliteClient
    {

        private readonly HttpClient _httpClient;

        public UltmateEliteClient(string clientName)
        {
            HttpClient _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"http://{clientName}/"); // Replace with your API base URL
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Version>> GetVersionAsync() // Replace Product with your model class
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/version"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Version>>();
            }
            else
            {
                throw new Exception($"{response.RequestMessage} Error: {response.StatusCode}");
            }
        }
        public async Task<Configs> GetCategories() // Replace Product with your model class
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/configs"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Configs>();
            }
            else
            {
                throw new Exception($"{response.RequestMessage} Error: {response.StatusCode}");
            }
        }
        public async Task UploadAndRunPrg(string ipAddress, string localFilePath)
        {
            using HttpClient client = new HttpClient();
            string url = $"http://{ipAddress}/v1/runners:run_prg";

            // Read the .prg file into a byte array
            byte[] prgData = await File.ReadAllBytesAsync(localFilePath);

            // Create the content as a binary stream
            using ByteArrayContent content = new ByteArrayContent(prgData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // Post the binary data to the Ultimate
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }


        public string FtpUpload(string ipAddress, string localPath)

        {
            string remoteFileName = $"{Path.GetFileName(localPath)}";

            // The Ultimate-64 typically uses /usb0/ or /flash/
            string ftpUrl = $"ftp://{ipAddress}/Temp/{remoteFileName}";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            // No credentials usually needed, but you can provide dummy ones
            request.Credentials = new NetworkCredential("anonymous", string.Empty);

            byte[] fileContents = File.ReadAllBytes(localPath);
            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload Finished: {response.StatusDescription}");
            }
            Thread.Sleep(500);
            return ftpUrl;
        }
        public async Task MountAndRunExistingFile(string ipAddress, string remoteFileName)
        {
            using HttpClient client = new HttpClient();

            // 1. Mount the local file (now it has a name, extension doesn't matter if you use ?type=d64)
            string mountUrl = $"http://{ipAddress}/v1/drives/8:mount?image=/Temp/{remoteFileName}&type=d64";
            await client.PutAsync(mountUrl, null);

            // 2. Wait 1 second for the 1541 hardware to "see" the disk
            await Task.Delay(1000);

            // 3. Send the keyboard commands
            string runCmd = "LOAD\"*\",8,1\rRUN\r";
            string keyboardUrl = $"http://{ipAddress}/v1/machine:keyboard?data={Uri.EscapeDataString(runCmd)}";
            await client.PutAsync(keyboardUrl, null);
        }


        public async Task UploadAndMountD64(string ipAddress, string localFilePath)
        {
            byte[] d64Data = await File.ReadAllBytesAsync(localFilePath);

            using HttpClient client = new HttpClient();

            // 1. Force simple binary transfer (avoids Ultimate rejecting the stream)
            client.DefaultRequestHeaders.ExpectContinue = false;
            client.DefaultRequestHeaders.TransferEncodingChunked = false;

            using ByteArrayContent content = new ByteArrayContent(d64Data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Headers.ContentLength = d64Data.Length;

            // 2. EXPLICITLY set the type to d64 so the filename doesn't matter
            string url = $"http://{ipAddress}/v1/drives/8:mount?type=d64";

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                // 3. Pause for hardware emulation and then start
                await Task.Delay(1000);
            //    await MountAndRunExistingFile(ipAddress);
            }
        }

        public async void MountAndRunExistingTempFile(string ipAddress, string remoteFileName)
        {
            using HttpClient client = new HttpClient();
            
            // 1. Mount the local file (now it has a name, extension doesn't matter if you use ?type=d64)
            string mountUrl = $"http://{ipAddress}/v1/drives/a:mount?image=Temp/{remoteFileName}&type=d64";
            await client.PutAsync(mountUrl, null);

            // 2. Wait 1 second for the 1541 hardware to "see" the disk
            await Task.Delay(2000);

            // 3. Send the keyboard commands
            string runCmd = "load\"*\",8,1\rrun\r";
            // Ensure the data is URL-encoded
            string keyboardUrl = $"http://{ipAddress}/v1/machine:keyboard?data={Uri.EscapeDataString(runCmd)}";
            var result = await client.PutAsync(keyboardUrl, null);
            if (!result.IsSuccessStatusCode)
            {
                 SendCommand(runCmd, ipAddress); 
            }
        }
        private void SendCommand(string command, string ipAddress)
        {
            Config config = new Config() { Hostname = ipAddress };
            String text = command.ToUpper();
            if (!text.EndsWith('\r'))            {                text += "\r";            }
            var data = Encoding.ASCII.GetBytes(text);
            int startIndex = 0;
            const int count = 10;  // Max size of C64 keyboard buffer

            do
            { 
                SendCommand(config, SocketCommand.SOCKET_CMD_KEYB, data.Skip(startIndex).Take(count).ToArray(), false);
                startIndex += count;
            }
            while (startIndex < data.Length);

        }
        private static byte[] SendCommand(Config config, SocketCommand Command, byte[] data, bool WaitReply)
        {
            String hostname = config.Hostname;
            int port = config.Port;

            // Create a TCP/IP  socket.  
            Socket sender = new Socket(SocketType.Stream, ProtocolType.Tcp);

            if (data == null) data = new byte[0];
            byte[] reply = null;

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                // sender.Connect(hostname, port);  Timeout controlled by OS - too long

                // Connect using a timeout
                IAsyncResult result = sender.BeginConnect(hostname, port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(SOCKET_TIMEOUT, true);

                if (sender.Connected)
                {
                    sender.EndConnect(result);
                }
                else
                {
                    sender.Close();
                    throw new ApplicationException("Failed to connect - Check IP Address.");
                }

                byte[] tosend = new byte[4 + data.Length];
                tosend[0] = Utilities.GetLowByte((UInt16)Command);
                tosend[1] = Utilities.GetHighByte((UInt16)Command);
                tosend[2] = Utilities.GetLowByte((UInt16)data.Length);
                tosend[3] = Utilities.GetHighByte((UInt16)data.Length);
                Array.Copy(data, 0, tosend, 4, data.Length);

                // Send the data through the socket.  
                int bytesSent = sender.Send(tosend);

                // Wait for a reply?
                if (WaitReply)
                {
                    reply = GetReply(sender);
                }

                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return reply;
            }
            catch (Exception e)
            {
                throw new Exception("Ultimate64: Cannot send command: " + e.Message);
            }
        }


        //public async Task RunLocalPrg(string ipAddress, string filePathOnUltimate)
        //{
        //    using HttpClient client = new HttpClient();

        //    // Example: 192.168.1.100
        //    string url = $"http://{ipAddress}/v1/runners:run_prg?file={Uri.EscapeDataString(filePathOnUltimate)}";

        //    HttpResponseMessage response = await client.PutAsync(url, null);
        // var msg=   response.EnsureSuccessStatusCode();
        //    if (msg.IsSuccessStatusCode) {
        //        var text = "ja"; 
        //    }
        //}

    }
}