using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Net.Cache;
using System.Reflection;

namespace ExpTree
{
    public class DynamicSelect
    {
        class User
        {

            public long Id { get; set; }
            public string Name { get; set; }
            public DateTime? Created { get; set; }
            public int VipValue { get; set; }
            public string Type { get; set; }

            static string[] types = new string[] { "Male", "Female", "3rd" };
            public User(string name)
            {
                Name = name;
                Created = new DateTime(2023, new Random(Guid.NewGuid().GetHashCode()).Next(1, 12), 1);
                VipValue = new Random(Guid.NewGuid().GetHashCode()).Next(0, 100);

                Type = types[new Random(Guid.NewGuid().GetHashCode()).Next(0, 3)];
            }
        }

        static List<User> Users
        {
            get
            {
                return Enumerable.Range(0, 50).Select(x => new User(Guid.NewGuid().ToString())).ToList();
            }
        }
        static void Demo1(int? maxMonth)
        {
            int lastMonth = maxMonth.HasValue ? maxMonth.Value : 12;

            var range = Enumerable.Range(1, lastMonth);
            string selectText = "new (Key AS Type";
            for (int month = 1; month <= lastMonth; month++)
            {
                selectText += ", ";
                selectText += $"Count(x => x.Month == {month}) AS M{month}";
            }
            selectText += ")";

            for (int i = 1; i <= lastMonth; i++)
            {

            }
            var r = Users.AsQueryable().Where(x => x.Created.HasValue && x.Type != null)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Month = x.Created.Value.Month,
                    Vip = x.VipValue,
                    Type = x.Type
                })
                .GroupBy(x => x.Type)
                .Select(selectText)
                .ToDynamicList();

            var item = r.FirstOrDefault();
            dynamic d = new
            {
                Age = 34
            };
            var pp = ((object)d).GetType().GetProperties();
            var props = ((object)item).GetType().GetProperties().Where(x=> x.DeclaringType != typeof(DynamicClass));
            foreach (var property in props)
                Console.WriteLine(property.Name);
            foreach(var i in r)
            {
                Console.WriteLine(i);
            }
        }

        public static void Run()
        {
            Demo1(12);
        }
    }
}
