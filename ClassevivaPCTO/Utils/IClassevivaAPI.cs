using Refit;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassevivaPCTO.Utils
{
    [Headers("User-Agent: CVVS/std/4.2.3 Android/10", "Z-Dev-Apikey: Tg1NWEwNGIgIC0K", "Content-Type: application/json")]
    public interface IClassevivaAPI
    {
        [Post("/auth/login")]
        Task<HttpResponseMessage> LoginAsync([Body(BodySerializationMethod.Serialized)] LoginData logdata);

        [Get("/students/{userId}/absences/details")]
        Task<AbsencesResult> GetAbsences(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/grades2")]
        Task<Grades2Result> GetGrades(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/overview/all/{startDate}/{endDate}")]
        Task<OverviewResult> GetOverview(string userId, string startDate, string endDate, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/cards")]
        Task<CardsResult> GetCards(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/card")]
        Task<Card> GetCardSingle(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/periods")]
        Task<PeriodsResult> GetPeriods(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/subjects")]
        Task<SubjectsResult> GetSubjects(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/notes/all")]
        Task<Notes> GetAllNotes(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/lessons/{startDate}/{endDate}")]
        Task<LessonsResult> GetLessons(string userId, string startDate, string endDate, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/lessons/today")]
        Task<LessonsResult> GetTodayLessons(string userId, string dayDate, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/noticeboard")]
        Task<NoticeboardResult> GetNotices(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/agenda/all/{startDate}/{endDate}")]
        Task<List<AgendaEvent>> GetAgendaEvents(string userId, string startDate, string endDate, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/noticeboard/attach/{noticeEvtCode}/{noticePubId}/{attachmentNumber}")]
        Task<HttpResponseMessage> GetNoticeAttachment(string userId, string noticePubId, string noticeEvtCode, string attachmentNumber, [Header("Z-Auth-Token")] string token);
    }

}
