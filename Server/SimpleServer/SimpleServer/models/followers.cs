using SpotifyWebAPI;

namespace SimpleServer.models
{
    internal class followers
    {
        public string href { get; set; }
        public int total { get; set; }

        public Followers ToPOCO()
        {
            Followers followers = new Followers();
            followers.HREF = this.href;
            followers.Total = this.total;

            return followers;
        }
    }
}