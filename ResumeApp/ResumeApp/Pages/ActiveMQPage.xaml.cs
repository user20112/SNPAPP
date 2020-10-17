using Apache.NMS;
using ResumeApp.Classes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveMQPage : BaseContentPage
    {
        protected static AutoResetEvent semaphore = new AutoResetEvent(false);
        protected static ITextMessage message = null;
        protected static TimeSpan receiveTimeout = TimeSpan.FromSeconds(10);
        private IDestination MQTTTopic;
        private IDestination StompTopic;
        private IDestination OpenWireTopic;
        private IMessageConsumer MQTTConsumer;
        private IMessageProducer MQTTProducer;
        private IMessageProducer OpenWireProducer;
        private IMessageProducer StompProducer;
        private IMessageConsumer StompConsumer;
        private IMessageConsumer OpenWireConsumer;
        private IConnection connection;
        private ISession session;
        private MQTTController MQTTController;

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (MQTTTopic != null)
                MQTTTopic.Dispose();
            if (StompTopic != null)
                StompTopic.Dispose();
            if (OpenWireTopic != null)
                OpenWireTopic.Dispose();
            if (MQTTConsumer != null)
                MQTTConsumer.Dispose();
            if (StompConsumer != null)
                StompConsumer.Dispose();
            if (OpenWireConsumer != null)
                OpenWireConsumer.Dispose();
            if (connection != null)
                connection.Dispose();
            if (session != null)
                session.Dispose();
        }

        public ActiveMQPage()
        {
            InitializeComponent();
            Task.Run(() => { SetupActiveMQReceive(); });
            Task.Run(() => { SetupStompSend(); });
            Task.Run(() => { SetupMQTTSend(); });
            Task.Run(() => { SetupOpenWireSend(); });
        }

        public void SetupStompSend()
        {
        }

        public void SetupMQTTSend()
        {
            MQTTController = new MQTTController("http://devlinpaddock.online", 1883, false);
            MQTTController.Connect();
        }

        public void SetupOpenWireSend()
        {
        }

        public void SetupActiveMQReceive()
        {
            Uri connecturi = new Uri("activemq:tcp://devlinpaddock.online:61616");
            Console.WriteLine("About to connect to " + connecturi);
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);
            connection = factory.CreateConnection();
            session = connection.CreateSession();
            MQTTTopic = session.GetTopic("MQTTTopic");
            StompTopic = session.GetTopic("StompTopic");
            OpenWireTopic = session.GetTopic("OpenWireTopic");
            MQTTConsumer = session.CreateConsumer(MQTTTopic);
            StompConsumer = session.CreateConsumer(StompTopic);
            OpenWireConsumer = session.CreateConsumer(OpenWireTopic);
            MQTTProducer = session.CreateProducer(MQTTTopic);
            StompProducer = session.CreateProducer(StompTopic);
            OpenWireProducer = session.CreateProducer(OpenWireTopic);
            MQTTConsumer.Listener += new MessageListener(MQTTReceived);
            StompConsumer.Listener += new MessageListener(StompReceived);
            OpenWireConsumer.Listener += new MessageListener(OpenWireReceived);
            connection.Start();
        }

        private void MQTTReceived(IMessage message)
        {
            Main.Post(delegate
            {
                MQTTReceivedLabel.Text = (message as ITextMessage).Text;
            }
            , null);
        }

        private void StompReceived(IMessage message)
        {
            Main.Post(delegate
            {
                StompReceivedLabel.Text = (message as ITextMessage).Text;
            }
            , null);
        }

        private void OpenWireReceived(IMessage message)
        {
            Main.Post(delegate
            {
                OpenWireReceivedLabel.Text = (message as ITextMessage).Text;
            }
            , null);
        }

        private void MQTTSendButton_Clicked(object sender, EventArgs e)
        {
            ITextMessage request = session.CreateTextMessage(SendMQTTEditor.Text);
            MQTTProducer.Send(request);
            //MQTTController.PublishAsync(SendMQTTEditor.Text, "", "MQTTTopic", new byte[0], MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, false);
        }

        private void StompSendButton_Clicked(object sender, EventArgs e)
        {
            ITextMessage request = session.CreateTextMessage(SendStompEditor.Text);
            StompProducer.Send(request);
        }

        private void OpenWireSendButton_Clicked(object sender, EventArgs e)
        {
            ITextMessage request = session.CreateTextMessage(SendOpenWireEditor.Text);
            OpenWireProducer.Send(request);
        }

        protected static void OnMessage(IMessage receivedMsg)
        {
            message = receivedMsg as ITextMessage;
            semaphore.Set();
        }
    }
}