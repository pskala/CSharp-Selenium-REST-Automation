/**
 * help class for create data to inputs
 **/
using Bogus;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace mySpaceName.Helpers
{
    public class User
    {

        public String FirstName { get; set; }
        public String LastName { get; set; }

        public void SetRandomData()
        {
            var fakeData = new Faker("en");
            FirstName = fakeData.Person.FirstName;
            LastName = fakeData.Person.LastName;
        }
    }
    class Data : IEquatable<Data>
    {
        User user;

        public Data()
        {
            user = new User();
        }
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
        public bool Equals(Data other)
        {
            JObject xpctJSON = JObject.Parse(JsonConvert.SerializeObject(other));
            JObject actJSON = JObject.Parse(JsonConvert.SerializeObject(this));

            return JToken.DeepEquals(xpctJSON, actJSON);
        }
        public String DiffData(Data other)
        {
            var jdp = new JsonDiffPatch();
            JToken diffResult = jdp.Diff(JsonConvert.SerializeObject(this), JsonConvert.SerializeObject(other));
            return diffResult.ToString();
        }
        public void SetRandomData()
        {
            user.SetRandomData();
        }

        public static string[] GetRoles()
        {
            return new[] {
            "Admin",
            "User"};
        }
    }
}
