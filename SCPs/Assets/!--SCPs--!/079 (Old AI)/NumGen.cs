using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class NumGen : MonoBehaviour {

   


   // Use this for initialization
   public static int GenerateNumber (int Seed) {

      string Pi = "31415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461284756482337867831652712019091456485669234603486104543266482133936072602491412737245870066063155881748815209209628292540917153643678925903600113305305488204665213841469519415116094330572703657595919530921861173819326117931051185480744623799627495673518857527248912279381830119491";
      string MarkiplierE = "271828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642742746639193200305992181741359662904357290033429526059563073813232862794349076323382988075319525101901157383418793070215408914993488416750924476146066808226480016847741185374234544243710753907774499206955170276183860626133138458300075204493382656029760673711320070932870912744374704723069697720931014169283681902551510865746377211125238978442505695369677078544996996794686445490598793163688923009879312";

      int[] NumberLetterLengths = { 4, 3, 3, 5, 4, 4, 3, 5, 5, 4};

      int ExampleAnswer = 0;

      switch (Seed) {
         case 1: //Sum of letters
            int LetterSum = 0;
            do {
               ExampleAnswer = Rnd.Range(10000, 10000000);
               int Case1Temp = ExampleAnswer;
               LetterSum = 0;
               while (Case1Temp != 0) {
                  LetterSum += NumberLetterLengths[Case1Temp % 10];
                  Case1Temp /= 10;
               }
            } while (ExampleAnswer % LetterSum != 0);
            return ExampleAnswer;

         case 2: //Pi
            int PiStartPos = Rnd.Range(0, 493);
            ExampleAnswer = int.Parse(Pi.Substring(PiStartPos, Rnd.Range(5, 8)));
            return ExampleAnswer;

         case 3: //n^2 - 1
            ExampleAnswer = ((int) Mathf.Pow(2, Rnd.Range(0, 24))) - 1; 
            return ExampleAnswer;

         case 4: //Cubes concatenated
            do {
               ExampleAnswer = int.Parse(Math.Pow(Rnd.Range(1, 27), 3).ToString() + Math.Pow(Rnd.Range(1, 46), 3).ToString());
            } while (ExampleAnswer > 9999999);
            return ExampleAnswer;

         case 5: // n / 5
            do {
               ExampleAnswer = Rnd.Range(0, 10000000);
            } while (ExampleAnswer % 7 != 0);
            return ExampleAnswer;

         case 6: //Divisible by 2 xor 3
            do {
               ExampleAnswer = Rnd.Range(0, 10000000);
            } while (!(ExampleAnswer % 2 == 0 ^ ExampleAnswer % 3 == 0));
            return ExampleAnswer;

         case 7: //4 in odd
            bool Case7Temp = false;

            do {
               ExampleAnswer = Rnd.Range(0, 10000000);
               for (int i = 0; i < ExampleAnswer.ToString().Length; i += 2) {
                  if (ExampleAnswer.ToString()[i].ToString() == "4") {
                     Case7Temp = true;
                  }
               }
            } while (!Case7Temp);

            return ExampleAnswer;

         case 8: //8
            return 8;

         case 9: //Swan

            bool Has1 = false;
            bool Has0 = false;
            bool Has8 = false;

            do {
               ExampleAnswer = Rnd.Range(0, 10000000);
               Has1 = false;
               Has0 = false;
               Has8 = false;
               for (int i = 0; i < ExampleAnswer.ToString().Length; i++) {
                  if (ExampleAnswer.ToString()[i].ToString() == "1") {
                     Has1 = true;
                  }
                  if (ExampleAnswer.ToString()[i].ToString() == "0" && Has1) {
                     Has0 = true;
                  }
                  if (ExampleAnswer.ToString()[i].ToString() == "8" && Has0) {
                     Has8 = true;
                  }
               }
            } while (!Has8);
            return ExampleAnswer;

         case 10: //EEEEEEEEEEEEEEEEEEEEEEEEEE
            int EStartPos = Rnd.Range(0, 493);
            ExampleAnswer = int.Parse(MarkiplierE.Substring(EStartPos, Rnd.Range(5, 8)));
            return ExampleAnswer;

         case 11: //Perfect Square
            ExampleAnswer = (int) Mathf.Pow(Rnd.Range(1, 3163), 2);
            return ExampleAnswer;

         case 12: //DEAF
            string HexAnswer = "";
            do {
               ExampleAnswer = Rnd.Range(0, 10000000);
               HexAnswer = ExMath.ConvertToBase(ExampleAnswer, 16);
            } while (!HexAnswer.Any(x => "DEAF".Contains(x)));
            return ExampleAnswer;

         case 13: //Sum is prime
            do {
               ExampleAnswer = Rnd.Range(10000, 10000000);
            } while (!ExMath.IsPrime(ExMath.DigitSum(ExampleAnswer)));
            //Debug.Log(ExMath.DigitSum(ExampleAnswer));
            return ExampleAnswer;

         case 14: //No Repeats

            int Case14Temp = 0;
            bool[] Case14BoolArr = new bool[10];
            bool Case14Flag = true;

            do {
               ExampleAnswer = Rnd.Range(10000, 10000000);
               Case14Temp = ExampleAnswer;
               Case14Flag = true;
               for (int i = 0; i < 10; i++) {
                  Case14BoolArr[i] = false;
               }
               while (Case14Temp != 0) {
                  if (Case14BoolArr[Case14Temp % 10]) {
                     Case14Flag = false;
                  }
                  Case14BoolArr[Case14Temp % 10] = true;
                  Case14Temp /= 10;
               }
            } while (!Case14Flag);
            return ExampleAnswer;

         case 15:
            int Case15Count = 0;
            int Case15Best = 0;

            do {
               ExampleAnswer = Rnd.Range(0, 10000000);
               Case15Count = 0;
               int[] ar = ExMath.ToIntArray(ExampleAnswer);
               for (int i = 1; i < ar.Length; i++) {
                  if (ar[i] > ar[i - 1]) {
                     Case15Count++;
                     if (Case15Count > Case15Best) {
                        Case15Best = Case15Count;
                     }
                  }
                  else {
                     Case15Count = 0;
                  }
               }
            } while (Case15Best < 4);
            return ExampleAnswer;

         case 16: //Prime
            do {
               ExampleAnswer = Rnd.Range(0, 10000000);
            } while (!ExMath.IsPrime(ExampleAnswer));
            return ExampleAnswer;

         case 17: //Double or half

            bool Case17Flag = true;

            do {
               Case17Flag = true;
               ExampleAnswer = Rnd.Range(0, 10000000);
               int[] AnswerArr = ExMath.ToIntArray(ExampleAnswer);
               for (int i = 1; i < AnswerArr.Length; i++) {
                  if (AnswerArr.Join("").Contains("0") || !(AnswerArr[i - 1] / 2 == AnswerArr[i] || AnswerArr[i - 1] * 2 == AnswerArr[i])) {
                     Case17Flag = false;
                  }
               }
            } while (!Case17Flag);
            return ExampleAnswer;

         case 18: //Leading Zero
            ExampleAnswer = Rnd.Range(0, 1000000);
            return ExampleAnswer;

         case 19: //Year or month

            do {
               ExampleAnswer = Rnd.Range(0, 10000000);
            } while (!(ExampleAnswer.ToString().Contains(System.DateTime.Today.Year.ToString()) || ExampleAnswer.ToString().Contains(System.DateTime.Today.Month.ToString())));
            return ExampleAnswer;

         case 20: // 1234 to 4321
            bool Case20Temp = false;
            do {
               ExampleAnswer = Rnd.Range(0, 10000000);
               for (int i = 0; i < ExampleAnswer.ToString().Length - 4; i += 2) {
                  if (int.Parse(ExampleAnswer.ToString().Substring(i, 4)) > 1234 && int.Parse(ExampleAnswer.ToString().Substring(i, 4)) < 4321) {
                     Case20Temp = true;
                  }
               }
            } while (!Case20Temp);
            return ExampleAnswer;

         case 21: //At most 5 from a perfect cube
            int CubeRoot = Rnd.Range(2, 216);
            ExampleAnswer = (int) Mathf.Pow(CubeRoot, 3) + Rnd.Range(-5, 6);
            return ExampleAnswer;

         case 22: //Palindromic number

            int NumLength = Rnd.Range(5, 8);
            int Case22Temp = Rnd.Range(10, 100);
            if (NumLength != 5) {
               Case22Temp = Case22Temp * 10 + Rnd.Range(0, 10);
            }
            if (NumLength != 6) {
               ExampleAnswer = Case22Temp * 10 + Rnd.Range(0, 10);
               while (Case22Temp != 0) {
                  ExampleAnswer *= 10;
                  ExampleAnswer += Case22Temp % 10;
                  Case22Temp /= 10;
               }
            }
            else {
               ExampleAnswer = Case22Temp;
               while (Case22Temp != 0) {
                  ExampleAnswer *= 10;
                  ExampleAnswer += Case22Temp % 10;
                  Case22Temp /= 10;
               }
            }
            return ExampleAnswer;

         case 23: //Greater or whatever
            bool Case23Flag = true;

            do {
               Case23Flag = true;
               ExampleAnswer = Rnd.Range(0, 10000000);

               int[] AnswerArr = ExMath.ToIntArray(ExampleAnswer);
               for (int i = 1; i < AnswerArr.Length; i++) {
                  if (!(AnswerArr[i - 1] < AnswerArr[i] || AnswerArr[i] == 0)) {
                     Case23Flag = false;
                  }
               }
            } while (!Case23Flag);
            return ExampleAnswer;

         case 24: //Self explanatory
            ExampleAnswer = 9999999;
            return ExampleAnswer;

         case 25: //Mod 9 is not even
            do {
               ExampleAnswer = Rnd.Range(0, 10000000);
            } while (ExampleAnswer % 9 % 2 == 0);
            return ExampleAnswer;

         default:
            return 8008135;
      }
   }
}
