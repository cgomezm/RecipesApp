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
    public class Recipes
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class RecipeInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string instructions { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    [Activity(Label = "AppNotas", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.Light")]
    public class MainActivity : Activity
    {
        HttpClient client;
        List<Recipes> names = new List<Recipes>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            Button getButton = FindViewById<Button>(Resource.Id.getButton);
            Button createButton = FindViewById<Button>(Resource.Id.createButton);
            ListView listview = FindViewById<ListView>(Resource.Id.listView1);       

            getButton.Click += async (sender, e) =>
            {
                string url = "http://redo-receta.herokuapp.com/recipes.json";
                
                names =  await GetRecipes(url);
                List<string> rnames = new List<string>();
                foreach (var item in names)
                    rnames.Add(item.name);

                ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, rnames);
                listview.Adapter = adapter;
                
             };
            
            listview.ItemClick += async (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                string s = listview.GetItemAtPosition(e.Position).ToString();
                var f = names.Find(x => x.name == s);
                
                var instructions = await GetRecipeInfo("http://redo-receta.herokuapp.com/recipes/" + f.id + ".json");
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage(s.ToUpperInvariant() + "\n\n" + "Instructions: " + "\n" + instructions);
                callDialog.SetNeutralButton("Delete", async delegate
                {
                    var uri = new Uri("http://redo-receta.herokuapp.com/recipes/" + f.id);

                    var response = await client.DeleteAsync(uri);
                    Android.Widget.Toast.MakeText(this, "Recipe Deleted!", ToastLength.Short).Show();
                    listview.Adapter = null;
                });

                callDialog.SetNegativeButton("Cancel", delegate
                {

                });

                callDialog.Show();                
            };

            createButton.Click += CreateButton_Click;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(CreateActivity));
        }

        private async Task<List<Recipes>> GetRecipes(string url)
        {
            var recipes = new List<Recipes>();            

            var uri = new Uri(url);
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    recipes = JsonConvert.DeserializeObject<List<Recipes>>(content);                    
                }
            }
            catch
            {
                Exception ex;
            }
            
            return recipes;
        }

        private async Task<string> GetRecipeInfo(string url)
        {
            var recipeinfo = new RecipeInfo();

            var uri = new Uri(url);
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    recipeinfo = JsonConvert.DeserializeObject<RecipeInfo>(content);
                }
            }
            catch
            {
                Exception ex;
            }

            return recipeinfo.instructions;
        }
    }
}
