﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using BB.BL.Domain;
using BB.BL.Domain.Organisations;
using BB.BL.Domain.Playlists;
using BB.BL.Domain.Users;

namespace BB.DAL.EFPlaylist
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly EFDbContext context;
        
        public PlaylistRepository(EFDbContext context)
        {
            this.context = context;
        }

        public Comment CreateComment(Comment comment)
        {
            throw new NotImplementedException();
        }

        public Playlist CreatePlaylist(Playlist playlist)
        {
            playlist = context.Playlists.Add(playlist);
            context.SaveChanges();
            return playlist;
        }

        public Playlist CreatePlaylist(Playlist playlist, Organisation organisation)
        {
            var playlist1 = playlist;
            var organisation1 = context.Organisations.Find(organisation.Id);
            organisation1.Playlists.Add(playlist1);
            context.Playlists.Add(playlist1);
            context.SaveChanges();
            return playlist;
        }

        public IEnumerable<Playlist> ReadPlaylistsForUser(long userId)
        {
           return context.Playlists.ToList().FindAll(p => p.CreatedById == userId);
        }

        public PlaylistTrack CreatePlaylistTrack(PlaylistTrack playlistTrack)
        {
            throw new NotImplementedException();
        }

        public Track CreateTrack(Track track)
        {
            throw new NotImplementedException();
        }

        public TrackSource CreateTrackSource(TrackSource trackSource)
        {
            throw new NotImplementedException();
        }

        public Vote CreateVote(Vote vote, long userId, long trackId)
        {
            var user = context.User.Find(userId);
            vote.User = user;
            var playlistTrack = context.PlaylistTracks.Find(trackId);
            var playlist = context.Playlists.Where(p => p.PlaylistTracks.Any(t => t.Id == trackId)).FirstOrDefault();
            var count = playlist.PlaylistTracks.SelectMany(p => p.Votes).Where(v => v.User.Id == userId).Count();
            if(count >= playlist.MaximumVotesPerUser) { return null; }
            vote = context.Votes.Add(vote);
            playlistTrack.Votes.Add(vote);
            context.SaveChanges();
            return vote;
        }

        public int ReadMaximumVotesPerUser(long trackId) {
            var playlist = context.Playlists.Where(p => p.PlaylistTracks.Any(t => t.Id == trackId)).FirstOrDefault();
            return playlist.MaximumVotesPerUser;
        }

        public int ReadNumberOfVotesUserForPlaylist(long userId, long trackId)
        {
            var playlist = context.Playlists.Where(p => p.PlaylistTracks.Any(t => t.Id == trackId)).FirstOrDefault();
            var count = playlist.PlaylistTracks.SelectMany(p => p.Votes).Where(v => v.User.Id == userId).Count();
            return count;
        }

        public void DeleteComment(long commentId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Playlist> ReadPlaylists(long userId)
        {
            return context.Playlists.Where(p => p.CreatedById == userId);
        }

        public Playlist DeletePlaylist(long playlistId)
        {
            var playlist = ReadPlaylist(playlistId);
            playlist = context.Playlists.Remove(playlist);
            context.SaveChanges();
            return playlist ;
        }

        public void DeletePlaylistTrack(long playlistTrackId)
        {
            PlaylistTrack track = context.PlaylistTracks.Single(f => f.Id == playlistTrackId);
            context.PlaylistTracks.Remove(track);
            context.SaveChanges();
        }

        public Track CreateTrack(long playlistId, Track track)
        {
            var playlist = ReadPlaylist(playlistId);
            if (playlist == null) return null;

            var playlistTrack = new PlaylistTrack {Track = track};
            if(playlist.PlaylistTracks == null) playlist.PlaylistTracks = new Collection<PlaylistTrack>();
            else
            {
                if (playlist.PlaylistTracks.Any(f => f.Track.TrackSource.TrackId == track.TrackSource.TrackId)) return null;
            }
            playlist.PlaylistTracks.Add(playlistTrack);

            context.SaveChanges();
            return playlistTrack.Track;
        }

        public void DeleteTrack(long trackId)
        {
            throw new NotImplementedException();
        }

        public void DeleteTrackSource(long trackSourceId)
        {
            throw new NotImplementedException();
        }

        public void DeleteVote(long voteId)
        {
            var vote = context.Votes.Find(voteId);
            context.Votes.Remove(vote);
            context.SaveChanges();
        }

        public Playlist UpdatePlaylist(Playlist playlist, string email)
        {
            var pl = context.Playlists.ToList().Single(p => p.Id == playlist.Id);

            if (email == null)
            {
                pl.PlaylistMasterId = null;
            }
            else
            {
                var user = context.User.Single(u => u.Email == email);
                pl.PlaylistMasterId = user.Id;
            }
            context.Entry(pl).State = EntityState.Modified;
            context.SaveChanges();

            return playlist;

        }


        public IEnumerable<Comment> ReadChatComments(Playlist playlist)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Comment> ReadComments(Playlist playlist)
        {
            throw new NotImplementedException();
        }

        public Playlist ReadPlaylist(string name)
        {
            return context.Playlists.Single(p => p.Name.Equals(name));
        }

        public Playlist ReadPlaylist(long playlistId)
        {
            return context.Playlists
                .Include(p => p.PlaylistTracks)
                .Include("PlaylistTracks.Track.TrackSource")
                .Include("PlaylistTracks.Votes.User")
                .FirstOrDefault(p => p.Id == playlistId);
        }

        public IEnumerable<Playlist> ReadPlaylists()
        {
            return context.Playlists.ToList();
        }

        public IEnumerable<Playlist> ReadPlaylists(Organisation organisation)
        {
            throw new NotImplementedException();
        }

        public PlaylistTrack ReadPlaylistTrack(long playlistTrackId)
        {
            return context.PlaylistTracks.Include(p => p.Votes).Include("Votes.User").FirstOrDefault(p => p.Id == playlistTrackId);
        }

        public IEnumerable<PlaylistTrack> ReadPlaylistTracks(Playlist playlist)
        {
            throw new NotImplementedException();
        }

        public Track ReadTrack(long trackId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Track> ReadTracks()
        {
            throw new NotImplementedException();
        }

        public TrackSource ReadTrackSource(long trackSourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TrackSource> ReadTrackSources()
        {
            throw new NotImplementedException();
        }

        public Vote ReadVote(long voteId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Vote> ReadVotesForPlaylist(Playlist playlist)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Vote> ReadVotesUser(User user)
        {
            return context.Votes.Where(v => v.User == user);
        }

        public Comment UpdateComment(Comment comment)
        {
            throw new NotImplementedException();
        }

        public Playlist UpdatePlaylist(Playlist playlist)
        {
            context.Playlists.AddOrUpdate(playlist);
            context.Entry(playlist).State = EntityState.Modified;
            context.SaveChanges();
            return playlist;
        }

        public PlaylistTrack UpdatePlayListTrack(PlaylistTrack playlistTrack)
        {
            if (context.PlaylistTracks.Find(playlistTrack.Id) == null) return null;

            context.PlaylistTracks.AddOrUpdate(playlistTrack);
            context.Entry(playlistTrack).State = EntityState.Modified;
            
            context.SaveChanges();

            return playlistTrack;
        }

        public Track UpdateTrack(Track track)
        {
            throw new NotImplementedException();
        }

        public TrackSource UpdateTracksource(TrackSource trackSource)
        {
            throw new NotImplementedException();
        }

        public Vote UpdateVote(Vote vote)
        {
            throw new NotImplementedException();
        }
    }
}
