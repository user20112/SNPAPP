using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using ResumeApp.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ResumeApp.Classes
{
    public class MQTTController : IMqttApplicationMessageReceivedHandler
    {
        public string clientId = Guid.NewGuid().ToString();
        public string URI = "http://devlinpaddock.online/";
        public string User = "";
        public string Password = "";
        public int Port = 1883;
        public bool Secure = false;
        private IManagedMqttClient MQTTClient;

        public event EventHandler<MQTTMessageReceivedEventArgs> OnMessageReceived;

        public MQTTController(string URI, int Port, bool Secure = false, string User = "", string Password = "")
        {
            this.URI = URI;
            this.Port = Port;
            this.Secure = Secure;
            this.User = User;
            this.Password = Password;
        }

        public void Connect()
        {
            MqttClientOptionsBuilder messageBuilder = new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithCredentials(User, Password)
                .WithTcpServer(URI, Port)
                .WithCleanSession();
            IMqttClientOptions options = Secure ? messageBuilder
                .WithTls()
                .Build() : messageBuilder.Build();
            ManagedMqttClientOptions managedOptions = new ManagedMqttClientOptionsBuilder().WithAutoReconnectDelay(TimeSpan.FromSeconds(5)).WithClientOptions(options).Build();
            MQTTClient = new MqttFactory().CreateManagedMqttClient();
            Task task = MQTTClient.StartAsync(managedOptions);
            while (task.Status != TaskStatus.RanToCompletion)
                Thread.Sleep(50);
            MQTTClient.ApplicationMessageReceivedHandler = this;
        }

        public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            return Task.Run(() =>
            {
                OnMessageReceived?.Invoke(this, new MQTTMessageReceivedEventArgs(e.ApplicationMessage));
            });
        }

        public async void PublishAsync(string Content, string ResponseTopic, string Topic, byte[] Payload, MqttQualityOfServiceLevel serviceLevel, bool RetainFlag)
        {
            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithContentType(Content)
                .WithResponseTopic(ResponseTopic)
                .WithTopic(Topic)
                .WithPayload(Payload)
                .WithQualityOfServiceLevel(serviceLevel)
                .WithRetainFlag(RetainFlag)
                .Build();
            if (MQTTClient != null)
                await MQTTClient.PublishAsync(message);
        }
    }
}