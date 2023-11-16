using System;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;

namespace Lab1_swp_kalkulator
{
    internal class Program
    {
        // Zmienne globalne
        static SpeechSynthesizer ss = new SpeechSynthesizer();
        static SpeechRecognitionEngine sre;
        // Bool żeby się konsola nie zamknęła
        static bool done = false;
        static void Main(string[] args)
        {
            ss.SetOutputToDefaultAudioDevice(); // Ustawienie mówienie na default urządzenie - z systemu
            ss.Speak("Witam w kalkulatorze"); // Powie Witam w kalulatorze 

            CultureInfo ci = new CultureInfo("pl-PL"); // Potrzebne przy rozpoznawaniu gadania
            sre = new SpeechRecognitionEngine(ci); // Powołanie engine rozpoznawania

            sre.SetInputToDefaultAudioDevice(); // Ustawienie wejścia domyślnego - z systemu

            sre.SpeechRecognized += Sre_SpeechRecognized;

            Choices stopChoice = new Choices(); // Możliwe wybory
            stopChoice.Add("Stop");
            stopChoice.Add("Pomoc");
            GrammarBuilder grammar_stop_builder = new GrammarBuilder(); // builder gramatyki
            grammar_stop_builder.Append(stopChoice); // Dodanie wyboru do gramatyki
            Grammar stop_grammar = new Grammar(grammar_stop_builder); // Budowanie gramatyki

            Choices ch_numbers = new Choices(); // Wybory cyferek
            string[] numbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }; // Cyferki
            string[] neg_numbers = new string[] {"minus 1", "minus 2", "minus 3", "minus 4", "minus 5", "minus 6", "minus 7", "minus 8", "minus 9" }; // Cyferki
            ch_numbers.Add(numbers); // Dodanie wyboru cyferek
            ch_numbers.Add(neg_numbers); // Dodanie wyboru cyferek ujemnych

            Choices ch_actions = new Choices(); // Wybory działania
            string[] actions = new string[] { "plus", "odjąć", "razy", "podzielić przez" }; // Działania
            ch_actions.Add(actions);
            
            GrammarBuilder gb_WhatIsXactionY = new GrammarBuilder();
            gb_WhatIsXactionY.Append("Ile jest");
            gb_WhatIsXactionY.Append(ch_numbers, 0, 2); // wybor cyferek, min słów, max słów
            gb_WhatIsXactionY.Append(ch_actions);
            gb_WhatIsXactionY.Append(ch_numbers, 0, 2);
            Grammar g_WhatIsXplusY = new Grammar(gb_WhatIsXactionY); // Gramatyka

            sre.LoadGrammarAsync(g_WhatIsXplusY); // Ładowanie gramatyki do silnika rozpoznawania
            sre.LoadGrammarAsync(stop_grammar);
            sre.RecognizeAsync(RecognizeMode.Multiple); // Może być dopasowanie do wielu gramatyk - użytkownik może ile to jest... Może też stop i pomoc

            while(done == false) {; }

            Console.WriteLine("\n Wciśnij <enter> aby wyjść \n");
            Console.ReadLine();
        }

        private static void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text; // Tekst który rozpozna silnik rozpoznawania dopasowany do gramatyki

            float confidence = e.Result.Confidence;
            Console.WriteLine("\n Rozpoznano: " + txt + " pewność: " + confidence);

            if(confidence < 0.6)
            {
                ss.SpeakAsync("Proszę powtórzyć");
                return; // Za mała pewność to wróci do lini 55
            }        

            if (txt.IndexOf("Stop") >= 0)
            {
                done = true;
            }

            if(txt.IndexOf("Pomoc") >= 0)
            {
                ss.SpeakAsync("Proszę powiedzieć Ile jest liczba działanie liczba");
                ss.SpeakAsync("Działania to plus, odjąć, razy, podzielić przez");
                return;
                // Wróci do lini 55 bo nie rozpozna
            }

            if(txt.IndexOf("Ile") >= 0 && txt.IndexOf("plus") >= 0 )
            {
                string[] words = txt.Split(' ');
                int num2, num1;

                if (words[2] == "minus") 
                {
                    num1 = int.Parse(words[3]);
                    num1 = -num1;
                }
                else
                {
                    num1 = int.Parse(words[2]);
                }

                if (words[2] != "minus")
                {
                    if (words[4] == "minus")
                    {
                        num2 = int.Parse(words[5]);
                        num2 = -num2;
                    }
                    else
                    {
                        num2 = int.Parse(words[4]);
                    }
                }
                else
                {
                    if (words[5] == "minus")
                    {
                        num2 = int.Parse(words[6]);
                        num2 = -num2;
                    }
                    else
                    {
                        num2 = int.Parse(words[5]);
                    }
                }               

                int sum = num1 + num2;

                if (sum < 0)
                {
                    ss.SpeakAsync("Wynik twojego dodawania to minus " + sum);
                }
                else
                {
                    ss.SpeakAsync("Wynik twojego dodawania to: " + sum);
                }
                

            }

            if (txt.IndexOf("Ile") >= 0 && txt.IndexOf("odjąć") >= 0)
            {
                string[] words = txt.Split(' ');

                int num2, num1;

                if (words[2] == "minus")
                {
                    num1 = int.Parse(words[3]);
                    num1 = -num1;
                }
                else
                {
                    num1 = int.Parse(words[2]);
                }

                if (words[2] != "minus")
                {
                    if (words[4] == "minus")
                    {
                        num2 = int.Parse(words[5]);
                        num2 = -num2;
                    }
                    else
                    {
                        num2 = int.Parse(words[4]);
                    }
                }
                else
                {
                    if (words[5] == "minus")
                    {
                        num2 = int.Parse(words[6]);
                        num2 = -num2;
                    }
                    else
                    {
                        num2 = int.Parse(words[5]);
                    }
                }

                int sum = num1 - num2;

                if (sum < 0)
                {
                    ss.SpeakAsync("Wynik twojego odejmowania to minus " + sum);
                }
                else
                {
                    ss.SpeakAsync("Wynik twojego odejmowania to" + sum);
                }

            }

            if (txt.IndexOf("Ile") >= 0 && txt.IndexOf("razy") >= 0)
            {
                string[] words = txt.Split(' ');

                int num2, num1;

                if (words[2] == "minus")
                {
                    num1 = int.Parse(words[3]);
                    num1 = -num1;
                }
                else
                {
                    num1 = int.Parse(words[2]);
                }

                if (words[2] != "minus")
                {
                    if (words[4] == "minus")
                    {
                        num2 = int.Parse(words[5]);
                        num2 = -num2;
                    }
                    else
                    {
                        num2 = int.Parse(words[4]);
                    }
                }
                else
                {
                    if (words[5] == "minus")
                    {
                        num2 = int.Parse(words[6]);
                        num2 = -num2;
                    }
                    else
                    {
                        num2 = int.Parse(words[5]);
                    }
                }

                int sum = num1 * num2;


                if (sum < 0)
                {
                    ss.SpeakAsync("Wynik twojego mnożenia to minus " + sum);
                }
                else
                {
                    ss.SpeakAsync("Wynik twojego mnożenia to: " + sum);
                }
            }

            if (txt.IndexOf("Ile") >= 0 && txt.IndexOf("podzielić") >= 0)
            {
                string[] words = txt.Split(' ');
                int num2, num1;

                if (words[2] == "minus")
                {
                    num1 = int.Parse(words[3]);
                    num1 = -num1;
                }
                else
                {
                    num1 = int.Parse(words[2]);
                }

                if (words[2] != "minus")
                {
                    if (words[5] == "minus")
                    {
                        num2 = int.Parse(words[6]);
                        num2 = -num2;
                    }
                    else
                    {
                        num2 = int.Parse(words[5]);
                    }
                }
                else
                {
                    if (words[6] == "minus")
                    {
                        num2 = int.Parse(words[7]);
                        num2 = -num2;
                    }
                    else
                    {
                        num2 = int.Parse(words[6]);
                    }
                }

                if (num2 == 0 && num2 == -0)
                {
                    ss.SpeakAsync("Nie wolno dzielić przez zero!");
                }
                else
                {
                    int sum = num1 / num2;

                    if (sum < 0)
                    {
                        ss.SpeakAsync("Wynik twojego dzielenia to minus " + sum);
                    }
                    else
                    {
                        ss.SpeakAsync("Wynik twojego dzielenia to: " + sum);
                    }
                }
                
            }
            
        }
    }
}
