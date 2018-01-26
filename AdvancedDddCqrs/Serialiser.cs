using System;

using Newtonsoft.Json;

namespace AdvancedDddCqrs
{
    public class MementoSerialiser
    {
        public TType Deserialise<TType, TMementoType>(string json)
        {
            object dynamicObject = JsonConvert.DeserializeObject(json);
            var memento = (TMementoType)Activator.CreateInstance(typeof(TMementoType), dynamicObject);
            var result = (TType)Activator.CreateInstance(typeof(TType), new[] { memento });
            return result;
        }

        public string Serialise<TType, TMementoType>(TType item)
            where TType : ISupportMemoisation<TMementoType>
        {
            TMementoType memento = item.GetMemento();
            string json = JsonConvert.SerializeObject(memento, Formatting.Indented);
            return json;
        }
    }

    public class Serialiser
    {
        public TType Deserialise<TType>(string json)
            where TType : IDynamicWrapper
        {
            object dynamicObject = JsonConvert.DeserializeObject(json);
            var result = (TType)Activator.CreateInstance(typeof(TType), dynamicObject);
            return result;
        }

        public string Serialise<TType>(TType item)
        {
            string json = JsonConvert.SerializeObject(item, Formatting.Indented);
            return json;
        }
    }
}
