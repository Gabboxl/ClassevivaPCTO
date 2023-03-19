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

    public class AbsenceEvent
    {
        public long? evtId { get; set; }
        public string evtCode { get; set; }
        public DateTime evtDate { get; set; }
        public long? evtHPos { get; set; }
        public long? evtValue { get; set; }
        public bool? isJustified { get; set; }
        public string justifReasonCode { get; set; }
        public string justifReasonDesc { get; set; }
        public List<int?> hoursAbsence { get; set; }

    }


    public class AbsencesResult
    {
        [JsonPropertyName("events")]
        public List<AbsenceEvent> AbsenceEvents { get; set; }
    }



    public class Grade
    {
        public long? subjectId { get; set; }
        public string subjectCode { get; set; }
        public string subjectDesc { get; set; }
        public long? evtId { get; set; }
        public string evtCode { get; set; }
        public DateTime evtDate { get; set; }
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

    public class Grades2Result
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


    public class Lesson
    {
        public long? evtId { get; set; }
        public DateTime evtDate { get; set; }
        public string evtCode { get; set; }
        public long? evtHPos { get; set; }
        public string evtDuration { get; set; }
        public string classDesc { get; set; }
        public string authorName { get; set; }
        public long? subjectId { get; set; }
        public string subjectCode { get; set; }
        public string subjectDesc { get; set; }
        public string lessonType { get; set; }
        public string lessonArg { get; set; }

    }


    public class OverviewResult
    {
        [JsonPropertyName("lessons")]
        public List<Lesson> Lessons { get; set; }

        [JsonPropertyName("grades")]
        public List<Grade> Grades { get; set; }

        [JsonPropertyName("events")]
        public List<AbsenceEvent> AbsenceEvents { get; set; }

        //to implement notes array json of the API response
    }



    public class Card
    {
        public string ident { get; set; }
        public string usrType { get; set; }
        public long? usrId { get; set; }
        public string miurSchoolCode { get; set; }
        public string miurDivisionCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime birthDate { get; set; }
        public string fiscalCode { get; set; }
        public string schCode { get; set; }
        public string schName { get; set; }
        public string schDedication { get; set; }
        public string schCity { get; set; }
        public string schProv { get; set; }

    }


    public class CardsResult
    {
        [JsonPropertyName("cards")]
        public List<Card> Cards { get; set; }
    }



    public class Period
    {
        public string periodCode { get; set; }
        public long? periodPos { get; set; }
        public string periodDesc { get; set; }
        public bool isFinal { get; set; }
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public string miurDivisionCode { get; set; }
    }


    public class PeriodsResult
    {
        [JsonPropertyName("periods")]
        public List<Period> Periods { get; set; }
    }


    [Headers("User-Agent: CVVS/std/4.2.3 Android/10", "Z-Dev-Apikey: Tg1NWEwNGIgIC0K", "Content-Type: application/json")]
    public interface IClassevivaAPI
    {
        [Post("/auth/login/")]
        Task<LoginResult> LoginAsync([Body(BodySerializationMethod.Serialized)] LoginData logdata);

        [Get("/students/{userId}/absences/details")]
        Task<AbsencesResult> GetAbsences(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/grades2")]
        Task<Grades2Result> GetGrades(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/overview/all/{startDate}/{endDate}")]
        Task<OverviewResult> GetOverview(string userId, string startDate, string endDate, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/cards")]
        Task<CardsResult> GetCards(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/periods")]
        Task<PeriodsResult> GetPeriods(string userId, [Header("Z-Auth-Token")] string token);

    }

    public class CvUtils
    {
        public string GetCode(string userId)
        {
            return Regex.Replace(userId, @"[A-Za-z]+", "");

        }


    }

}
