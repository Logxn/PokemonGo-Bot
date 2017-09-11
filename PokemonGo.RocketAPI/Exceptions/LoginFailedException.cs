using System;

namespace PokemonGo.RocketAPI.Exceptions
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException(System.Net.HttpStatusCode _httpStatusCode) : base()
        {
            if (_httpStatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Logger.Debug("Exception: "+ "Your account is banned, you received a Bad Request from server at login time. Press a key to exit.");
                End();
            }
        }

        public LoginFailedException(string message) : base(message)
        {
            Logger.Debug("Exception: "+ message);
            End();
        }

        public LoginFailedException(Exception ex) : base()
        {
            Logger.Debug("Exception: "+ ex.StackTrace);
            End();
        }

        public LoginFailedException()
        {

        }

        public void End()
        {
            Console.WriteLine("Press a key to finish");
            Console.ReadKey();
            Environment.Exit(0);
        }

    }
}