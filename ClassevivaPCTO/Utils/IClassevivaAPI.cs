using System;
using Refit;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassevivaPCTO.Utils
{
    //Add a new attribute MarkRequest without authentication
    [AttributeUsage(AttributeTargets.Method, Inherited = false)] public class NoAuth : Attribute { }

    [Headers("User-Agent: CVVS/std/4.2.3 Android/10", "Z-Dev-Apikey: Tg1NWEwNGIgIC0K", "Content-Type: application/json")]
    public interface IClassevivaAPI
    {
        //TODO: add mark in some way to mark the request that doesnt need authentication

        [NoAuth]
        [Post("/auth/login")]
        Task<HttpResponseMessage> LoginAsync([Body(BodySerializationMethod.Serialized)] LoginData logdata);

        [Get("/students/{userId}/absences/details")]
        Task<AbsencesResult> GetAbsences(string userId);

        [Get("/students/{userId}/absences/details/{startDate}/{endDate}")]
        Task<AbsencesResult> GetAbsencesBetweenDates(string userId, string startDate, string endDate);

        [Get("/students/{userId}/grades2")]
        Task<Grades2Result> GetGrades(string userId);

        [Get("/students/{userId}/overview/all/{startDate}/{endDate}")]
        Task<OverviewResult> GetOverview(string userId, string startDate, string endDate);

        [Get("/students/{userId}/cards")]
        Task<CardsResult> GetCards(string userId);

        [Get("/students/{userId}/card")]
        Task<SingleCardResult> GetCardSingle(string userId);

        [Get("/students/{userId}/periods")]
        Task<PeriodsResult> GetPeriods(string userId);

        [Get("/students/{userId}/subjects")]
        Task<SubjectsResult> GetSubjects(string userId);

        [Get("/students/{userId}/notes/all")]
        Task<List<Note>> GetAllNotes(string userId);

        [Post("/students/{userId}/notes/{evtCode}/read/{evtId}")]
        Task<ReadNoteResult> ReadNote(string userId, string evtCode, string evtId);

        [Get("/students/{userId}/lessons/{startDate}/{endDate}")]
        Task<LessonsResult> GetLessons(string userId, string startDate, string endDate);

        [Get("/students/{userId}/lessons/today")]
        Task<LessonsResult> GetTodayLessons(string userId, string dayDate);

        [Get("/students/{userId}/noticeboard")]
        Task<NoticeboardResult> GetNotices(string userId);

        [Get("/students/{userId}/agenda/all/{startDate}/{endDate}")]
        Task<AgendaResult> GetAgendaEvents(string userId, string startDate, string endDate);

        [Get("/students/{userId}/noticeboard/attach/{noticeEvtCode}/{noticePubId}/{attachmentNumber}")]
        Task<HttpResponseMessage> GetNoticeAttachment(string userId, string noticePubId, string noticeEvtCode, string attachmentNumber);

        [Post("/students/{userId}/noticeboard/read/{noticeEvtCode}/{noticePubId}/101")]
        Task<NoticeReadResult> ReadNotice(string userId, string noticePubId, string noticeEvtCode, [Body(buffered: true, serializationMethod: BodySerializationMethod.Serialized)] NoticeReadSignRequest noticeReadSignRequest);

        [Post("/students/{userId}/documents")]
        Task<ScrutiniDocumentsResult> GetScrutiniDocuments(string userId);

        [Post("/students/{userId}/documents/check/{documentHash}")]
        Task<ScrutiniCheckResult> CheckScrutinioDocument(string userId, string documentHash);

        [Post("/students/{userId}/documents/read/{documentHash}")]
        Task<HttpResponseMessage> GetScrutinioDocumentFile(string userId, string documentHash);

        [Get("/students/{userId}/calendar/all")]
        Task<CalendarResult> GetCalendar(string userId);
    }

}
