using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using ClassevivaPCTO.Utils;
namespace ClassevivaPCTO.Deserializers
{
    public class NoteDeserializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<Note>);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var notesList = new List<Note>();
            var jObject = JObject.Load(reader);
            foreach (var property in jObject.Properties())
            {
                var evtCode = Enum.Parse<NoteEventCode>(property.Name);
                var jsonArray = JArray.Parse(property.Value.ToString());
                foreach (var jsonElement in jsonArray)
                {
                    var note = jsonElement.ToObject<Note>();
                    note.evtCode = evtCode;
                    notesList.Add(note);
                }
            }
            return notesList;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}