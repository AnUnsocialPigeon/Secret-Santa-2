using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secret_Santa_2 {
    class Program {
        // Var
        public static string[] NameList;
        public static Dictionary<string, string> Dislikes;
        public static Dictionary<string, string> Likes;
        public static List<string[]> AvoidanceJson;
        public static Dictionary<string, string[]> Avoidance = new Dictionary<string, string[]>();

        public static string nameListDir = Directory.GetCurrentDirectory() + @"\namelist.json";
        public static string dislikesDir = Directory.GetCurrentDirectory() + @"\dislikes.json";
        public static string likesDir = Directory.GetCurrentDirectory() + @"\likes.json";
        public static string avoidDir = Directory.GetCurrentDirectory() + @"\avoid.json";
        public static string saveDir = Directory.GetCurrentDirectory() + @"\Final.txt";
        public static Dictionary<string, string> matches;
        public static List<string> remain = new List<string>();

        public static Random rnd = new Random();
        public static int seed;
        public static int Population;

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {
            // File shit
            ExistCheck(nameListDir);
            ExistCheck(dislikesDir);
            ExistCheck(likesDir);
            ExistCheck(avoidDir);

            NameList = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(nameListDir));
            Dislikes = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(nameListDir)).ToDictionary<string, string>(x => x);
            Likes = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(nameListDir)).ToDictionary<string, string>(x => x);
            string[] DislikesJson = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(dislikesDir));
            string[] LikesJson = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(likesDir));

            // setting up the dictionaries properly, as the key and value are the same item.
            for (int i = 0; i < NameList.Length; i++) {
                Dislikes[NameList[i]] = DislikesJson[i];
                Likes[NameList[i]] = LikesJson[i];
            }

            AvoidanceJson = JsonConvert.DeserializeObject<List<string[]>>(File.ReadAllText(avoidDir));

            int n = 0;
            foreach (string key in NameList) {
                try {
                    Avoidance.Add(key, AvoidanceJson[n]);
                }
                catch { Avoidance.Add(key, new string[] { "" }); }
                n++;
            }

            // setting up all other vars
            Population = NameList.Length;
            matches = NameList.ToDictionary(x => x);
            foreach (string match in NameList) {
                matches[match] = "";
            }

            //Console.WriteLine(Directory.GetCurrentDirectory() + "\n" + matches);

            // seed setting
            string askSeed = Ask("Seed (blank for random): ");
            seed = new Random().Next();
            if (askSeed != "" && int.TryParse(askSeed, out int a)) seed = a;
            rnd = new Random(seed);

            Console.WriteLine("Creating List of seed " + seed);


            // Setting up the random
            bool validCycle = false;
            while (!validCycle) {
                remain = NameList.ToList();

                // Going through and matching
                for (int i = 0; i < NameList.Length; i++) {
                    string interest = "";
                    bool repeatLastValue = false;
                    int removalIndex;

                    // Getting the match
                    do {
                        removalIndex = rnd.Next(remain.Count);
                        interest = remain[removalIndex];
                        matches[NameList[i]] = interest;
                        if (i >= NameList.Length - 1 && !ValidPair(i, interest)) {
                            break;
                        }
                        repeatLastValue = true;

                        // Add

                    } while (ValidPair(i, interest));

                    // repeatLastValue is a bool which finds if the last name in the sequence is validly addable to the list.
                    if (!repeatLastValue && i == NameList.Length - 1) validCycle = true;

                    remain.RemoveAt(removalIndex);
                }
            }

            Console.WriteLine("Generated List...");

            // Formatting for saving
            File.WriteAllText(saveDir, "Seed: " + seed.ToString() + "\n\n");
            for (int i = 0; i < Population; i++) {
                string currentName = NameList[i];
                string output = currentName + " has " + matches[currentName] + "\nLikes: " + Likes[currentName] + "\nDislikes: " + Dislikes[currentName] + "\n\n";

                File.AppendAllText(saveDir, output);
            }

            Console.WriteLine("Done.");
            Console.ReadLine();

        }

        private static bool ValidPair(int i, string interest) =>
            NameList[i] == interest || matches[interest] == interest || Avoidance[NameList[i]].Contains(interest) || Avoidance[interest].Contains(NameList[i]);

        private static void ExistCheck(string dir) {
            if (!File.Exists(dir)) File.WriteAllText(dir, "");
        }

        private static string Ask(string text) {
            Console.Write(text);
            return Console.ReadLine();
        }

    }
}
