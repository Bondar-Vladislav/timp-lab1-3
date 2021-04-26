using System.Net;

namespace Server
{
    public class Client
    {
        public IPEndPoint EndPoint { get; set; }

        public Client(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }
    }
}
