using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
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

    public class Absence
    {
        public int evtId { get; set; }
        public string evtCode { get; set; }
        public DateTime evtDate { get; set; }
        public int evtHPos { get; set; }
        public int evtValue { get; set; }
        public bool isJustified { get; set; }
        public string justifReasonCode { get; set; }
        public string justifReasonDesc { get; set; }
        public List<int> hoursAbsence { get; set; }

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

        [Get("/students/{userId}/absences/details")]
        Task<List<Absence>> GetAbsences(string userId);
    }

}
