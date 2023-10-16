using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClassevivaPCTO.Utils;
namespace ClassevivaPCTO.Deserializers
{
    public class NoteDeserializerNet : JsonConverter<List<Note>>
    {
        public override bool CanConvert(Type objectType)
        {
            //if object type is List<Note> return true
            return objectType == typeof(List<Note>);
        }

        public override List<Note> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<Note> notesList = new();
        
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            JsonElement root = document.RootElement;

            foreach (JsonProperty entry in root.EnumerateObject())
            {
                NoteEventCode evtCode = Enum.Parse<NoteEventCode>(entry.Name);
                JsonElement jsonArray = entry.Value;

                foreach (JsonElement jsonElement in jsonArray.EnumerateArray())
                {
                    Note note = jsonElement.Deserialize<Note>(options);
                    note.evtCode = evtCode;
                    notesList.Add(note);
                }
            }

            return notesList;
        }

        public override void Write(Utf8JsonWriter writer, List<Note> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}