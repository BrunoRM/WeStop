using System;
using WeStop.Api.Domain;

namespace WeStop.UnitTest.Helpers
{
    public static class TestUsers
    {
        public static User Dustin = new User(Guid.NewGuid(), "Dustin");
        public static User Mike = new User(Guid.NewGuid(), "Mike");
        public static User Lucas = new User(Guid.NewGuid(), "Lucas");
        public static User Will = new User(Guid.NewGuid(), "Will");
    }
}