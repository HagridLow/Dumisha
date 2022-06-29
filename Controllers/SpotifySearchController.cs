using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Contexts;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Swan.Formatters;
using static API.Entities.SpotifySearch;

namespace API.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SpotifySearchController: BaseAPIController
    {
        private readonly SearchHelper _searchHelper;
        private readonly DataContext _dataContext;
        private readonly IUserAccessor _userAccessor;
        private readonly IdentityContext _identityContext;

        public SpotifySearchController(SearchHelper searchHelper, DataContext dataContext, IUserAccessor userAccessor, IdentityContext identityContext)
        {
            _identityContext = identityContext;
            _userAccessor = userAccessor;
            _dataContext = dataContext;
            _searchHelper = searchHelper;
        }
        
        [HttpGet("getalbum")]
        public async Task<List<SpotifyAlbum>> SearchAlbums(string search)
        {
            await Task.Run(async () => await SearchHelper.GetTokenAsync());
            
            var result = SearchHelper.GetAlbumTrackOrArtist(search);

            var album = new List<SpotifyAlbum>();
            
            foreach(var item in result.albums.items)
            {
                album.Add(new SpotifyAlbum(){
                   ID = item.id,
                   Name = item.name,
                   Artist = item.artists.Any() ? item.artists[0].name : "e pa nema",
                   Image = item.images.Any() ? item.images[0].url: "https://user-images.githubusercontent.com/24848110/33519396-7e56363c-d79d-11e7-969b-09782f5ccbab.png",
                   TotalTracks = item.total_tracks,
                   ReleaseDate = item.release_date     
                });

               
            }    

            return album;
            
        }



        [HttpPost("ratealbum")]
        public async Task<ActionResult<SpotifyAlbumRated>> RateAlbum(string search)
        {    

            await Task.Run(async () => await SearchHelper.GetTokenAsync());

            var result = SearchHelper.GetAlbumTrackOrArtist(search);
    
            var album = new SpotifyAlbumRated();
            
            foreach(var item in result.albums.items)
            {
 
                album.ID = item.id;
                album.Name = item.name;
                album.Artist = item.artists.Any() ? item.artists[0].name : "e pa nema";
                album.Image = item.images.Any() ? item.images[0].url : "https://user-images.githubusercontent.com/24848110/33519396-7e56363c-d79d-11e7-969b-09782f5ccbab.png";
                album.TotalTracks = item.total_tracks;
                album.ReleaseDate = item.release_date;

                var user = await _identityContext.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                var rater = new AlbumRaters
                {
                    AppUser = user,
                    SpotifyAlbumRated = album
                };

                album.Raters.Add(rater);

                _dataContext.SpotifyAlbums.Add(album);
                await _dataContext.SaveChangesAsync();
                return album;

            }    

            


            return album;

        }

        [HttpGet("albums")]
        public async Task<IReadOnlyList<SpotifyAlbum>> GetSpotifyAlbumAsync()
        {
            var album = await _dataContext.SpotifyAlbums.ToListAsync();

            return album;
        }   
    }
}