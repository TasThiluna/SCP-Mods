using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class SCP2719 : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public TextMesh[] InputText;
   public TextMesh[] InsideResultText;

   int InputIndex;

   public KMSelectable Mod;

   private char[] CorrectLetters;
   char CurLet = ' ';
   string Inside = null;
   string UserInput = "";

   string InputedBecome = null;

   bool Focused;

   bool WillStrike;

   string[] BecomeInsides = { "GREEN", "DISARMED", "CONTAINED", "MISSING", "TRANSCENDENCE" };
   string[] IsInsides = { "STATUS LIGHT", "MODULE", "SCP", "POINTER", "DEFUSER" };
   List<int> Indices = new List<int> { };

   string[] LetterGroups = { "ABCDE", "FGHIJ", "KLMNO", "PQRST", "UVWXYZ" };
   string[] SafeGuesses = { "Neutralized", "Euclid", "Redacted", "Keter", "Safe", "Jack", "Bright", "Alto", "Clef", "Charles", "Gears", "Everette", "King", "Quantum", "Unknown", "Apollyon", "████████████", "Expunged", "Metaphysical", "Zermelo" };

   List<List<char>> AllSets = new List<List<char>> { };

   bool AnimateText;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake () {
      ModuleId = ModuleIdCounter++;

      Mod.OnFocus += delegate () { Focused = true; };
      Mod.OnDefocus += delegate () { Focused = false; };

      if (Application.isEditor) {
         Focused = true;
      }
   }

   KeyCode[] TheKeys = {
      KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
      KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
      KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M,

      KeyCode.Keypad0, KeyCode.Alpha0, KeyCode.Keypad1, KeyCode.Alpha1, KeyCode.Keypad2, KeyCode.Alpha2, KeyCode.Keypad3, KeyCode.Alpha3, KeyCode.Keypad4, KeyCode.Alpha4, KeyCode.Keypad5, KeyCode.Alpha5, KeyCode.Keypad6, KeyCode.Alpha6, KeyCode.Keypad7, KeyCode.Alpha7, KeyCode.Keypad8, KeyCode.Alpha8, KeyCode.Keypad9, KeyCode.Alpha9,

      KeyCode.Space, KeyCode.Backspace, KeyCode.KeypadEnter, KeyCode.Return
   };
   string NotTheKeys = "QWERTYUIOPASDFGHJKLZXCVBNM00112233445566778899 <>>";

   void Start () {
      GetLetters();
      for (int i = 0; i < SafeGuesses.Length; i++) {
         SafeGuesses[i] = SafeGuesses[i].ToUpperInvariant();
      }
      for (int i = 0; i < 17; i++) {
         InputText[i].text = "";
         InsideResultText[i].text = "";
      }
   }

   void GetLetters () {
      GetLettersHelper(Bomb.GetModuleNames());
   }

   void GetLetters (string ModuleName) {
      List<string> Temp = new List<string> { };
      Temp.Add(ModuleName);
      GetLettersHelper(Temp);
   }

   void GetLettersHelper (List<string> Temp) {
      var ModulesInLogic = Temp.Select(name => name.ToUpperInvariant()).ToArray();

      // For each letter, build up a sorted list of indices
      var alphabetSets = Enumerable.Range(0, 26).Select(i => new List<int>()).ToArray();
      for (int i = 0; i < ModulesInLogic.Length; i++) {
         var moduleLetters = ModulesInLogic[i].ToUpperInvariant().Intersect("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
         foreach (char c in moduleLetters) {
            alphabetSets[c - 'A'].Add(i);
         }
      }

      // Turn the index lists into strings so we get key hashing for free
      var alphabetKeys = alphabetSets.Select(indices => indices.Select(i => i.ToString()).Join(",")).ToArray();

      var keysToCorrectChars = new Dictionary<string, List<char>>();
      for (int i = 0; i < alphabetKeys.Length; i++) {
         var key = alphabetKeys[i];
         if (!keysToCorrectChars.ContainsKey(key)) {
            keysToCorrectChars[key] = new List<char>();
         }
         keysToCorrectChars[key].Add((char) ('A' + i));
      }

      var singletons = keysToCorrectChars.Where(kvp => kvp.Value.Count() == 1);

      for (int i = 0; i < alphabetKeys.Count(); i++) {
         AllSets.Add(keysToCorrectChars[alphabetKeys[i]]);
      }

      if (singletons.Any()) {
         CorrectLetters = new[] { singletons.PickRandom().Value.First() };
      }
      else {
         CorrectLetters = AllSets.PickRandom().ToArray();
      }

      UpdateCurrentLetter();
   }

   void Update () {
      if (ModuleSolved || AnimateText) {
         return;
      }
      if (Focused) {
         for (int i = 0; i < TheKeys.Count(); i++) {
            if (Input.GetKeyDown(TheKeys[i])) {
               HandleKey(NotTheKeys[i]);
            }
         }
      }
   }

   void HandleKey (char Input) {
      if (Input == '>') {
         HandleEnter();
         return;
      }
      else if (Input == '<') {
         if (UserInput != "") {
            UserInput = UserInput.Substring(0, UserInput.Length - 1);
         }
      }
      else {
         if (UserInput.Length == 0 || (UserInput.Length != 0 && UserInput.Last() == ' ')) {
            UserInput += Input.ToString();
         }
         else {
            UserInput += Input.ToString().ToLowerInvariant();
         }
      }
      InputText[InputIndex].text = UserInput;
   }

   void Strike () {
      StartCoroutine(UpdateInsideOutsideText("Outside"));
      WillStrike = false;
      UserInput = "";
      InputedBecome = null;
      GetComponent<KMBombModule>().HandleStrike();
   }

   void Solve () {
      GetComponent<KMBombModule>().HandlePass();
      StartCoroutine(UpdateInsideOutsideText("Went Inside"));
      ModuleSolved = true;
   }

   void HandleEnter () {

      if (UserInput == "") {
         return;
      }

      UserInput = UserInput.ToUpper();

      InputIndex++;
      bool Recognized = false;

      if (BecomeInsides.Contains(UserInput)) {
         StartCoroutine(UpdateInsideOutsideText("Became Inside"));
         InputedBecome = UserInput;
         if (InputedBecome != BecomeInsides[GetIndexOfSubmissionBullshit()]) {
            WillStrike = true;
         }
         UserInput = "";
         return;
      }

      if (InputedBecome != null) {
         if (!WillStrike && UserInput == IsInsides[Array.IndexOf(BecomeInsides, InputedBecome)]) {
            Solve();
         }
         else {
            Strike();
         }
         UserInput = "";
         return;
      }

      if (SafeGuesses.Contains(UserInput)) {
         Recognized = true;
         if (UserInput.Contains(CurLet)) {
            if (Rnd.Range(0, 3) == 0) {
               StartCoroutine(UpdateInsideOutsideText("Became Inside"));
               Inside = UserInput;
               Inside = Regex.Replace(Inside, "[^a-zA-Z0-9\\s]", String.Empty);
               UpdateCurrentLetter();
            }
            else {
               StartCoroutine(UpdateInsideOutsideText("Went Inside"));
            }
         }
         else {
            StartCoroutine(UpdateInsideOutsideText("Outside"));
         }
         UserInput = "";
         return;
      }

      foreach (String ModRaw in Bomb.GetModuleNames()) {
         string Mod = ModRaw;
         Mod = Regex.Replace(Mod, "[^a-zA-Z0-9\\s]", String.Empty);
         Mod = Mod.ToUpperInvariant();
         if (Mod == UserInput) {
            Recognized = true;
            if (UserInput.Contains(CurLet)) {
               if (Rnd.Range(0, 10) == 0) {
                  StartCoroutine(UpdateInsideOutsideText("Became Inside"));
                  Inside = UserInput;
                  UpdateCurrentLetter();
               }
               else {
                  StartCoroutine(UpdateInsideOutsideText("Went Inside"));
               }
            }
            else {
               StartCoroutine(UpdateInsideOutsideText("Outside"));
            }
         }
      }

      if (!Recognized) {
         StartCoroutine(UpdateInsideOutsideText("Unrecognized"));
      }
      UserInput = "";
   }

   IEnumerator UpdateInsideOutsideText (string input) {
      AnimateText = true;
      switch (input) {
         case "Became Inside":
            Debug.LogFormat("[Inside #{0}] The input \"{1}\" became inside.", ModuleId, UserInput);
            break;
         case "Went Inside":
            Debug.LogFormat("[Inside #{0}] The input \"{1}\" went inside.", ModuleId, UserInput);
            break;
         case "Outside":
            Debug.LogFormat("[Inside #{0}] The input \"{1}\" is outside.", ModuleId, UserInput);
            break;
         case "Unrecognized":
            Debug.LogFormat("[Inside #{0}] The input \"{1}\" is unrecognized.", ModuleId, UserInput);
            break;
      }
      InsideResultText[InputIndex - 1].text = "";
      for (int i = 0; i < input.Length; i++) {
         InsideResultText[InputIndex - 1].text += input[i].ToString();
         if (input[i] != ' ') {
            yield return new WaitForSeconds(.01f);
         }
      }
      InputIndex %= 17;
      if (InputIndex == 0) {
         for (int i = 0; i < 17; i++) {
            InputText[i].text = "";
            InsideResultText[i].text = "";
         }
      }
      AnimateText = false;
   }

   void UpdateCurrentLetter () {
      int count = 0;
      char OldCur = CurLet;

      if (Inside == null) {
         CurLet = CorrectLetters.PickRandom();
      }
      else {
         do {
            CorrectLetters = AllSets.PickRandom().ToArray();
            CurLet = CorrectLetters.PickRandom();
            count++;
         } while (!Inside.Contains(CurLet) && count < 10000);
         if (count == 10000) {
            StartCoroutine(UpdateInsideOutsideText("ERROR"));
            StartCoroutine(UpdateInsideOutsideText("Went Inside"));
            CurLet = OldCur;
         }
      }
      Debug.LogFormat("[Inside #{0}] \"{1}\" became inside.", ModuleId, CurLet);
      Debug.LogFormat("[Inside #{0}] {1} should become inside, and {2} should go inside.", ModuleId, BecomeInsides[GetIndexOfSubmissionBullshit()], IsInsides[GetIndexOfSubmissionBullshit()]);
   }

   int GetIndexOfSubmissionBullshit () {
      for (int i = 0; i < 5; i++) {
         if (LetterGroups[i].Contains(CurLet)) {
            return i;
         }
      }
      return 8008135;
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} XXXX to submit that word.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
      Command = Command.ToUpper().Trim();
      if (!Regex.IsMatch(Command, "[^a-zA-Z0-9\\s]")) {
         for (int i = 0; i < Command.Length; i++) {
            HandleKey(Command[i]);
            yield return new WaitForSeconds(.1f);
         }
         HandleEnter();
      }
      else {
         yield return "sendtochaterror I don't understand!";
      }
   }

   IEnumerator TwitchHandleForcedSolve () {
      int weed = GetIndexOfSubmissionBullshit();
      
      if (InputedBecome == null) {
         yield return ProcessTwitchCommand(BecomeInsides[weed]);
      }
      yield return ProcessTwitchCommand(IsInsides[weed]);
   }
}
