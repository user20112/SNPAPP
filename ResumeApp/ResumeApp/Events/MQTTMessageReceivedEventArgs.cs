using MQTTnet;

namespace ResumeApp.Events
{
    public class MQTTMessageReceivedEventArgs
    {
        public MqttApplicationMessage Message;

        public MQTTMessageReceivedEventArgs(MqttApplicationMessage message)
        {
            Message = message;
        }
    }
}