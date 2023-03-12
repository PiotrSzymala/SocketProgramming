namespace Server
{
    public static class Program
    {
        public static readonly DateTime ServerStartCount = DateTime.Now;

        private static void Main(string[] args)
        {
            ServerExecuter.ExecuteServer();
        }
        
    }
}