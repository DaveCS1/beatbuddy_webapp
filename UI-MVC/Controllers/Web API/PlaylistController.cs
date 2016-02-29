﻿using System;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BB.BL;
using BB.BL.Domain;
using BB.BL.Domain.Playlists;
using BB.UI.Web.MVC.Controllers.Utils;

namespace BB.UI.Web.MVC.Controllers.Web_API
{
    [Authorize]
    [RoutePrefix("api/Playlist")]
    public class PlaylistController : ApiController
    {
        private readonly IPlaylistManager playlistManager;
        private readonly IUserManager userManager;

        public PlaylistController()
        {
            playlistManager = new PlaylistManager(ContextEnum.BeatBuddy);
            userManager = new UserManager(ContextEnum.BeatBuddy);
        }

        public PlaylistController(ContextEnum contextEnum)
        {
            playlistManager = new PlaylistManager(contextEnum);
            userManager = new UserManager(contextEnum);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(Playlist))]
        public HttpResponseMessage getPlaylist(long id)
        {
            var playlist = playlistManager.ReadPlaylist(id);
            if(playlist == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, playlist);
        }

        [HttpPost]
        [Route("createPlaylist")]
        [ResponseType(typeof(Playlist))]
        public HttpResponseMessage createPlaylist(FormDataCollection formData)
        {
            var userIdentity = RequestContext.Principal.Identity as ClaimsIdentity;
            if (userIdentity == null) return new HttpResponseMessage(HttpStatusCode.Forbidden);

            var email = userIdentity.Claims.First(c => c.Type == "sub").Value;
            if(email == null) return new HttpResponseMessage(HttpStatusCode.Forbidden);

            var user = userManager.ReadUser(email);
            if (user == null) return new HttpResponseMessage(HttpStatusCode.Forbidden);

            string imagePath = "";
            if (formData["image"] != null && formData["image"].Length > 0)
            {
                try
                {
                    var bitmap = ImageDecoder.DecodeBase64String(formData["image"]);

                    string extension = string.Empty;
                    if (bitmap.RawFormat.Equals(ImageFormat.Jpeg)) extension = ".jpg";
                    if (bitmap.RawFormat.Equals(ImageFormat.Png)) extension = ".png";
                    if (bitmap.RawFormat.Equals(ImageFormat.Bmp)) extension = ".bmp";
                    if (bitmap.RawFormat.Equals(ImageFormat.Gif)) extension = ".gif";

                    if (string.IsNullOrEmpty(extension)) return Request.CreateResponse(HttpStatusCode.BadRequest, "The supplied image is not a valid image");

                    imagePath = FileHelper.NextAvailableFilename(Path.Combine(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["PlaylistImgPath"]), "playlist" + extension));
                    bitmap.Save(imagePath);

                    imagePath = Path.GetFileName(imagePath);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
                }
            }

            var playlist = playlistManager.CreatePlaylistForUser(formData["name"], formData["description"], formData["key"], 1, false, imagePath, null, user);
            if (playlist != null)
                return Request.CreateResponse(HttpStatusCode.OK, playlist);
            
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

    }
}