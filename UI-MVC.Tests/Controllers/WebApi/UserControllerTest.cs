﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using BB.BL;
using BB.BL.Domain.Users;
using BB.BL.Domain.Playlists;
using BB.DAL.EFPlaylist;
using BB.DAL;
using BB.BL.Domain;
using BB.DAL.EFUser;
using BB.BL.Domain.Organisations;
using BB.UI.Web.MVC.Controllers.Web_API;
using MyTested.WebApi;
using BB.DAL.EFOrganisation;
using System.Collections.Generic;
using BB.UI.Web.MVC.Models;
using MyTested.WebApi.Builders.Contracts.Controllers;

namespace BB.UI.Web.MVC.Tests.Controllers.WebApi
{
    [TestClass]
    public class UserControllerTest
    {
        
        PlaylistManager playlistManager;
        UserManager userManager;
        OrganisationManager organisationManager;
        User user;
        Playlist playlist;
        Organisation organisation;

        IAndControllerBuilder<UserController> userControllerWithAuthenticatedUser;

        [TestInitialize]
        public void Initialize()
        {
           
            playlistManager = new PlaylistManager(new PlaylistRepository(new EFDbContext(ContextEnum.BeatBuddyTest)), new UserRepository(new EFDbContext(ContextEnum.BeatBuddyTest)));
            userManager = new UserManager(new UserRepository(new EFDbContext(ContextEnum.BeatBuddyTest)));
            organisationManager = new OrganisationManager(new OrganisationRepository(new EFDbContext(ContextEnum.BeatBuddyTest)));
            user = userManager.CreateUser("werknu@gmail.com", "matthias", "test", "acidshards", "");
           
            playlist = playlistManager.CreatePlaylistForUser("testplaylist", "gekke playlist om te testen", "125", 5, true, "", user);
            userControllerWithAuthenticatedUser = MyWebApi.Controller<UserController>()
                .WithResolvedDependencyFor<PlaylistManager>(playlistManager)
                .WithResolvedDependencyFor<UserManager>(userManager)
                .WithResolvedDependencyFor<OrganisationManager>(organisationManager)
                .WithAuthenticatedUser(
                 u => u.WithIdentifier("NewId")
                       .WithUsername(user.Email)
                       .WithClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email))
                       .WithClaim(new System.Security.Claims.Claim("sub", user.Email))
                );

            Track track = new Track()
            {
                Artist = "Metallica",
                Title = "Master of Puppets (live)",
                CoverArtUrl = "",
                Duration = 800,
                TrackSource = new TrackSource
                {
                    SourceType = SourceType.YouTube,
                    Url = "https://www.youtube.com/watch?v=kV-2Q8QtCY4"
                }
            };
            Track addedtrack = playlistManager.AddTrackToPlaylist(playlist.Id, track);


            organisation = organisationManager.CreateOrganisation("gek organisatie test","",user);
        }

        /*
        [TestMethod]
        public void RegisterTest() {
            MyWebApi.Controller<UserController>()
                .WithResolvedDependencyFor<PlaylistManager>(playlistManager)
                .WithResolvedDependencyFor<UserManager>(userManager)
                .WithResolvedDependencyFor<OrganisationManager>(organisationManager)
                .CallingAsync(c => c.Register("testvoornaam", "testachternaam", "testnicknaam", "testemailuniek@gmail.com", "Password1",
                "http://www.froot.nl/wp-content/uploads/2012/10/these_funny_animals_1023_640_37.jpeg"
                ))
                
                .Ok()
                .WithResponseModelOfType<User>();
        }*/

        [TestMethod]
        public void GetUserOrganisationsTest() {
            userControllerWithAuthenticatedUser
                .Calling(c => c.GetUserOrganisations())
                .ShouldReturn()
                .Ok()
                .WithResponseModelOfType<IEnumerable<SmallOrganisationViewModel>>();
            ;
            //TODO Move back to cleanup
        }

        /*
        [TestMethod]
        public void GetNullUserOrganisationTest() {
            var userWrongEmail = "doesntexist@gmail.com";
            MyWebApi.Controller<UserController>()
                .WithResolvedDependencyFor<UserManager>(userManager)
                .WithAuthenticatedUser(
                u => u.WithIdentifier("NewId")
                    .WithUsername(userWrongEmail)
                    .WithClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, userWrongEmail))
                    .WithClaim(new System.Security.Claims.Claim("sub", userWrongEmail))
                )
                .Calling(c => c.GetUserOrganisations())
                .ShouldReturn()
                .NotFound();
        }*/

            /*
        [TestMethod]
        public void GetUnauthorizedOrganisationTest() {
            MyWebApi.Controller<UserController>()
                .Calling(c => c.GetUserOrganisations())
                .ShouldReturn()
                .Unauthorized();
        }
        */

        [TestMethod]
        public void GetUserPlaylistsTest() {
            userControllerWithAuthenticatedUser
                 .Calling(c => c.GetUserPlaylists())
                 .ShouldReturn()
                 .Ok();
            //TODO add playlistviewmodel
        }

        [TestMethod]
        public void GetUserTest()
        {
            userControllerWithAuthenticatedUser
                 .Calling(c => c.GetUser (user.Email))
                 .ShouldReturn()
                 .Ok()
                 .WithResponseModelOfType<User>();
        }

        [TestCleanup]
        public void Cleanup() {
            playlistManager.DeletePlaylist(playlist.Id);
            organisationManager.DeleteOrganisation(organisation.Id);
            userManager.DeleteUser(user.Email);
        }
    }
}