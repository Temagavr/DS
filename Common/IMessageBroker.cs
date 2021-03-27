namespace Common
{
    public interface IMessageBroker
    {
        void SendMsg(string title, string value);

        void SendMsgToLogger(Event info);
    }
}