using Shared.Controllers;

namespace Client
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            ClientExecuter clientExecuter = new ClientExecuter(new DataSender(), new DataReceiver());
            clientExecuter.ExecuteClient();
        }
    }
}