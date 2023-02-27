using Newtonsoft.Json;
using Refit;
using System;
using System.Threading.Tasks;

namespace ClassevivaPCTO.Utils
{
    public class LoginData
    {

        [AliasAs("uid")]
        public string Uid { get; set; }

        [AliasAs("pass")]
        public string Pass { get; set; }

    }


    public class LoginResult
    {
        public string Ident { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Token { get; set; }

        [JsonProperty("release")]
        public DateTime Release { get; set; }

        [JsonProperty("expire")]
        public DateTime Expire { get; set; }
    }


    [Headers("User-Agent: zorro/1.0", "Z-Dev-Apikey: +zorro+", "Content-Type: application/json")]
    public interface IClassevivaAPI
    {

        [Post("/auth/login/")]
        Task<LoginResult> LoginAsync([Body(BodySerializationMethod.Serialized)] LoginData logdata);

        [Post("/auth/login/")]
        Task<LoginResult> GetAgenda([Body(BodySerializationMethod.Serialized)] LoginData logdata);
    }

}
