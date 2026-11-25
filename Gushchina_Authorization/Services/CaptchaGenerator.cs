using System;

namespace Gushchina_Authorization.Services
{
    public static class CaptchaGenerator
    {
        private static Random random = new Random();

        private const string symbols =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateCaptchaText(int length)
        {
            string captcha = "";

            for (int i = 0; i < length; i++)
            {
                captcha += symbols[random.Next(symbols.Length)];
            }

            return captcha;
        }
    }
}