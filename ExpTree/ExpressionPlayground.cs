using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExpTree
{
    public class ExpressionPlayground
    {

        class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString()
            {
                return $"Id: {Id}. Name: {Name}";
            }
        }
        public static void Run()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine();
            Exp4();
            Console.WriteLine();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        static Expression<Func<User, User>> Exp1()
        {
            // user => new User(){ Id = user.Id + 10, Name = user.Name + " changed!"

            // user
            var parameterExp = Expression.Parameter(typeof(User));

            // new User()
            var newExp = Expression.New(typeof(User));

            // user
            ParameterExpression userParam = Expression.Parameter(typeof(User), "user");
            // user.Id 
            MemberExpression idProperty = Expression.Property(userParam, "Id");
            // + 10
            BinaryExpression addExpression = Expression.Add(idProperty, Expression.Constant(10));
            // Id = user.Id + 10
            MemberBinding idBinding = Expression.Bind(typeof(User).GetProperty("Id"), addExpression);

            // user.Name
            MemberExpression nameProperty = Expression.Property(userParam, "Name");
            // + "Changed!"
            BinaryExpression nameAddExpression = Expression.Add(nameProperty, Expression.Constant("Changed!"),
                typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }));

            // Name = user.Name + "Changed!"
            MemberBinding nameBinding = Expression.Bind(typeof(User).GetProperty("Name"), nameAddExpression);

            // new User(){ Id = user.Id + 10 , Name = user.Name + "Changed!" }
            MemberInitExpression memberInit = Expression.MemberInit(Expression.New(typeof(User)), idBinding, nameBinding);

            // user => new User(){ Id = user.Id + 10 }
            Expression<Func<User, User>> lambdaExpression = Expression.Lambda<Func<User, User>>(memberInit, userParam);

            return lambdaExpression;

        }

        static Expression<Func<User, ExpandoObject>> Exp2()
        {
            // user => new { Id = user.Id + 10, Name = user.Name + " changed!"

            // user
            var parameterExp = Expression.Parameter(typeof(User));

            // new User()
            var newExp = Expression.New(typeof(User));

            // user
            ParameterExpression userParam = Expression.Parameter(typeof(User), "user");
            // user.Id 
            MemberExpression idProperty = Expression.Property(userParam, "Id");
            // + 10
            BinaryExpression addExpression = Expression.Add(idProperty, Expression.Constant(10));
            // Id = user.Id + 10
            MemberBinding idBinding = Expression.Bind(typeof(User).GetProperty("Id"), addExpression);

            // user.Name
            MemberExpression nameProperty = Expression.Property(userParam, "Name");
            // + "Changed!"
            BinaryExpression nameAddExpression = Expression.Add(nameProperty, Expression.Constant("Changed!"),
                typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }));

            // Name = user.Name + "Changed!"
            MemberBinding nameBinding = Expression.Bind(typeof(User).GetProperty("Name"), nameAddExpression);

            // new { Id = user.Id + 10 , Name = user.Name + "Changed!" }
            MemberInitExpression memberInit = Expression.MemberInit(Expression.New(typeof(ExpandoObject)), idBinding, nameBinding);

            // user => new User(){ Id = user.Id + 10 }
            Expression<Func<User, ExpandoObject>> lambdaExpression = Expression.Lambda<Func<User, ExpandoObject>>(memberInit, userParam);

            return lambdaExpression;

        }

        static void Exp3()
        {
            User user = new User { Name = "John" };

            ParameterExpression userParam = Expression.Parameter(typeof(User), "user");
            MemberExpression nameProperty = Expression.Property(userParam, "Name");
            BinaryExpression concatExpression = Expression.Add(nameProperty, Expression.Constant(" holy!"),
    typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }));


            Type anonType = CreateAnonymousType<string, string>("SelectedProp", "TypeOfProp");
            var anon = Activator.CreateInstance(anonType);
            NewExpression creationExpression = Expression.New(anonType.GetConstructor(Type.EmptyTypes));

            //"SelectProp = item.name"
            MemberBinding assignment1 = Expression.Bind(anonType.GetField("SelectedProp"), concatExpression);
            //"TypeOfProp = item.GetType()"
            MemberBinding assignment2 = Expression.Bind(anonType.GetField("TypeOfProp"), nameProperty);


            var initialization = Expression.MemberInit(creationExpression, assignment1, assignment2);
     
            Expression<Func<User, object>> expression = Expression.Lambda<Func<User, object>>(initialization, userParam);
            var result = expression.Compile().Invoke(new User() { Id = 1, Name = "Bruce" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            
        }

        static Type CreateAnonymousType<TFieldA, TFieldB>(string fieldNameA, string fieldNameB)
        {
            AssemblyName dynamicAssemblyName = new AssemblyName("TempAssembly");
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("TempAssembly");

            TypeBuilder dynamicAnonymousType = dynamicModule.DefineType("AnonymousType", TypeAttributes.Public);

            dynamicAnonymousType.DefineField(fieldNameA, typeof(TFieldA), FieldAttributes.Public);
            dynamicAnonymousType.DefineField(fieldNameB, typeof(TFieldB), FieldAttributes.Public);

            return dynamicAnonymousType.CreateType();
        }

        /// <summary>
        /// 익명타입 생성
        /// </summary>
        static void Exp4()
        {
            User user = new User { Id = 44, Name = "John" };

            ParameterExpression userParam = Expression.Parameter(typeof(User), "user");

            // user.Name + " holy!"
            MemberExpression nameProperty = Expression.Property(userParam, "Name");
            BinaryExpression concatExpression = Expression.Add(nameProperty, Expression.Constant(" holy!"),
                typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }));

            // user.Id * user.Id
            MemberExpression idExpression = Expression.Property(userParam, "Id");
            BinaryExpression mulExpression = Expression.Multiply(idExpression, idExpression);

            // user.Id + 100
            BinaryExpression add100Expression = Expression.Add(idExpression, Expression.Constant(100));

            Type anonymousType = CreateType(new List<KeyValuePair<string, Type>>()
            {
                new KeyValuePair<string, Type>("Prop1", typeof(string)),
                new KeyValuePair<string, Type>("Prop2", typeof(int)),
                new KeyValuePair<string, Type>("Prop3", typeof(int)),
            });
            var anon = Activator.CreateInstance(anonymousType);
            NewExpression creationExpression = Expression.New(anonymousType.GetConstructor(Type.EmptyTypes));

            // Prop1 = 
            MemberBinding assignment1 = Expression.Bind(anonymousType.GetField("Prop1"), concatExpression);
            // Prop2 = 
            MemberBinding assignment2 = Expression.Bind(anonymousType.GetField("Prop2"), mulExpression);
            // Prop3 = 
            MemberBinding assignment3 = Expression.Bind(anonymousType.GetField("Prop3"), add100Expression);

            var initialization = Expression.MemberInit(creationExpression, assignment1, assignment2, assignment3);

            Expression<Func<User, object>> expression = Expression.Lambda<Func<User, object>>(initialization, userParam);
            var result = expression.Compile().Invoke(new User() { Id = 44, Name = "Bruce" });
 
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Console.Write(json);
        }

        static AssemblyName dynamicAssemblyName = new AssemblyName("TempAssembly");
        static AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
        static ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("TempAssembly");

        static TypeBuilder dynamicAnonymousType = dynamicModule.DefineType("AnonymousType", TypeAttributes.Public);

        static Type CreateType(IEnumerable<KeyValuePair<string, Type>> fieldAndTypes)
        {
          

            foreach(var fieldType in fieldAndTypes)
            {
                dynamicAnonymousType.DefineField(fieldType.Key, fieldType.Value, FieldAttributes.Public);
            }

            return dynamicAnonymousType.CreateType();
        }

    }
}
