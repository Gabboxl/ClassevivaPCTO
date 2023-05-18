﻿using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ClassevivaPCTO.Utils
{

    public static class Endpoint
    {
        public const string Official = "https://web.spaggiari.eu/rest/v1";
        public const string Test = "https://stoplight.io/mocks/gabboxl/classeviva/155337827";

        public static string CurrentEndpoint { get; set; }
    }

    public class CvError
    {
        public long statusCode { get; set; }
        public string error { get; set; }
        public string message { get; set; }
    }


    public class LoginData
    {
        [AliasAs("uid")]
        public string Uid { get; set; }

        [AliasAs("pass")]
        public string Pass { get; set; }

        [AliasAs("ident")]
        public string Ident { get; set; }

    }

    public enum AbsenceEventCode
    {
        [ApiValue("A", "Assenza")]
        ABA0, //assenza

        [ApiValue("R", "Ritardo")]
        ABR0, //ritardo

        [ApiValue("Rb", "Ritardo breve")]
        ABR1, //ritardo breve

        [ApiValue("U", "Uscita anticipata")]
        ABU0 //uscita anticipata
    }

    public class AbsenceEvent
    {
        public long? evtId { get; set; }
        public AbsenceEventCode evtCode { get; set; }
        public DateTime evtDate { get; set; }
        public long? evtHPos { get; set; }
        public long? evtValue { get; set; }
        public bool isJustified { get; set; }
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

    public class LoginChoice
    {
        public string cid { get; set; }
        public string ident { get; set; }
        public string name { get; set; }
        public string school { get; set; }
    }

    public class LoginResultChoices
    {
        public string requestedAction { get; set; }

        [JsonPropertyName("choices")]
        public List<LoginChoice> choices { get; set; }

        public string message { get; set; }
    }

    public class LoginResultComplete
    {
        public string ident { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string token { get; set; }
        public bool showPwdChangeReminder { get; set; }

        [JsonProperty("release")]
        public DateTime release { get; set; }

        [JsonProperty("expire")]
        public DateTime expire { get; set; }
    }


    public class Lesson
    {
        public long? evtId { get; set; }
        public DateTime evtDate { get; set; }
        public string evtCode { get; set; }
        public long? evtHPos { get; set; }
        public long? evtDuration { get; set; }
        public string classDesc { get; set; }
        public string authorName { get; set; }
        public long? subjectId { get; set; }
        public string subjectCode { get; set; }
        public string subjectDesc { get; set; }
        public string lessonType { get; set; }
        public string lessonArg { get; set; }

    }

    public class AgendaEvent
    {
        public int evtId { get; set; }
        public string evtCode { get; set; }
        public DateTime evtDatetimeBegin { get; set; }
        public DateTime evtDatetimeEnd { get; set; }
        public bool isFullDay { get; set; }
        public string notes { get; set; }
        public string authorName { get; set; }
        public string classDesc { get; set; }
        public int? subjectId { get; set; }
        public string subjectDesc { get; set; }
        public int? homeworkId { get; set; }
    }



    public class OverviewResult
    {
        [JsonPropertyName("lessons")]
        public List<Lesson> Lessons { get; set; }

        [JsonPropertyName("agenda")]
        public List<AgendaEvent> AgendaEvents { get; set; }

        [JsonPropertyName("events")]
        public List<AbsenceEvent> AbsenceEvents { get; set; }

        [JsonPropertyName("grades")]
        public List<Grade> Grades { get; set; }

        [JsonPropertyName("notes")]
        public NotesResult NotesResult { get; set; }
    }


    public class SingleCardResult
    {
        [JsonPropertyName("card")]
        public Card Card { get; set; }
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

    public class Teacher
    {
        public string teacherId { get; set; }
        public string teacherName { get; set; }
    }

    public class Subject
    {
        public long id { get; set; }
        public string description { get; set; }
        public long order { get; set; }
        public List<Teacher> teachers { get; set; }
    }

    public class NoticeAttachment
    {
        public string fileName { get; set; }
        public int attachNum { get; set; }
    }

    public class Notice
    {
        public int pubId { get; set; }
        public DateTime pubDT { get; set; }
        public bool readStatus { get; set; }
        public string evtCode { get; set; }
        public int cntId { get; set; }
        public DateTime cntValidFrom { get; set; }
        public DateTime cntValidTo { get; set; }
        public bool cntValidInRange { get; set; }
        public string cntStatus { get; set; }
        public string cntTitle { get; set; }
        public string cntCategory { get; set; }
        public bool cntHasChanged { get; set; }
        public bool cntHasAttach { get; set; }
        public bool needJoin { get; set; }
        public bool needReply { get; set; }
        public bool needFile { get; set; }
        public bool needSign { get; set; }
        public string evento_id { get; set; }
        public string dinsert_allegato { get; set; }
        public List<NoticeAttachment> attachments { get; set; }
    }

    public partial class NoticeReadResult
    {
        public NoticeReadItem item { get; set; }

        public NoticeReadReply reply { get; set; }
    }

    public partial class NoticeReadItem
    {
        public string title { get; set; }

        public string text { get; set; }
    }

    public partial class NoticeReadReply
    {
        public bool replJoin { get; set; }

        public bool? replText { get; set; }

        public bool? replFile { get; set; }

        public bool? replSign { get; set; }
    }


    //notes

    public enum NoteEventCode
    {
        [ApiValue("", "Annotazione")]
        NTTE,

        [ApiValue("", "Nota disciplinare")]
        NTCL,

        [ApiValue("", "Richiamo")]
        NTWN,

        [ApiValue("", "Sanzione disciplinare")]
        NTST
    }

    public class Note
    {
        public long evtId { get; set; }
        public string evtText { get; set; }
        public DateTime evtDate { get; set; }
        public string authorName { get; set; }
        public bool readStatus { get; set; }

        public NoteEventCode? evtCode { get; set; }
        public DateTime? evtBegin { get; set; }
        public DateTime? evtEnd { get; set; }
        public bool? toUpload { get; set; }
        public string? warningType { get; set; }
    }

    public class NotesResult
    {
        [JsonPropertyName("NTTE")] //Annotazioni
        public List<Note> NotesNTTE { get; set; }

        [JsonPropertyName("NTCL")] //Note disciplinari
        public List<Note> NotesNTCL { get; set; }

        [JsonPropertyName("NTWN")] //Richiami
        public List<Note> NotesNTWN { get; set; }

        [JsonPropertyName("NTST")] //Sanzioni disciplinari
        public List<Note> NotesNTST { get; set; }
    }

    public class ReadNoteResult
    {
        [JsonPropertyName("event")]
        public Note Note { get; set; }
    }

    public class PeriodsResult
    {
        [JsonPropertyName("periods")]
        public List<Period> Periods { get; set; }
    }

    public class SubjectsResult
    {
        [JsonPropertyName("subjects")]
        public List<Subject> Subjects { get; set; }
    }

    public class LessonsResult
    {
        [JsonPropertyName("lessons")]
        public List<Lesson> Lessons { get; set; }
    }

    public class NoticeboardResult
    {
        [JsonPropertyName("items")]
        public List<Notice> Notices { get; set; }
    }

    //scrutini

    public class ScrutiniSchoolReport
    {
        public string desc { get; set; }
        public string confirmLink { get; set; }
        public string viewLink { get; set; }
    }

    public class ScrutiniDocument
    {
        public string hash { get; set; }
        public string desc { get; set; }

        //this value is not present in the API response, but it's added by us later
        public ScrutiniDocumentCheckStatus checkStatus { get; set; }
    }

    public class ScrutiniDocumentsResult
    {
        [JsonPropertyName("documents")]
        public List<ScrutiniDocument> Documents { get; set; }

        [JsonPropertyName("schoolReports")]
        public List<ScrutiniSchoolReport> SchoolReports { get; set; }
    }

    public class ScrutiniDocumentCheckStatus
    {
        public bool available { get; set; }
    }

    public class ScrutiniCheckResult
    {
        public ScrutiniDocumentCheckStatus document { get; set; }
    }


    //calendar

    public enum DayStatus
    {
        SD, //school day

        ND, // non school day

        HD, //holiday

        NW, // no work day

        US //unknown status
    }

    public class CalendarDay
    {
        public DateTime dayDate { get; set; }
        public int dayOfWeek { get; set; }
        public DayStatus dayStatus { get; set; }
    }

    public class CalendarResult {

        [JsonPropertyName("calendar")]
        public List<CalendarDay> CalendarDays { get; set; }
    }

}
