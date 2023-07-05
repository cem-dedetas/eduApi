using System;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
using System.ComponentModel;

namespace EduAPI

{
    public class ParticipantModel
    {
        public string uid { get; set; }
        public string rid { get; set; }
        public string token { get; set; }
        public int ctype { get; set; }
    }
    public class AgoraServiceClass
    {


        IConfigurationBuilder configuration;
        IConfigurationRoot config;
        string appId;
        string agoraAppCertificate;
        string secretKey;
        int vendor;
        int region;
        string bucket;
        string accessKey;

        string customerKey;
        // Customer secret
        string customerSecret;
        // Concatenate customer keys

        // Encode with base64
        string encodedCredential;

        public AgoraServiceClass()
        {
            configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            config = configuration.Build();
            appId = config.GetSection("AGORA_APP_ID").Value ?? "";
            agoraAppCertificate = config.GetSection("AGORA_APP_CERT").Value ?? "";
            secretKey = config.GetSection("S3_SECRET").Value ?? "";
            vendor = Convert.ToInt32(config.GetSection("S3_VENDOR").Value);
            region = Convert.ToInt32(config.GetSection("S3_REGION").Value);
            bucket = config.GetSection("S3_BUCKET").Value ?? "";
            accessKey = config.GetSection("S3_ACCESS").Value ?? "";

            customerKey = config.GetSection("AGORA_KEY").Value ?? "";
            customerSecret = config.GetSection("AGORA_SECRET").Value ?? "";
            encodedCredential = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{customerKey}:{customerSecret}"));
        }

        
        
        public async Task<string> AcquireResource(string channelName, string recordingUid)
        {
            var client = new HttpClient();

            var url = $"https://api.agora.io/v1/apps/{appId}/cloud_recording/acquire";

            var requestData = new
            {
                cname = channelName,
                uid = recordingUid,
                clientRequest = new {
                    resourceExpiredHour= 24,
                    scene= 0
                }
            };

            var json = JsonConvert.SerializeObject(requestData);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredential);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            


            // Handle the response as required

            return responseContent;
        }

        public async Task<string> StartRecording(string resourceId, string channelName, string uid, string token, int channelType, string presenter)
        {

            var client = new HttpClient();
            string[] recordingtypes = { "hls", "mp4" };
            int[] parti = { 5 };
            //var test = "https://webhook.site/0232b90e-a1d5-4e79-921a-41426aea21d7";
            var url = $"https://api.agora.io/v1/apps/{appId}/cloud_recording/resourceid/{resourceId}/mode/mix/start";

            var requestData = new
            {
                //url = url,
                cname = channelName,
                uid = uid,
                clientRequest = new
                {
                    token = token,
                    recordingConfig = new
                    {
                        channelType = 0,
                        streamTypes = 2,
                        audioProfile = 0,
                        videoStreamType = 0,
                        maxIdleTime = 300,
                        transcodingConfig = new
                        {
                            width = 1280,
                            height = 720,
                            fps = 30,
                            bitrate = 1000,
                            mixedVideoLayout = 0
                            

                        }
                    },
                    storageConfig = new
                    {
                        secretKey = secretKey,
                        vendor = 1,
                        region = 0,
                        bucket = bucket,
                        accessKey = accessKey
                    }
                }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredential);

            var response = await client.PostAsync(url, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            // Handle the response as required
            return responseContent;

        }

        public async Task<string> StopRecording(string resourceId, string sid, string recordingUid, string channelName)
        {
            var client = new HttpClient();


            var url = $"https://api.agora.io/v1/apps/{appId}/cloud_recording/resourceid/{resourceId}/sid/{sid}/mode/mix/stop";
            var requestData = new
            {
                cname = channelName,
                uid = recordingUid,
                clientRequest = new { }
            };

            var json = JsonConvert.SerializeObject(requestData);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredential);

            var response = await client.PostAsync(url, content);

            // Handle the response as required
            var responseContent = await response.Content.ReadAsStringAsync();

            // Handle the response as required
            return responseContent;
        }
        public async Task<string> TokenGenerator(string cname, string uid)
        {
            var client = new HttpClient();

            var url = $"http://3.82.92.7:3100/rtc/{cname}/publisher/uid/{uid}/?expiry=38400";

            var response = await client.GetAsync(url);

            // Handle the response as required
#pragma warning disable CS8603 // Possible null reference return.
            return await response.Content.ReadFromJsonAsync<string>();
#pragma warning restore CS8603 // Possible null reference return.

        }
        public async Task<string> TokenGenerator2(string uid)
        {
            var client = new HttpClient();

            var url = $"http://3.82.92.7:3100/rtm/{uid}";

            var response = await client.GetAsync(url);

            // Handle the response as required
#pragma warning disable CS8603 // Possible null reference return.
            return await response.Content.ReadFromJsonAsync<string>();
#pragma warning restore CS8603 // Possible null reference return.

        }

    }


        
}
