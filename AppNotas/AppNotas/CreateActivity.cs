using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Net;
using System.IO;
using System.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AppNotas
{
    public class Recipe
    {
        public string name { get; set; }
        public string instructions { get; set; }
    }

    [Activity(Label = "Create Recipe", Theme = "@android:style/Theme.Material.Light")]
    public class CreateActivity : Activity
    {
        HttpClient client;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.CreateLayout);
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            Button createButton = FindViewById<Button>(Resource.Id.createButton2);
            EditText nameText = FindViewById<EditText>(Resource.Id.nameText);
            EditText instText = FindViewById<EditText>(Resource.Id.instText);

            createButton.Click += async (sender, e) =>
            {
                if (!string.IsNullOrEmpty(nameText.Text) && !string.IsNullOrEmpty(instText.Text))
                {
                    Recipe recipe = new Recipe();
                    recipe.name = nameText.Text;
                    recipe.instructions = instText.Text;

                    var uri = new Uri("http://redo-receta.herokuapp.com/recipes/");
                    var json = JsonConvert.SerializeObject(recipe);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;
                    response = await client.PostAsync(uri, content);
                    Android.Widget.Toast.MakeText(this, "Recipe Created!", ToastLength.Short).Show();
                    StartActivity(typeof(MainActivity));
                }
                else
                    Android.Widget.Toast.MakeText(this, "Fill all gaps!", ToastLength.Short).Show();
            };
        }
    }
}