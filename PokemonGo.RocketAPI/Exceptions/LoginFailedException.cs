using System;

namespace PokemonGo.RocketAPI.Exceptions
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException(System.Net.HttpStatusCode _httpStatusCode) : base()
        {
            if (_httpStatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Logger.ExceptionInfo("Your account is banned, you received a Bad Request from server at login time. Press a key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public LoginFailedException(string message) : base(message)
        {
            Logger.ExceptionInfo(message);
            Console.ReadKey();
            Environment.Exit(0);
        }

        public LoginFailedException(Exception ex) : base()
        {
            Logger.ExceptionInfo(ex.StackTrace);
            Console.ReadKey();
            Environment.Exit(0);
        }

        public LoginFailedException()
        {

        }

    }
}