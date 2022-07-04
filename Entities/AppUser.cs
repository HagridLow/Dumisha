using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser: IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public List<SpotifyAlbumRated> SpotifyAlbumRateds { get; set; } = new List<SpotifyAlbumRated>();
    }
}