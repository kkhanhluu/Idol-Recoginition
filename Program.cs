using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json; 
using System.Web;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace FaceAPI
{
    class Program
    {
        public static List<Idol> idols; 
        static void Main()
        {
            //MakeRequest();
            //Console.WriteLine("Hit ENTER to exit...");
            //Console.ReadLine();
            LoadJson();
            foreach (var idol in idols)
            {
                SubmitIdol(idol);
            }
            Console.ReadLine();
        }

        static void SubmitIdolFace(String personId, String faceUrl)
        {
            Console.WriteLine(String.Format("Begin submit image {0} for person id {1}", faceUrl, personId));

            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "d2f305860c3447d585a8f288827d24b2"); 
            var uri = "https://westeurope.api.cognitive.microsoft.com/face/v1.0/persongroups/vav-idols/persons/" + personId + "/persistedFaces";

            HttpResponseMessage response;

            object data = new
            {
                url = faceUrl
            };

            // serialize object to json
            var content = JsonConvert.SerializeObject(data);
            // request body 
            byte[] byteData = Encoding.UTF8.GetBytes(content);
            
            using (var byteContent = new ByteArrayContent(byteData))
            {
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = client.PostAsync(uri, byteContent).Result; 
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("SUCCESS - Submit image " + faceUrl); 
                }
            }
        }
        static void SubmitIdol(Idol idol)
        {
            Console.WriteLine("Start submitting idol " + idol.name);
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "d2f305860c3447d585a8f288827d24b2");

            var uri = "https://westeurope.api.cognitive.microsoft.com/face/v1.0/persongroups/vav-idols/persons";

            HttpResponseMessage response;

            // serialze object to json 
            object data = new
            {
                name = idol.name,
                userData = idol.index
            };
            var content = JsonConvert.SerializeObject(data); 
            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(content);

            using (var byteContent = new ByteArrayContent(byteData))
            {
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = client.PostAsync(uri, byteContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var person =  JsonConvert.DeserializeObject<Person>(response.Content.ReadAsStringAsync().Result);
                    Console.WriteLine(String.Format("SUCCESS - Submit idol {0} - {1}. Person ID: {2}", idol.name, idol.index, person.personId)); 

                    for (var i = 0; i < idol.images.Count; i++)
                    {
                        try
                        {
                            SubmitIdolFace(person.personId, idol.images[i].image);
                            Thread.Sleep(4 * 1000);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message); 
                        }
                    }
                }
            }

        }

        static void LoadJson()
        {
            using (StreamReader reader = new StreamReader("F:\\Informatik\\HTML, CSS and JavaScript\\HTML CSS Project\\Javascript\\Idol\\data.json"))
            {
                string json = reader.ReadToEnd();
                idols = JsonConvert.DeserializeObject<List<Idol>>(json);
            }
        }
    }

    public class Idol
    {
        public int index{ get; set; }
        public String name { get; set; }
        public List<Images> images { get; set; }
    }

    public class Images
    {
        public String thumbnail { get; set; }
        public String image { get; set; }
    }

    public class Person
    {
        public String personId { get; set; }
    }
}
