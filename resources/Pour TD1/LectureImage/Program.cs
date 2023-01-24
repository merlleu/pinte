using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace LectureImage
{
    class Program
    {

        static void Main(string[] args)
        {
            // http://wxfrantz.free.fr/index.php?p=format-bmp


            byte[] myfile = File.ReadAllBytes("./Images/Test.bmp");
            //myfile est un vecteur composé d'octets représentant les métadonnées et les données de l'image

            //Métadonnées du fichier
            Console.WriteLine("\n Header \n");
            for (int i = 0; i < 14; i++)
                Console.Write(myfile[i] + " ");
            //Métadonnées de l'image
            Console.WriteLine("\n HEADER INFO \n");
            for (int i = 14; i < 54; i++)
                Console.Write(myfile[i] + " ");
            //L'image elle-même
            Console.WriteLine("\n IMAGE \n");
            for (int i = 54; i < myfile.Length; i = i + 60)
            {
                for (int j = i; j < i + 60; j++)
                {
                    Console.Write(myfile[j] + " ");


                }
                Console.WriteLine();
            }

            myfile[54] = 255;
            myfile[55] = 0;
            myfile[56] = 0;
       
            /*
            230 4 0 0 little endian

            0 0 4 230 = 230*250^0 + 4*256^1+0*256^2+0*256^3



            */
            File.WriteAllBytes("./Images/Sortie.bmp", myfile);
  
            Console.ReadLine();
        }
    }
}
