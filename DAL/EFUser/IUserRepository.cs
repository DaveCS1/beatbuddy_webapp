﻿using BB.BL.Domain.Organisations;
using BB.BL.Domain.Users;
using System.Collections.Generic;

namespace BB.DAL.EFUser
{
    public interface IUserRepository
    {
        //User
        User CreateUser(User user);
        User UpdateUser(User user);
        User ReadUser(long userId);
        User ReadUser(string email);
        User ReadUser(string lastname, string firstname);
        IEnumerable<User> ReadUsers();
        User ReadOrganiserFromOrganisation(Organisation organisation);
        void DeleteUser(long userId);

        //UserRole
        IEnumerable<UserRole> ReadUserRolesForOrganisation(Organisation organisation);
        IEnumerable<UserRole> ReadOrganisationsForUser(long userId);
        UserRole CreateUserRole(long userId, long organisationId, Role role);
        UserRole CreateUserRole(UserRole userRole);
    }
}
