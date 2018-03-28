using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            IMaybe<MyUser> maybeUser = Maybe.Just(new MyUser());
            IMaybe<IEnumerable<UserGroup>> maybeGroups = maybeUser.Select(user => user.Groups);

            // if you need to check if it has value, use this extension method, but probably don't do this often.
            if (maybeGroups.HasValue())
            {
                Console.WriteLine("We have groups");
            }
            else
            {
                Console.WriteLine("No groups");
            }

            // null coalescing equivalent
            IEnumerable<UserGroup> groupsForDisplay = maybeGroups.Or(Enumerable.Empty<UserGroup>());
            foreach (var group in groupsForDisplay)
            {
                Console.WriteLine(group);
            }

            
            // or better yet...
            maybeGroups.Do(groups =>
            {
                Console.WriteLine("We have groups");
                foreach (var group in groups)
                {
                    Console.WriteLine(group);
                }
            }, () => Console.WriteLine("No groups"));


            // or maybe you only want to do something if you have a value
            maybeGroups.Do(groups =>
            {
                foreach (var group in groups)
                {
                    Console.WriteLine(group);
                }
            });

            maybeUser
                .If(u => u.Name.Length < 5)
                .Do(user => Console.WriteLine($"User {user.Name} has a short name"));
        }
    }

    public class MyUser
    {
        public string Name => "Foo";
        public IEnumerable<UserGroup> Groups { get { yield return new UserGroup(); } } 
    }

    public class UserGroup
    {
    }
}
