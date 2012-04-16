using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class Value
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
    public class ValuesController : ApiController
    {
        private static SortedDictionary<int, string> values = new SortedDictionary<int, string>()
                                                                  {
                                                                      {1, "Hello"},
                                                                      {2, "world"},
                                                                      {666,string.Join("", Enumerable.Repeat("long", 100)) }
                                                                  };
        public IEnumerable<Value> Get()
        {
            return values.Select(v => new Value() {Id = v.Key, Text = v.Value } );
        }

        public Value Get(int id)
        {
            string text;
            if(values.TryGetValue(id, out text))
            {
                return new Value(){ Id = id, Text = text};
            }
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        public void Post(string text)
        {
            lock (values)
            {
                values.Add(values.Last().Key + 1, text);
            }
        } 
        public void Put(int id, string text)
        {
            lock (values)
            {
                if (!values.ContainsKey(id))
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                values[id] = text;
            }
        }

        public void Delete(int id)
        {
            lock (values)
            {
                if (!values.Remove(id))
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
            }
        }
    }
}