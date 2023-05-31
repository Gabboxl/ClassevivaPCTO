using System;
using System.Collections.Generic;
using ClassevivaPCTO.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ClassevivaPCTO.Deserializers
{
    public class NoteDeserializer : JsonConverter<List<Note>>
    {
        public override List<Note> ReadJson(JsonReader reader, Type objectType, List<Note> existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            List<Note> notesList = new List<Note>();
            JObject jsonObject = JObject.Load(reader);

            foreach (KeyValuePair<string, JToken> entry in jsonObject)
            {
                JArray jsonArray = (JArray)entry.Value;
                foreach (JToken jsonElement in jsonArray)
                {
                    Note note = jsonElement.ToObject<Note>(serializer);
                    note.evtCode = Enum.Parse<NoteEventCode>(entry.Key);
                    notesList.Add(note);
                }
            }

            return notesList;
        }

        public override void WriteJson(JsonWriter writer, List<Note> value, JsonSerializer serializer)
        {
            throw new NotImplementedException("WriteJson is not implemented for this converter.");
        }
    }
}