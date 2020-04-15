using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Gramadan;

namespace Gramadoir
{
    /// <summary>
    /// Basic class that provides a command line interface to take verb
    /// definitions from the Irish National Morphology Database and print out
    /// verb definitions and forms in a basic JSON format for parsing
    /// </summary>
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("Please provide XML file as argument.");
                Environment.Exit(1);
            }
            else if (args.Count() > 1)
            {
                Console.WriteLine($"Expected single argument but received {args.Count()}");
                Environment.Exit(2);
            }
            string filename = args[0];
            FileInfo file = new FileInfo(filename);
            if (!file.Exists)
            {
                Console.WriteLine($"File not found: {filename}");
                Environment.Exit(3);
            }

            Verb verb = new Verb(filename);
            VP verbPrinter = new VP(verb);

            var output = new Dictionary<string, object>
            {
                ["verbName"] = verbPrinter.moods[VPMood.Imper][VPPerson.Sg2][VPPolarity.Pos][0].value,
                ["verbalNounForms"] = verb.verbalNoun.Select(vn => vn.value),
                ["verbalAdjectiveForms"] = verb.verbalAdjective.Select(va => va.value),
                ["forms"] = new List<Dictionary<string, string>>()
            };

            foreach (var tense in verbPrinter.tenses)
            {
                foreach (var shape in tense.Value)
                {
                    foreach (var person in shape.Value)
                    {
                        foreach (var polarity in person.Value)
                        {
                            foreach (var form in polarity.Value)
                            {
                                var formVal = new Dictionary<string, string>
                                {
                                    ["value"] = form.value,
                                    ["tense"] = tense.Key.ToString(),
                                    ["shape"] = shape.Key.ToString(),
                                    ["person"] = person.Key.ToString(),
                                    ["polarity"] = polarity.Key.ToString()
                                };
                                var forms = (List<Dictionary<string, string>>)output["forms"];
                                forms.Add(formVal);
                            }
                        }
                    }
                }
            }

            foreach (var mood in verbPrinter.moods)
            {
                foreach (var person in mood.Value)
                {
                    foreach (var polarity in person.Value)
                    {
                        foreach (var form in polarity.Value)
                        {
                            var formVal = new Dictionary<string, string>
                            {
                                ["value"] = form.value,
                                ["tense"] = mood.Key.ToString(),
                                ["shape"] = "Declar",
                                ["person"] = person.Key.ToString(),
                                ["polarity"] = polarity.Key.ToString()
                            };
                            var forms = (List<Dictionary<string, string>>)output["forms"];
                            forms.Add(formVal);
                        }
                    }
                }
            }

            string json = JsonConvert.SerializeObject(output);
            Console.WriteLine(json);
        }
    }
}
