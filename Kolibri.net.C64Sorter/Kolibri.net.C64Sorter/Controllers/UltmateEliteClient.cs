using Kolibri.net.C64Sorter.Entities;
using Kolibri.net.Common.Utilities.Extensions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using static Kolibri.net.C64Sorter.Controllers.Ultimate64Commands;
using Version = Kolibri.net.C64Sorter.Entities.Version;

// Make sure to install Newtonsoft.Json NuGet package if using older .NET versions, 
// or use the built-in System.Text.Json in modern .NET.
namespace Kolibri.net.C64Sorter.Controllers
{
    public class UltmateEliteClient
    {
        private readonly HttpClient _httpClient;
        private string _clientName;
        public string ClientName { get { return _clientName; } }

        public  static FileInfo AppsettingsPath { get { return new FileInfo(@".\Resources\appsettings.json"); } }
        public static  DirectoryInfo ResourcesPath { get { return new DirectoryInfo(@".\Resources\"); } }

        public UltmateEliteClient(string clientName)
        {
            _clientName = clientName;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"http://{_clientName}/"); // Replace with your API base URL
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Version> GetVersionAsync() // Replace Product with your model class
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/version"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Version>();
            }
            else
            {
                throw new Exception($"{response.RequestMessage} Error: {response.StatusCode}");
            }
        }

        public async Task UploadAndRunPrgOrCrt(string ipAddress, FileInfo localFilePath)
        {
            var ext = Path.GetExtension(localFilePath.FullName).TrimStart('.').ToLower();
            using HttpClient client = new HttpClient();
            string url = $"http://{ipAddress}/v1/runners:run_{ext}";
            if (ext.Equals("sid") || ext.Equals("mod"))
                url = $"http://{ipAddress}/v1/runners:{ext}play";


            // Read the .prg file into a byte array
            byte[] prgData = await File.ReadAllBytesAsync(localFilePath.FullName);

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
        //public async Task MountAndRunExistingFile(string ipAddress, string remoteFileName)
        //{
        //    using HttpClient client = new HttpClient();
        //    var ext = Path.GetExtension(remoteFileName).TrimStart('.').ToLower();

        //    // 1. Mount the local file (now it has a name, extension doesn't matter if you use ?type=d64)
        //    string mountUrl = $"http://{ipAddress}/v1/drives/8:mount?image=/Temp/{remoteFileName}&type=d64";
        //    await client.PutAsync(mountUrl, null);

        //    // 2. Wait 1 second for the 1541 hardware to "see" the disk
        //    await Task.Delay(1000);

        //    // 3. Send the keyboard commands
        //    string runCmd = "LOAD\"*\",8,1\rRUN\r";
        //    string keyboardUrl = $"http://{ipAddress}/v1/machine:keyboard?data={Uri.EscapeDataString(runCmd)}";
        //    await client.PutAsync(keyboardUrl, null);
        //}


        //public async Task UploadAndMountD64(string ipAddress, string localFilePath)
        //{
        //    byte[] d64Data = await File.ReadAllBytesAsync(localFilePath);

        //    using HttpClient client = new HttpClient();

        //    // 1. Force simple binary transfer (avoids Ultimate rejecting the stream)
        //    client.DefaultRequestHeaders.ExpectContinue = false;
        //    client.DefaultRequestHeaders.TransferEncodingChunked = false;

        //    using ByteArrayContent content = new ByteArrayContent(d64Data);
        //    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //    content.Headers.ContentLength = d64Data.Length;

        //    // 2. EXPLICITLY set the type to d64 so the filename doesn't matter
        //    string url = $"http://{ipAddress}/v1/drives/8:mount?type=d64";

        //    var response = await client.PostAsync(url, content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // 3. Pause for hardware emulation and then start
        //        await Task.Delay(1000);
        //    //    await MountAndRunExistingFile(ipAddress);
        //    }
        //}

        public async void MountAndRunExistingTempFile(string ipAddress, string remoteFileName)
        {
            var drive = "a";
            using HttpClient client = new HttpClient();
            var mode = "1541";//1541, 1571 and 1581
            var ext = Path.GetExtension(remoteFileName).TrimStart('.').ToLower();

            // 1. Mount the local file (now it has a name, extension doesn't matter if you use ?type=d64)
            string mountUrl = $"http://{ipAddress}/v1/drives/{drive}:mount?image=Temp/{remoteFileName}&type={ext}";

            switch (ext)// d64, g64, d71, g71 or d81.
            {
                case "d71":
                case "g71":
                    mode = "1571";

                    break;
                case "d81":
                    mode = "1581";

                    break;

                default:
                    mode = "1541";//1541, 1571 and 1581
                    break;
            }
            mountUrl += $"&mode={mode}";

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
            if (!text.EndsWith('\r')) { text += "\r"; }
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

        #region Machine commands
        internal async Task<bool> MachineReset()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/version"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                string url = $"http://{_clientName}/v1/machine:reset";
                var res = await _httpClient.PutAsync(url, null);
                return res.IsSuccessStatusCode;
            }
            return false;
        }

        internal async Task<bool> MachineReboot()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/version"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                string url = $"http://{_clientName}/v1/machine:reboot";
                var res = await _httpClient.PutAsync(url, null);
                return res.IsSuccessStatusCode;
            }
            return false;
        }

        internal async Task<bool> MachinePowerOff()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/version"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                string url = $"http://{_clientName}/v1/machine:poweroff";
                var res = await _httpClient.PutAsync(url, null);
                return res.IsSuccessStatusCode;
            }
            return false;
        }

        internal async Task<bool> MachineResume()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/version"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                string url = $"http://{_clientName}/v1/machine:resume";
                var res = await _httpClient.PutAsync(url, null);
                return res.IsSuccessStatusCode;
            }
            return false;
        }

        internal async Task<bool> MachinePause()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/version"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                string url = $"http://{_clientName}/v1/machine:pause";
                var res = await _httpClient.PutAsync(url, null);
                return res.IsSuccessStatusCode;
            }
            return false;
        }
        #endregion
        #region Configuration commands


        internal async Task<int> ConfigurationGetVolumeLevel()
        {
            int ret = 0;
            var url = $"v1/configs/Speaker%20Mixer/Vol%20UltiSid%20{1}*/*current";
            HttpResponseMessage response = await _httpClient.GetAsync(url); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                if (response.IsSuccessStatusCode)
                {
                    var json =  await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(json.Replace(" ", string.Empty));
                    var test = $"{data.SpeakerMixer.VolUltiSid1.current}".Replace("dB", string.Empty).ToInt32(); 
                    ret = test;
                }
                else
                {
                    return ret;
                }
            }
            return ret;
        }

        internal async Task<bool> ConfigurationVolumeLevel(int level)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/version"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                int value = level;
                if (level < -18)
                {
                    if (level > -27)
                        value = -24;
                    else if (level > -30)
                        value = -27;
                    else if (level > -36)
                        value = -30;
                    else if (level > -36)
                        value = -30;
                    else value = -42;
                }

                //     var url = $"http://{_clientName}/v1/configs/Speaker%20Mixer/Vol%20UltiSid%20{2}?value={value}%20dB";
                var url = $"v1/configs/Speaker%20Mixer/Vol%20UltiSid%20{1}?value={value}%20dB";
                var result = await _httpClient.PutAsync(url, null);
                url = $"v1/configs/Speaker%20Mixer/Vol%20UltiSid%20{2}?value={value}%20dB";
                result = await _httpClient.PutAsync(url, null);
                string body = await result.Content.ReadAsStringAsync();
                return result.IsSuccessStatusCode;

                //using var client = new HttpClient();
                //client.BaseAddress = new Uri("http://192.168.5.65");

                //string category = Uri.EscapeDataString("Speaker Mixer");
                //string item = Uri.EscapeDataString("Vol UltiSid 1");

                //string url = $"/v1/configs/{category}/{item}?value=-5%20dB";

                //var result = await client.PutAsync(url, null);
                //string body = await result.Content.ReadAsStringAsync();

                //return result.IsSuccessStatusCode;





            }
            return false;
        }
        internal async Task<bool> ConfigurationVolumeOff()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/version"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                //Speaker Mixer/ Vol UltiSid 1
                //     string url = $"v1/configs/Speaker%20Mixer/Vol%20UltiSid%201?value={level}%20dB";
                //     var reult = await _httpClient.PutAsync(url, null); 

                var url = $"http://{_clientName}/v1/configs/Speaker%20Mixer/Speaker%20Enable?value=Disabled";
                var result = await _httpClient.PutAsync(url, null);
                //Thread.Sleep(1000);
                //url = $"http://{_clientName}/v1/configs/Audio%20Mixer/Vol%20UltiSid%201?value=Off";
                //result = await _httpClient.PutAsync(url, null);

                return result.IsSuccessStatusCode;
            }
            return false;
        }

        internal async Task<string> ConfigurationGetSpeakerEnable()
        {   var url = $"v1/configs/Speaker%20Mixer/Speaker%20Enable/";
            HttpResponseMessage response = await _httpClient.GetAsync(url); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                // Read response content as a string asynchronously
                string responseBody =await   response.Content.ReadAsStringAsync(); 
              
       
                Speaker  dynamicObject =    JsonConvert.DeserializeObject<Speaker>(responseBody);

                var ret = dynamicObject.SpeakerMixer.SpeakerEnable.current;
                return ret; 
            }
            return null;
        }

        internal async Task<bool> ConfigurationSpeakerEnable(string state = "Enabled")
        {
            var url = $"v1/configs/Speaker%20Mixer/Speaker%20Enable?value={state}";
            HttpResponseMessage response = await _httpClient.PutAsync(url, null);
            return response.IsSuccessStatusCode;
        }



        public async Task<string> GetConfigs(string category = null)
        {
            string url = "v1/configs";
            if (category != null)
                url = url + $"/{Uri.EscapeDataString(category)}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"{response.RequestMessage} Error: {response.StatusCode}");
            }
        }

        public async Task<Configs> GetConfigs()
        {
            string url = "v1/configs";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var text = response.Content.ReadAsStringAsync();
                return await response.Content.ReadFromJsonAsync<Configs>();
            }
            else
            {
                throw new Exception($"{response.RequestMessage} Error: {response.StatusCode}");
            }
        }

        internal void GetSystemInformation()
        {
            throw new NotImplementedException();
        }

        /* volume control values
        Set UltiSID 1 volume

To set the volume for UltiSID 1:

PUT http://<IP>/v1/configs/Audio%20Mixer/Vol%20UltiSid%201?value=-6%20dB


Or for another level like 0 dB:

PUT http://<IP>/v1/configs/Audio%20Mixer/Vol%20UltiSid%201?value=0%20dB


Note:

If using curl it would look like:

curl -X PUT "http://<IP>/v1/configs/Audio%20Mixer/Vol%20UltiSid%201?value=-12%20dB"

Set UltiSID 2 volume

Similarly, to change UltiSID 2:

PUT http://<IP>/v1/configs/Audio%20Mixer/Vol%20UltiSid%202?value=-12%20dB


Or “Off”:

PUT http://<IP>/v1/configs/Audio%20Mixer/Vol%20UltiSid%202?value=Off
        */



        #endregion
                    #region System commands
        public async Task<UltimateSystem> GetSystemInformationAsync() // Replace Product with your model class
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/system"); // Replace "products" with your endpoint path
            if (response.IsSuccessStatusCode)
            {
                string url = $"http://{_clientName}/v1/machine:reset";
                return await response.Content.ReadFromJsonAsync<UltimateSystem>();
            }
            return null;
        }

        internal async Task<bool> PostUrl(string url)
        {
            HttpResponseMessage response = await _httpClient.PostAsync(url, null); // Replace "products" with your endpoint path
            return (response.IsSuccessStatusCode);
        }
        internal async void sendCommand(string command)
        {




            // 3. Send the keyboard commands
            string runCmd = command;
            // Ensure the data is URL-encoded
            string keyboardUrl = $"v1/machine:keyboard?data={Uri.EscapeDataString(runCmd)}";
            var result = await _httpClient.PutAsync(keyboardUrl, null);
            if (!result.IsSuccessStatusCode)
            {
                SendCommand(runCmd, _clientName);
            }
        }
        internal async Task<bool> PutUrl(string url, bool run = false)
        {
            {

                HttpResponseMessage response = await _httpClient.PutAsync(url, null); // Replace "products" with your endpoint path
                if (response.IsSuccessStatusCode)
                {
                    var text = response.Content.ReadAsStringAsync();

                    if (run)
                    { 
                        // 3. Send the keyboard commands
                        string runCmd = "load\"*\",8,1\rrun\r";
                        sendCommand(runCmd);
                    }

                    return response.IsSuccessStatusCode;
                }
                else
                {
                    var text = response.Content.ReadAsStringAsync();
                    return (response.IsSuccessStatusCode);
                }
            }
            #endregion
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
}