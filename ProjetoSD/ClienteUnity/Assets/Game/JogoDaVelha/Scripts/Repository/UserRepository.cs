using UnityEngine;

public class UserRepository
{
    private struct Keys
    {
        public const string UserName = "UserName";
        public const string IPAdress = "IPAdress";
        public const string Port = "Port";
    }

    public static void SaveName(string str) => PlayerPrefs.SetString(Keys.UserName, str);
    public static string GetName() => PlayerPrefs.GetString(Keys.UserName, "");

    public static void SaveIPAdress(string str) => PlayerPrefs.SetString(Keys.IPAdress, str);
    public static string GetIPAdress() => PlayerPrefs.GetString(Keys.IPAdress, "");

    public static void SavePort(int port) => PlayerPrefs.SetInt(Keys.Port, port);
    public static int GetPort() => PlayerPrefs.GetInt(Keys.Port, 0);
}
