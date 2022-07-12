namespace BankWebApi.Models
{
    public class Token
    {
        public string access { get; set; }
        public int access_expires { get; set; }
        public string refresh { get; set; }
        public int refresh_expires { get; set; }

    }
}
