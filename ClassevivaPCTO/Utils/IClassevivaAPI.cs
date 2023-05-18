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

        [Get("/students/{userId}/absences/details/{startDate}/{endDate}")]
        Task<AbsencesResult> GetAbsencesBetweenDates(string userId, string startDate, string endDate, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/grades2")]
        Task<Grades2Result> GetGrades(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/overview/all/{startDate}/{endDate}")]
        Task<OverviewResult> GetOverview(string userId, string startDate, string endDate, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/cards")]
        Task<CardsResult> GetCards(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/card")]
        Task<SingleCardResult> GetCardSingle(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/periods")]
        Task<PeriodsResult> GetPeriods(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/subjects")]
        Task<SubjectsResult> GetSubjects(string userId, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/notes/all")]
        Task<ApiResponse<string>> GetAllNotes(string userId, [Header("Z-Auth-Token")] string token);

        [Post("/students/{userId}/notes/{evtCode}/read/{evtId}")]
        Task<ReadNoteResult> ReadNote(string userId, string evtCode, string evtId, [Header("Z-Auth-Token")] string token);

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

        [Post("/students/{userId}/noticeboard/read/{noticeEvtCode}/{noticePubId}/101")]
        Task<NoticeReadResult> ReadNotice(string userId, string noticePubId, string noticeEvtCode, [Header("Z-Auth-Token")] string token);

        [Post("/students/{userId}/documents")]
        Task<ScrutiniDocumentsResult> GetScrutiniDocuments(string userId, [Header("Z-Auth-Token")] string token);

        [Post("/students/{userId}/documents/check/{documentHash}")]
        Task<ScrutiniCheckResult> CheckScrutinioDocument(string userId, string documentHash, [Header("Z-Auth-Token")] string token);

        [Post("/students/{userId}/documents/read/{documentHash}")]
        Task<HttpResponseMessage> GetScrutinioDocumentFile(string userId, string documentHash, [Header("Z-Auth-Token")] string token);

        [Get("/students/{userId}/calendar/all")]
        Task<CalendarResult> GetCalendar(string userId, [Header("Z-Auth-Token")] string token);
    }

}
