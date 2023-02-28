using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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
        public long? evtId { get; set; }
        public string evtCode { get; set; }
        public string evtDate { get; set; }
        public long? evtHPos { get; set; }
        public long? evtValue { get; set; }
        public bool? isJustified { get; set; }
        public string justifReasonCode { get; set; }
        public string justifReasonDesc { get; set; }
        public List<int?> hoursAbsence { get; set; }

    }


    public class AbsenceEvents
    {
        [JsonPropertyName("events")]
        public List<Absence> Events { get; set; }
    }



    public class Grade
    {
        public long? subjectId { get; set; }
        public string subjectCode { get; set; }
        public string subjectDesc { get; set; }
        public long? evtId { get; set; }
        public string evtCode { get; set; }
        public string evtDate { get; set; }
        public float? decimalValue { get; set; }
        public string displayValue { get; set; }
        public long? displaPos { get; set; }
        public string notesForFamily { get; set; }
        public string color { get; set; }
        public bool canceled { get; set; }
        public bool underlined { get; set; }
        public long? periodPos { get; set; }
        public string periodDesc { get; set; }
        public long? componentPos { get; set; }
        public string componentDesc { get; set; }
        public long? weightFactor { get; set; }
        public long? skillId { get; set; }
        public long? gradeMasterId { get; set; }
        public string skillDesc { get; set; }
        public string skillCode { get; set; }
        public long? skillMasterId { get; set; }
        public string skillValueDesc { get; set; }
        public string skillValueShortDesc { get; set; }
        public long? oldskillId { get; set; }
        public string oldskillDesc { get; set; }

    }

    public class GradesList
    {
        [JsonPropertyName("grades")]
        public List<Grade> Grades { get; set; }
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
        Task<AbsenceEvents> GetAbsences(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/grades2")]
        Task<GradesList> GetGrades(string userId, [Header("Z-Auth-Token")] string token);

    }

    public class CvUtils
    {
        public string GetCode(string userId)
        {
            return Regex.Replace(userId, @"[A-Za-z]+", "");

        }


    }

}
